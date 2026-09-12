using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class UniversalFlavors : ElectronCard
{
    public UniversalFlavors() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithTip(ElectronHoverTip.Produce);
        WithQuarkTip<UpQuark>();
        WithQuarkTip<DownQuark>();
        WithQuarkTip<BottomQuark>();
        WithQuarkTip<TopQuark>();
        WithQuarkTip<CharmQuark>();
        WithQuarkTip<StrangeQuark>();
        WithVar(new QuarkCountVar(1));
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
        {
            await QuarkCmd.Produce<UpQuark>(choiceContext, Owner, this, play);
            await QuarkCmd.Produce<DownQuark>(choiceContext, Owner, this, play);
            await QuarkCmd.Produce<TopQuark>(choiceContext, Owner, this, play);
            await QuarkCmd.Produce<BottomQuark>(choiceContext, Owner, this, play);
            await QuarkCmd.Produce<CharmQuark>(choiceContext, Owner, this, play);
            await QuarkCmd.Produce<StrangeQuark>(choiceContext, Owner, this, play);
        }
    }
}