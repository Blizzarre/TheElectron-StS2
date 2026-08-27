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
using TheElectron.TheElectronCode.Field;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Patches;

// Patch to spend excess Energy amount with HP later with Drain cards or setting the spending to 0 for Empty cards.
[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources), MethodType.Async)]
internal class CardModelSpendResourcesPatch
{
    [HarmonyTranspiler]
    static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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
            } )
            .call(typeof(CardModel), nameof(CardModel.SpendEnergy))
        ).Step(-3).Insert([
            CodeInstruction.LoadArgument(0),
            CodeInstruction.LoadLocal(1),
            CodeInstruction.LoadArgument(0),
            new CodeInstruction(OpCodes.Ldfld, energyToSpendField),
            CodeInstruction.Call(typeof(CardModelSpendResourcesPatch), nameof(BeforeEnergySpent)),
            new CodeInstruction(OpCodes.Stfld, energyToSpendField),
        ]);
    }

    private static int BeforeEnergySpent(CardModel cardModel, int energyToSpend)
    {
        var energy = cardModel.Owner.PlayerCombatState!.Energy;
        
        if (cardModel is ElectronEmptyCard emptyCard && energy == 0)
        {
            emptyCard.IsPlayedAsEmpty = true;
            return 0;
        }

        if (cardModel is ElectronDepleteCard depleteCard && energyToSpend >= energy)
        {
            depleteCard.IsEnergyDepleted = true;
        }

        if (cardModel.Keywords.Contains(ElectronKeywords.Drain))
        {
            var excess = energyToSpend - energy;
            if (excess <= 0) return energyToSpend;

            ElectronField.DrainExcessEnergy[cardModel] = excess;

            // only pay exact energy amount
            return energy;
        }

        return energyToSpend;
    }
}

// Patch to take away HP based on the excess Energy needed to play the card
[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper), MethodType.Async)]
internal class CardModelOnPlayWrapperPatch
{
    [HarmonyTranspiler]
    static List<CodeInstruction> Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions,
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
        if (excessEnergy > 0)
        {
            // TODO: Maybe hook this to determine hp conversion ratio?
            
            // TODO: Take 'HP' away from Farad first
            await CreatureCmd.Damage(choiceContext, __instance.Owner.Creature,
                new DamageVar(2m * excessEnergy, ValueProp.Unpowered | ValueProp.Unblockable), __instance, null);
            ElectronField.DrainExcessEnergy[__instance] = 0;
            
            // TODO: Hook for Quantum Link trigger (must trigger after damage)
        }
    }
}