using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Players;

namespace TheElectron.TheElectronCode.Combat;

public class FaradModifiedEntry(
    int amount,
    Player player,
    int roundNumber,
    CombatSide currentSide,
    CombatHistory history,
    IEnumerable<Player> players) : CombatHistoryEntry(player.Creature, roundNumber, currentSide, history, players)
{
    public int Amount { get; } = amount;

    public Player Player { get; } = player;

    public override string Description =>
        $"{Actor.Player?.Character.Id.Entry} {(Amount < 0 ? "lost" : "gained")} {Amount} farad";
}