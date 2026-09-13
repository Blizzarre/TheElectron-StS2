using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Common;

public class CathodeRay : ElectronDepleteCard
{
    public CathodeRay() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(ElectronKeywords.Drain);
        WithCalculatedDamage(14, 6,
            static (card, _) =>
                card is ElectronDepleteCard electronDepleteCard &&
                (electronDepleteCard.IsEnergyDepleted || electronDepleteCard.WouldDeplete)
                    ? 1
                    : 0,
            ValueProp.Move, 3, 3);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }
}