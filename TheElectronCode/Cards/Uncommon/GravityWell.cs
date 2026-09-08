using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Uncommon;


public class GravityWell : ElectronEmptyCard
{
    public GravityWell() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(2, 1);
        WithTip(ElectronHoverTip.Produce);
        WithQuarkTip<BottomQuark>();
        WithVar(new QuarkCountVar(1));
        WithVar(new DynamicVar("ExtraQuark", 1));
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        
        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
        {
            await QuarkCmd.Produce<BottomQuark>(choiceContext, Owner, this, play);
        }
    }
    
    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        for (var i = 0; i < DynamicVars["ExtraQuark"].IntValue; i++)
        {
            await QuarkCmd.Produce<BottomQuark>(choiceContext, Owner, this, play);
        }
    }
}