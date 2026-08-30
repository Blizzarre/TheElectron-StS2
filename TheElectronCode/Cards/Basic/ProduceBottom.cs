using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Models.Quarks;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Basic;

public class ProduceBottom : ElectronCard
{
    public ProduceBottom() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithQuarkTip<BottomQuark>();
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await QuarkCmd.Produce<BottomQuark>(choiceContext, Owner, this, play);
    }
}