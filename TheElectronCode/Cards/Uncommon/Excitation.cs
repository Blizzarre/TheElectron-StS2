using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Uncommon;


public class Excitation : ElectronEmptyCard
{
    public Excitation() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(ElectronHoverTip.Produce);
        WithQuarkTip<TopQuark>();
        WithVar(new QuarkCountVar(2).WithUpgrade(1));
        WithVar(new FaradVar(2).WithUpgrade(2));
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        
        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
        {
            await QuarkCmd.Produce<TopQuark>(choiceContext, Owner, this, play);
        }
    }
    
    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad.BaseValue, this, play);
    }
}