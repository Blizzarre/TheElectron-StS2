using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class ElectricArc : ElectronDepleteCard
{
    public ElectricArc() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(13, 2);
        WithPower<VulnerablePower>(2, 1);
    }

    protected override async Task OnPlayWrapper(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .TargetingAllOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override async Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<VulnerablePower>(choiceContext, CombatState!.HittableEnemies,
            DynamicVars.Power<VulnerablePower>().BaseValue, Owner.Creature, this);
    }
}