using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class DarkMatter : ElectronCard
{
    public DarkMatter() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithTip(ElectronHoverTip.Produce);
        WithQuarkTip<CharmQuark>();
        WithQuarkTip<StrangeQuark>();
        WithVar(new DynamicVar("StrangeQuark", 1));
        WithVar(new QuarkCountVar(2).WithUpgrade(1));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
            await QuarkCmd.Produce<CharmQuark>(choiceContext, Owner, this, play);
        for (var i = 0; i < DynamicVars["StrangeQuark"].IntValue; i++)
            await QuarkCmd.Produce<StrangeQuark>(choiceContext, Owner, this, play);
    }
}