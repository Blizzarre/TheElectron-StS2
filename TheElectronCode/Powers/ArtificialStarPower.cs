using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheElectron.TheElectronCode.Hooks;

namespace TheElectron.TheElectronCode.Powers;

public class ArtificialStarPower : TheElectronPower, IAfterFaradOrHpDrained
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public async Task AfterFaradOrHpDrained(PlayerChoiceContext choiceContext, Player player, decimal amountDrained,
        CardModel? cardSource = null, CardPlay? cardPlay = null)
    {
        if (amountDrained > 0 && player == Owner.Player)
        {
            await PlayerCmd.GainEnergy(Amount, player);
            await CardPileCmd.Draw(choiceContext, Amount, player);
        }
    }
}