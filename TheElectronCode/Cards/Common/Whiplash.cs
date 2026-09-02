using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TheElectron.TheElectronCode.Cards.Common;

public class Whiplash : ElectronEmptyCard
{
    public Whiplash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithDamage(8, 3);
        WithBlock(3, 2);
    }

    public override TargetType TargetType => HasEnoughEnergy ? TargetType.AllEnemies : TargetType.Self;

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .TargetingAllOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }
}