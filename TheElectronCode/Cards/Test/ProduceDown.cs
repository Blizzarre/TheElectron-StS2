using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Test;

public class ProduceDown : ElectronCard
{
    public ProduceDown() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithQuarkTip<DownQuark>();
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await QuarkCmd.Produce<DownQuark>(choiceContext, Owner, this, play);
    }
}