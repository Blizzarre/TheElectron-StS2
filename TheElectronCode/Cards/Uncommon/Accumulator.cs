using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class Accumulator : ElectronDepleteCard
{
    private const string IncreaseKey = "Increase";

    private decimal ExtraHit
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
        WithDamage(9, 3);
        WithVar(new RepeatVar(1));
        WithVar(IncreaseKey, 1);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var increment = DynamicVars[IncreaseKey].BaseValue;
        DynamicVars.Repeat.BaseValue += increment;
        ExtraHit += increment;
        return Task.CompletedTask;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Repeat.BaseValue += ExtraHit;
    }
}