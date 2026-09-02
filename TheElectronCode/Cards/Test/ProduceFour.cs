using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Test;

public class ProduceFour : ElectronCard
{
    public ProduceFour() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithQuarkTip<UpQuark>();
        WithQuarkTip<DownQuark>();
        WithQuarkTip<BottomQuark>();
        WithQuarkTip<TopQuark>();
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await QuarkCmd.Produce<UpQuark>(choiceContext, Owner, this, play);
        await QuarkCmd.Produce<DownQuark>(choiceContext, Owner, this, play);
        await QuarkCmd.Produce<BottomQuark>(choiceContext, Owner, this, play);
        await QuarkCmd.Produce<TopQuark>(choiceContext, Owner, this, play);
    }
}