using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class ShortCircuit : ElectronCard
{
    private const string FaradCountKey = "FaradCount";

    public ShortCircuit() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain);
        WithTip(ElectronHoverTip.Farad);
        WithVar(FaradCountKey, 2);
        WithCalculatedVar("EnergyGain", 0, static (card, _) =>
        {
            var farad = card.Owner.PlayerCombatState?.GetFarad() ?? 0;
            // ReSharper disable once PossibleLossOfFraction
            return farad / card.DynamicVars[FaradCountKey].IntValue;
        });
        WithCostUpgradeBy(-1);
        WithEnergyTip();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var farad = Owner.PlayerCombatState?.GetFarad() ?? 0;
        var energyToGain = farad / DynamicVars[FaradCountKey].IntValue;
        
        await ElectronPlayerCmd.LoseFarad(choiceContext, Owner, farad, this, play);

        await PlayerCmd.GainEnergy(energyToGain, Owner);
    }
}