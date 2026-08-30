using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using TheElectron.TheElectronCode.Entities;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Utils;

public class ElectronSubscriber
{
    public static void Subscribe()
    {
        ModHelper.SubscribeForCombatStateHooks(TheElectronMod.ModId, CollectRuneModels);
    }

    private static IEnumerable<QuarkModel> CollectRuneModels(CombatState combatState)
    {
        return combatState.Players
            .Select(p => p.PlayerCombatState?.GetQuarkQueue())
            .OfType<QuarkQueue>()
            .SelectMany(rq => rq.Quarks)
            .Where(r => r is { HasBeenRemovedFromState: false, Owner.IsActiveForHooks: true });
    }
}