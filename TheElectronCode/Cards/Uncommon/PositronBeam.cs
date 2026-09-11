using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class PositronBeam : ElectronEmptyCard
{
    public PositronBeam() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(9, 1);
        WithPower<QuantumLinkPower>(1, 1);
        WithVar(new FaradVar(2).WithUpgrade(1));
        WithVar("FaradLoss", 1);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .TargetingAllOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        
        await PowerCmd.Apply<QuantumLinkPower>(choiceContext, CombatState!.HittableEnemies,
            DynamicVars.Power<QuantumLinkPower>().BaseValue, Owner.Creature, this);
    }

    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad.BaseValue, this, play);
        await Cmd.CustomScaledWait(0.2f, 0.4f);
        await ElectronPlayerCmd.LoseFarad(choiceContext, Owner, DynamicVars["FaradLoss"].BaseValue, this, play);
    }
}