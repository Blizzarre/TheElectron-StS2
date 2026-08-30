using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace TheElectron.TheElectronCode.Hooks;

public interface IAfterFaradOrHpDrained
{
    public Task AfterFaradOrHpDrained(PlayerChoiceContext choiceContext, Player player, decimal amountDrained,
        CardModel? cardSource = null, CardPlay? cardPlay = null);
}