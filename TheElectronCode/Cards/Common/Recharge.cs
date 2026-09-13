using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;

namespace TheElectron.TheElectronCode.Cards.Common;

public class Recharge : ElectronCard
{
    public Recharge() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(8, 1);
        WithVar(new FaradVar(1).WithUpgrade(1));
        WithTip(ElectronHoverTip.Farad);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play) {
        await CommonActions.CardBlock(this, play);
        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad.BaseValue, this, play);
    }
}