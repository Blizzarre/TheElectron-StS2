using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class Accumulator : ElectronDepleteCard
{
    private const string DamageIncreaseKey = "DamageIncrease";

    private decimal ExtraDamage
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public Accumulator() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 2);
        WithVar(DamageIncreaseKey, 7, 2);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var increment = DynamicVars[DamageIncreaseKey].BaseValue;
        DynamicVars.Damage.BaseValue += increment;
        ExtraDamage += increment;
        return Task.CompletedTask;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        var damage = DynamicVars.Damage;
        damage.BaseValue += ExtraDamage;
    }
}