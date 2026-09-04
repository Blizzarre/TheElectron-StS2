#region

using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using SmartFormat;
using TheElectron.TheElectronCode.Formatters;

#endregion

namespace TheElectron.TheElectronCode.Patches;

[HarmonyPatch(typeof(LocManager), nameof(LocManager.LoadLocFormatters))]
public class LocManagerLoadLocFormattersPatch
{
    [HarmonyPostfix]
    private static void AddCustomFormatters()
    {
        Smart.Default.AddExtensions(new GrayOutFormatter());
    }
}