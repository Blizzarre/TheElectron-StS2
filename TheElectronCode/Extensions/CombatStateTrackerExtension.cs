#region

using MegaCrit.Sts2.Core.Combat;
using static TheElectron.TheElectronCode.Extensions.PlayerCombatStateExtension;

#endregion

namespace TheElectron.TheElectronCode.Extensions;

public static class CombatStateTrackerExtension
{
    private static void OnFaradChanged(this CombatStateTracker tracker, int _, int __)
    {
        tracker.NotifyCombatStateChanged("OnPlayerCombatStateValueChanged");
    }

    public static void SubscribeFarad(this CombatStateTracker tracker, ElectronCombatState combatState)
    {
        combatState.FaradChanged += tracker.OnFaradChanged;
    }

    public static void UnsubscribeFarad(this CombatStateTracker tracker, ElectronCombatState combatState)
    {
        combatState.FaradChanged -= tracker.OnFaradChanged;
    }
}