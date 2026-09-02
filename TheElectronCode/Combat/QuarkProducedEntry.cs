using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Players;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Combat;

public class QuarkProducedEntry(
    QuarkModel quark,
    int roundNumber,
    CombatSide currentSide,
    CombatHistory history,
    IEnumerable<Player> players) : CombatHistoryEntry(quark.Owner.Creature, roundNumber, currentSide, history, players)
{
    public QuarkModel Quark { get; } = quark;

    public override string Description => Actor.Player?.Character.Id.Entry + " produced " + Quark.Id.Entry;
}