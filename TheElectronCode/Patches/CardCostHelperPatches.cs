using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers.Models;
using MegaCrit.Sts2.Core.Models;
using TheElectron.TheElectronCode.Cards;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Patches;

// This is way more complicated than it needs to be but idc - Blizz
// Also most likely not very compatible with other mods that mess with this.
[HarmonyPatch(typeof(CardCostHelper), nameof(CardCostHelper.GetEnergyCostColor))]
public class CardCostHelperGetEnergyCostColorPatches
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new InstructionPatcher(instructions).Match(new InstructionMatcher()
                .ldloc_1()
                .ldarg_0()
                .callvirt(AccessTools.PropertyGetter(typeof(CardModel), nameof(CardModel.EnergyCost)))
                .ldc_i4_0()
                .callvirt(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))
                .call_any() // GetColorForHookModifiedCost
                .ret()
            ).Step(-1).Insert([
                // Above this is CardCostHelper.GetColorForHookModifiedCost call
                CodeInstruction.LoadLocal(1), // hookModifiedCost
                CodeInstruction.LoadArgument(0), // card
                // Consume the color enum from the stack and replace with our own conditionally
                CodeInstruction.Call(typeof(CardCostHelperGetEnergyCostColorPatches),
                    nameof(OverrideCardCostColorForDrain))
            ])
            .Match(new InstructionMatcher()
                .ldarg_0()
                .callvirt(AccessTools.PropertyGetter(typeof(CardModel), nameof(CardModel.EnergyCost)))
                .ldc_i4_2()
                .callvirt(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))
            ).Insert([new CodeInstruction(OpCodes.Dup)]) // Dup local cost int value
            .Match(new InstructionMatcher()
                    .ldarg_0()
                    .callvirt(AccessTools.PropertyGetter(typeof(CardModel), nameof(CardModel.EnergyCost)))
                    .ldc_i4_0()
                    .callvirt(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))
                    .call_any() // GetColorForLocalCost
            ).Insert([
                // Local Cost Dup earlier
                // result from CardCostHelper.GetColorForHookModifiedCost call
                CodeInstruction.LoadArgument(0), // card
                // Consume the color enum from the stack and replace with our own conditionally
                CodeInstruction.Call(typeof(CardCostHelperGetEnergyCostColorPatches),
                    nameof(OverrideCardCostColorForDrainLocal))
            ])
            .Match(new InstructionMatcher()
                .ldc_i4_0() // Unmodified color
                .ret()
            ).Step(-1).Insert([
                CodeInstruction.LoadArgument(0), // card
                CodeInstruction.Call(typeof(CardCostHelperGetEnergyCostColorPatches),
                    nameof(OverrideCardCostColorForDrainFinal))
            ]);
    }

    private static CardCostColor GetCardCostColor(CardCostColor orig, CardModel card, int cost)
    {
        var energy = card.Owner.PlayerCombatState?.Energy ?? 0;
        if (card.Keywords.Contains(ElectronKeywords.Drain) && energy < cost) return ElectronEnums.CostColorDrain;
        return card.ShouldGlowBlack() ? ElectronEnums.CostColorEmpty : orig;
    }

    private static CardCostColor OverrideCardCostColorForDrain(CardCostColor orig, decimal hookModifiedCost,
        CardModel card)
    {
        return GetCardCostColor(orig, card, (int)hookModifiedCost);
    }

    private static CardCostColor OverrideCardCostColorForDrainLocal(int localCost, CardCostColor orig, CardModel card)
    {
        return GetCardCostColor(orig, card, localCost);
    }

    private static CardCostColor OverrideCardCostColorForDrainFinal(CardCostColor orig, CardModel card)
    {
        return GetCardCostColor(orig, card, card.EnergyCost.GetWithModifiers(CostModifiers.None));
    }
}