using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace TheElectron.TheElectronCode.Hooks;

public interface IAfterFaradLost
{
    public Task AfterFaradLost(PlayerChoiceContext choiceContext, Player player, decimal amountLost,
        CardModel? cardSource = null, CardPlay? cardPlay = null);
}