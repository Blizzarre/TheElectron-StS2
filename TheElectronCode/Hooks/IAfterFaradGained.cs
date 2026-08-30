using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace TheElectron.TheElectronCode.Hooks;

public interface IAfterFaradGained
{
    public Task AfterFaradGained(PlayerChoiceContext choiceContext, Player player, decimal amount,
        CardModel? cardSource = null, CardPlay? cardPlay = null);
}