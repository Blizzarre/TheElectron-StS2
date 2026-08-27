using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheElectron.TheElectronCode.Cards.Basic;

public class EmptyStrike : ElectronEmptyCard
{
    public EmptyStrike() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(15, 5);
        WithVar(new DamageVar("EmptyDamage", 5, ValueProp.Move).WithUpgrade(2));
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override async Task OnPlayEmpty(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars["EmptyDamage"].BaseValue).FromCard(this, play).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }
}