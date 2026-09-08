using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class PlasmaTurbine : ElectronCard
{
    public PlasmaTurbine() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar(new FaradVar(1).WithUpgrade(1));
        WithTip(typeof(SpinPower));
        WithCalculatedVar("SpinGain", 0, static (card, _) =>
        {
            var farad = card.Owner.PlayerCombatState?.GetFarad() ?? 0;
            var faradGained = ElectronHook.ModifyFaradGain(card.CombatState!, card.Owner,
                card.DynamicVars.Farad.BaseValue,
                ValueProp.Move, card, out var _);
            return farad + faradGained;
        });
        WithEnergyTip();
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad.BaseValue, this, play);
        
        var farad = Owner.PlayerCombatState?.GetFarad() ?? 0;
        await PowerCmd.Apply<SpinPower>(choiceContext, Owner.Creature,
            farad, Owner.Creature, this);
    }
}