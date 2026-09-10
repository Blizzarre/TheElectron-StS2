using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class TailWhip : ElectronDepleteCard
{
    public TailWhip() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(7, 1);
        WithCalculatedVar("CalculatedHits", 2, 1, static (card, _) => ((TailWhip)card).WouldDeplete ? 1 : 0, 0, 1);
    }

    protected override async Task OnPlayWrapper(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var hitCount = DynamicVars["CalculatedHitsBase"].IntValue;
        if (IsEnergyDepleted) hitCount += DynamicVars["CalculatedHitsExtra"].IntValue;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .WithHitCount(hitCount)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }
}