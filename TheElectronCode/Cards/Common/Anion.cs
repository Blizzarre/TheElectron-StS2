using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;

namespace TheElectron.TheElectronCode.Cards.Common;

public class Anion : ElectronEmptyCard
{
    private const string EmptyFaradKey = "EmptyFarad";
    
    public Anion() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithVar(new FaradVar(3).WithUpgrade(1));
        WithVar(new FaradVar(EmptyFaradKey, 1).WithUpgrade(1));
        WithTip(ElectronHoverTip.Farad);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad.BaseValue, this, play);
    }

    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars[EmptyFaradKey].BaseValue, this, play);
    }
}