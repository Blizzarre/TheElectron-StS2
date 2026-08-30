using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Patches;

// Add custom resources to preload list
[HarmonyPatch(typeof(AssetSets), nameof(AssetSets.CommonAssets), MethodType.Getter)]
internal static class AssetSetsCommonAssetPatch
{
    [HarmonyPostfix]
    private static void Postfix(ref IReadOnlySet<string> __result)
    {
        __result = __result.Concat(ElectronResource.AssetPaths).ToHashSet();
    }
}