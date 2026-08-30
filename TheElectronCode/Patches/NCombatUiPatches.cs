#region

using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using TheElectron.TheElectronCode.Field;

#endregion

namespace TheElectron.TheElectronCode.Patches;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal class NCombatUiPatches
{
    [HarmonyPostfix]
    private static void Postfix(NCombatUi __instance, CombatState state)
    {
        var faradCounter = ElectronNode.NFaradCounter[__instance];
        faradCounter.Initialize(LocalContext.GetMe(state)!);
        faradCounter.Reparent(__instance._energyCounter);
        faradCounter.ShowBehindParent = true;
        faradCounter.Position = new Vector2(128, 0);
    }
}