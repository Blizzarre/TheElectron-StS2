using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class MiracleMatter : ElectronDepleteCard
{
    public MiracleMatter() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(ElectronHoverTip.Produce);
        WithQuarkTip<CharmQuark>();
        WithVar(new QuarkCountVar(1));
        WithTips(c => c.IsUpgraded ? [ElectronHoverTipFactory.Static(ElectronHoverTip.Stable)] : []);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
            await QuarkCmd.Produce<CharmQuark>(choiceContext, Owner, this, play, IsUpgraded);
    }

    protected override async Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await QuarkCmd.Fuse(choiceContext, Owner, this, play);
    }
}