using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Extensions;

namespace TheElectron.TheElectronCode.Cards.Common;

public class DoubleSlit : ElectronCard
{
    public DoubleSlit() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithCalculatedDamage(4, (card, _) => card.Owner.PlayerCombatState?.Electron()?.QuarkQueue.Quarks.Count ?? 0,
            ValueProp.Move, 1, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this, play)
            .Targeting(play.Target)
            .WithHitCount(2)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}