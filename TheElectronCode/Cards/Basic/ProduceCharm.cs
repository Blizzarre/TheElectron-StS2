using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Basic;

public class ProduceCharm : ElectronCard
{
    public ProduceCharm() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithQuarkTip<CharmQuark>();
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await QuarkCmd.Produce<CharmQuark>(choiceContext, Owner, this, play);
    }
}