using System.Reflection.Emit;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using TheElectron.TheElectronCode.Cards;
using TheElectron.TheElectronCode.Entities;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Field;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Patches;

[HarmonyPatch(typeof(PlayerCombatState), MethodType.Constructor)]
[HarmonyPatch([typeof(Player)])]
internal class PlayerCombatStateConstructorPatch
{
    [HarmonyPostfix]
    private static void Postfix(Player player, PlayerCombatState __instance)
    {
        var quarkQueue = new QuarkQueue(player);
        quarkQueue.Clear();
        if (player.Character is Character.TheElectron) quarkQueue.AddCapacity(QuarkQueue.DefaultCapacity);

        var electronCombatState = new PlayerCombatStateExtension.ElectronCombatState(__instance, quarkQueue);

        ElectronField.ElectronCombatState[__instance] = electronCombatState;

        CombatManager.Instance.StateTracker.SubscribeFarad(electronCombatState);
    }
}

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.AfterCombatEnd))]
internal class PlayerCombatStateAfterCombatEndPatch
{
    [HarmonyPostfix]
    public static void Postfix(PlayerCombatState __instance)
    {
        var electronCombatState = __instance.Electron();
        if (electronCombatState != null) CombatManager.Instance.StateTracker.UnsubscribeFarad(electronCombatState);
    }
}

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
internal class PlayerCombatStateHasEnoughResourcesForPatch
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new InstructionPatcher(instructions).Match(new InstructionMatcher()
            .ldarg_2()
            .opcode(OpCodes.Ldind_I4)
            .opcode(OpCodes.Ldc_I4_0)
            .opcode(OpCodes.Ceq)
            .opcode(OpCodes.Ret)
        ).Step(-4).Insert([
            CodeInstruction.LoadArgument(0),
            CodeInstruction.LoadArgument(1),
            CodeInstruction.LoadArgument(2),
            CodeInstruction.Call(typeof(PlayerCombatStateHasEnoughResourcesForPatch), nameof(IgnoreEnergyCostLimit))
        ]);
    }

    // Remove energy cost restriction if the card has the Drain keyword or is Empty while having 0 Energy
    private static void IgnoreEnergyCostLimit(PlayerCombatState instance, CardModel card, ref UnplayableReason reason)
    {
        if (!reason.HasFlag(UnplayableReason.EnergyCostTooHigh)) return;
        if (card.Keywords.Contains(ElectronKeywords.Drain) ||
            (card is ElectronEmptyCard && card.Owner.PlayerCombatState!.Energy == 0))
        {
            reason ^= UnplayableReason.EnergyCostTooHigh;
        }
    }
}