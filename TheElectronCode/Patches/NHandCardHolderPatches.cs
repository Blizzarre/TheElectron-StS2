using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using TheElectron.TheElectronCode.Extensions;

namespace TheElectron.TheElectronCode.Patches;

// Patches to make Drain card glow different color when it's going to activate Drain effect (not enough Energy)
[HarmonyPatch(typeof(NHandCardHolder), nameof(NHandCardHolder.UpdateCard))]
public static class NHandCardHolderUpdateCardPatch
{
    
    [HarmonyPostfix]
    private static void Postfix(NHandCardHolder __instance)
    {
        var card = __instance.CardNode?.Model;
        if (card == null || !card.CanPlay() || card.ShouldGlowGold || card.ShouldGlowRed)
            return;
        
        if (card.ShouldGlowBlack())
        {
            __instance.CardNode?.CardHighlight.Modulate = new Color(0.5f, 0.5f, 0.5f, 0.98f);
        }
        if (card.ShouldGlowPurple())
        {
            __instance.CardNode?.CardHighlight.Modulate = new Color(0.505f, 0.104f, 0.931f, 0.98f);
        }
    }
}

[HarmonyPatch(typeof(NHandCardHolder), nameof(NHandCardHolder.Flash))]
public static class NHandCardHolderFlashPatch
{
    
    [HarmonyPostfix]
    private static void Postfix(NHandCardHolder __instance)
    {
        var card = __instance.CardNode?.Model;
        if (card == null || !card.CanPlay() || card.ShouldGlowGold || card.ShouldGlowRed)
            return;
        
        if (card.ShouldGlowBlack())
        {
            __instance._flash.Modulate = new Color(0.5f, 0.5f, 0.5f, 0.98f);
        }
        if (card.ShouldGlowPurple())
        {
            __instance._flash.Modulate = new Color(0.505f, 0.104f, 0.931f, 0.98f);
        }
    }
}
