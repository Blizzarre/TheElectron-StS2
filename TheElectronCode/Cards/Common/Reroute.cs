using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Common;

public class Reroute : ElectronDepleteCard
{
    private const string DepleteFaradKey = "DepleteFarad";

    public Reroute() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeyword(ElectronKeywords.Drain);
        WithVars(new FaradVar(4).WithUpgrade(1), new FaradVar(DepleteFaradKey,1).WithUpgrade(1));
        WithTip(ElectronHoverTip.Farad);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad.BaseValue, this, play);
    }

    protected override async Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars[DepleteFaradKey].BaseValue, this, play);
    }
}