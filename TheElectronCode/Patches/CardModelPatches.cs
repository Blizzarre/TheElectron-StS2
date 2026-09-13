using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Cards;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Field;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.Powers;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Patches;

// Patch to spend excess Energy amount with HP later with Drain cards or setting the spending to 0 for Empty cards.
[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources), MethodType.Async)]
internal class CardModelSpendResourcesPatch
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        FieldInfo? energyToSpendField = null;

        return new InstructionPatcher(instructions).Match(new InstructionMatcher()
            .ldloc_1()
            .ldarg_0()
            .ldfld().PredicateMatch(o =>
            {
                if (o is not FieldInfo field || !field.Name.Contains("energyToSpend")) return false;
                energyToSpendField = field;
                return true;
            })
            .call(typeof(CardModel), nameof(CardModel.SpendEnergy))
        ).Step(-3).Insert([
            CodeInstruction.LoadArgument(0),
            CodeInstruction.LoadLocal(1),
            CodeInstruction.LoadArgument(0),
            new CodeInstruction(OpCodes.Ldfld, energyToSpendField),
            CodeInstruction.Call(typeof(CardModelSpendResourcesPatch), nameof(BeforeEnergySpent)),
            new CodeInstruction(OpCodes.Stfld, energyToSpendField)
        ]);
    }

    private static int BeforeEnergySpent(CardModel cardModel, int energyToSpend)
    {
        var energy = cardModel.Owner.PlayerCombatState?.Energy ?? 0;

        if (cardModel is ElectronDepleteCard depleteCard && energyToSpend >= energy)
            depleteCard.IsEnergyDepleted = true;

        if (cardModel.Keywords.Contains(ElectronKeywords.Drain))
        {
            var excess = energyToSpend - energy;
            if (excess > 0)
            {
                ElectronField.DrainExcessEnergy[cardModel] = excess;

                // only pay exact energy amount
                energyToSpend = energy;
            }
        }
        
        if (cardModel is ElectronEmptyCard emptyCard)
        {
            if (energy >= energyToSpend)
            {
                emptyCard.HasPaidEnergyCost = true;
            }
            
            if (energy == 0)
            {
                emptyCard.IsPlayedAsEmpty = true;
                energyToSpend = 0;
            }
        }

        return energyToSpend;
    }
}

// Patch to take away HP based on the excess Energy needed to play the card
[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper), MethodType.Async)]
internal class CardModelOnPlayWrapperPatch
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        // Place method call after CombatManager.Instance.WaitForUnpause()
        return AsyncMethodCall.Create(generator, instructions, original,
            AccessTools.Method(typeof(CardModelOnPlayWrapperPatch), nameof(DrainHp)),
            afterState: AccessTools.Method(typeof(CombatManager), nameof(CombatManager.WaitForUnpause)));
    }

    private static async Task DrainHp(CardModel __instance, PlayerChoiceContext choiceContext)
    {
        var excessEnergy = ElectronField.DrainExcessEnergy[__instance];
        if (excessEnergy > 0 && __instance.CombatState != null)
        {
            var player = __instance.Owner;
            
            var totalAmount = excessEnergy * 2m;
            var drainAmount = totalAmount;
            var electronCombatState = player.PlayerCombatState?.Electron();
            if (electronCombatState != null)
            {
                var faradDrain = Math.Min(electronCombatState.Farad, drainAmount);
                await ElectronPlayerCmd.LoseFarad(choiceContext, player, faradDrain, __instance);
                drainAmount -= faradDrain;
            }
            
            // TODO: Sorry for hard coding, replace this with hook later!!
            var power = player.Creature.GetPower<AlephNullPower>();
            if (drainAmount > 0 && power != null)
            {
                await PowerCmd.Decrement(power);
                drainAmount = 0;
            }
            
            if (drainAmount > 0)
            {
                await CreatureCmd.Damage(choiceContext, player.Creature,
                    new DamageVar(drainAmount, ValueProp.Unpowered | ValueProp.Unblockable), __instance, null);
            }

            await ElectronHook.AfterFaradOrHpDrained(__instance.CombatState, choiceContext, player, totalAmount,
                __instance);
        }
    }
}

// Patch to clean up single turn Drain
[HarmonyPatch(typeof(CardModel), nameof(CardModel.EndOfTurnCleanup))]
internal class CardModelEndOfTurnCleanupPatch
{
    [HarmonyPrefix]
    static void Prefix(CardModel __instance)
    {
        ElectronField.SingleTurnDrain[__instance] = false;
    }
}