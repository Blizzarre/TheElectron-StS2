using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Test;

public class InfiniteStrike : ElectronCard
{
    public InfiniteStrike() : base(99, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(999, 444);
        WithKeyword(ElectronKeywords.Drain);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }
}