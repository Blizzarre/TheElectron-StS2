using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class FluxCapacitor : ElectronCard
{
    public FluxCapacitor() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(ElectronHoverTip.Farad);
        WithVar(new FaradVar(0).WithUpgrade(2));
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var faradGain = ResolveEnergyXValue() * 2;
        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad.BaseValue + faradGain, this, play);
    }
}