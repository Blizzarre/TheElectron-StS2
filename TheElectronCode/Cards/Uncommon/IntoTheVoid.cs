using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class IntoTheVoid : ElectronEmptyCard
{
    private const string HitsIncreaseKey = "HitsIncrease";
    private const string HitsKey = "Hits";

    private decimal ExtraHits
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public IntoTheVoid() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(3, 1);
        WithVar(HitsKey, 4);
        WithVar(HitsIncreaseKey, 2);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .TargetingRandomOpponents(CombatState!)
            .WithHitCount(DynamicVars[HitsKey].IntValue)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var increment = DynamicVars[HitsIncreaseKey].BaseValue;
        DynamicVars[HitsKey].BaseValue += increment;
        ExtraHits += increment;
        return Task.CompletedTask;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars[HitsKey].BaseValue += ExtraHits;
    }
}