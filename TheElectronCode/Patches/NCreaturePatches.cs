using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs;
using TheElectron.TheElectronCode.Field;
using TheElectron.TheElectronCode.Nodes.Quarks;

namespace TheElectron.TheElectronCode.Patches;

[HarmonyPatch(typeof(NCreature), nameof(NCreature._Ready))]
internal class NCreatureReadyPatch
{
    // TODO figure out navigation
    // private static void UpdateQuarkNavigation(NCreature __instance)
    // {
    //     var quarkManager = ElectronNode.NQuarkManager[__instance];
    //     if (quarkManager is { DefaultFocusOwner: not null })
    //         __instance.Hitbox.FocusNeighborTop = quarkManager.DefaultFocusOwner.GetPath();
    // }

    [HarmonyPrefix]
    private static void Prefix(NCreature __instance)
    {
        if (!__instance.Entity.IsPlayer) return;
        var quarkManager = NQuarkManager.Create(__instance, LocalContext.IsMe(__instance.Entity));
        __instance.AddChildSafely(quarkManager);
        quarkManager.Position = Vector2.Zero;
        ElectronNode.NQuarkManager[__instance] = quarkManager;
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetOrbManagerPosition))]
internal class NCreatureSetOrbManagerPositionPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCreature __instance)
    {
        if (!__instance.Entity.IsPlayer) return;
        var quarkManager = ElectronNode.NQuarkManager[__instance];

        if (quarkManager == null) return;
        quarkManager.Scale = __instance.Visuals.Scale.X > 1f
            ? Vector2.One
            : __instance.Visuals.Scale.Lerp(Vector2.One, 0.5f);
        quarkManager.Position = (__instance.Visuals.OrbPosition.Position + NQuarkManager.CenterOffset) *
                                Mathf.Min(__instance.Visuals.Scale.X, 1.25f);
    }
}

[HarmonyPatch(typeof(NCreature), "AnimDie")]
internal class NCreatureAnimDiePatch
{
    [HarmonyPostfix]
    private static async Task Postfix(Task results, NCreature __instance)
    {
        await results;
        if (!RunManager.Instance.IsSingleplayerOrFakeMultiplayer)
        {
            var quarkManager = ElectronNode.NQuarkManager[__instance];
            quarkManager?.ClearQuarks();
        }
    }
}

[HarmonyPatch(typeof(NCreature), "OnCombatEnded")]
internal class NCreatureOnCombatEndedPatch
{
    [HarmonyPrefix]
    private static void Prefix(NCreature __instance)
    {
        var quarkManager = ElectronNode.NQuarkManager[__instance];
        quarkManager?.ClearQuarks();
    }
}