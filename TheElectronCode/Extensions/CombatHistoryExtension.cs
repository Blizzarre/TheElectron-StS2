using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Players;
using TheElectron.TheElectronCode.Combat;

namespace TheElectron.TheElectronCode.Extensions;

public static class CombatHistoryExtension
{
    public static void FaradModified(this CombatHistory combatHistory, ICombatState combatState, int amount,
        Player player)
    {
        combatHistory.Add(combatState,
            new FaradModifiedEntry(amount, player, combatState.RoundNumber, combatState.CurrentSide, combatHistory,
                [player]));
    }
}