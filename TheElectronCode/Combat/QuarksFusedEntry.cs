using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Combat;

public class QuarksFusedEntry(
    IEnumerable<QuarkModel> quarks,
    Creature actor,
    int roundNumber,
    CombatSide currentSide,
    CombatHistory history,
    IEnumerable<Player> players) : CombatHistoryEntry(actor, roundNumber, currentSide, history, players)
{
    public IEnumerable<QuarkModel> Quarks { get; } = quarks;

    public override string Description => Actor.Player?.Character.Id.Entry + " fused " + string.Join(", ", Quarks.Select(q => q.Id.Entry));
}