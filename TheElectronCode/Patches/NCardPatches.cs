using BaseLib.Utils.Patching;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;
using TheElectron.TheElectronCode.Field;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Patches;

[HarmonyPatch(typeof(NCard), nameof(NCard.GetCostTextColorInHand))]
public class NCardGetCostTextColorInHandPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardCostColor costColor, ref Color __result)
    {
        if (costColor == ElectronEnums.CostColorDrain)
        {
            __result = new Color(0.729f, 0.424f, 0.953f);
            return false;
        }

        if (costColor == ElectronEnums.CostColorEmpty)
        {
            __result = new Color(0.412f, 0.412f, 0.412f);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard.GetCostOutlineColorInHand))]
public class NCardGetCostOutlineColorInHandPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardCostColor costColor, ref Color __result)
    {
        if (costColor == ElectronEnums.CostColorDrain)
        {
            __result = new Color(0.397f, 0.133f, 0.616f);
            return false;
        }

        if (costColor == ElectronEnums.CostColorEmpty)
        {
            __result = new Color(0.149f, 0.149f, 0.149f);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
internal class NCardUpdateVisualsPatch
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new InstructionPatcher(instructions).Match(new InstructionMatcher()
            .ldarg_0()
            .ldarg_1()
            .call(typeof(NCard), nameof(NCard.UpdateStarCostVisuals), [typeof(PileType)])
        ).Insert([
            CodeInstruction.LoadArgument(0),
            CodeInstruction.LoadArgument(1),
            CodeInstruction.Call(typeof(NCardUpdateVisualsPatch), nameof(UpdateElectronVisuals))
        ]);
    }
    
    private static void UpdateElectronVisuals(NCard instance, PileType pileType)
    {
        var indicator = ElectronNode.NElectronCardIndicator[instance];
        indicator.UpdateVisuals();
    }
}