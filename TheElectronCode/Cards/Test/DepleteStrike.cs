using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TheElectron.TheElectronCode.Cards.Test;

public class DepleteStrike : ElectronDepleteCard
{
    public DepleteStrike() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(9, 3);
        WithBlock(3, 2);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override async Task OnPlayDepleteBefore(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }
}