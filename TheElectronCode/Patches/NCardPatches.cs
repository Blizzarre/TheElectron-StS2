using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;
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
            __result = new Color(0.15f, 0.15f, 0.15f);
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
            __result = new Color(0.05f, 0.05f, 0.05f);
            return false;
        }
        
        return true;
    }
}