using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using TheElectron.TheElectronCode.Hooks;

namespace TheElectron.TheElectronCode.Patches;

[HarmonyPatch(typeof(PlayerCmd), nameof(PlayerCmd.LoseEnergy))]
public class PlayerCmdLoseEnergyPatch
{
    [HarmonyPostfix]
    static async Task Postfix(Task results, decimal amount, Player player)
    {
        await results;
        if (amount > 0 && !CombatManager.Instance.IsEnding)
        {
            await ElectronHook.AfterEnergyLost(player.Creature.CombatState!, player, amount);
        }
    }
}

[HarmonyPatch(typeof(PlayerCmd), nameof(PlayerCmd.GainEnergy), MethodType.Async)]
public class PlayerCmdGainEnergyPatch
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        return AsyncMethodCall.Create(generator, instructions, original,
            AccessTools.Method(typeof(PlayerCmdGainEnergyPatch), nameof(AfterEnergyGained)), afterState: original
        );
    }

    private static async Task AfterEnergyGained(decimal finalAmount, Player player)
    {
        if (finalAmount > 0 && !CombatManager.Instance.IsEnding)
        {
            await ElectronHook.AfterEnergyGained(player.Creature.CombatState!, player, finalAmount);
        }
    }
}