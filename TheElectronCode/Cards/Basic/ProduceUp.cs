using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Basic;

public class ProduceUp : ElectronCard
{
    public ProduceUp() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithQuarkTip<UpQuark>();
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await QuarkCmd.Produce<UpQuark>(choiceContext, Owner, this, play);
    }
}