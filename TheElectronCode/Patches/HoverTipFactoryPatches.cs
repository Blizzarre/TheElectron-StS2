using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Patches;

[HarmonyPatch(typeof(HoverTipFactory), nameof(HoverTipFactory.FromKeyword))]
public static class HoverTipFactoryPatch
{
    [HarmonyPostfix]
    static void Postfix(CardKeyword keyword, ref IHoverTip __result)
    {
        if (keyword == ElectronKeywords.Drain && __result is HoverTip { Icon: null } hoverTip)
        {
            var icon = PreloadManager.Cache.GetTexture2D("drain".CardUiResourcePath().ToRes());
            hoverTip.Icon = icon;

            HoverTipFactory._keywordHoverTips[keyword] = hoverTip;
            __result = hoverTip;
        }
    }
}