using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class Overvolt : ElectronCard
{
    public Overvolt() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithCalculatedDamage(6, static (card, _) => card.Owner.PlayerCombatState?.Electron()?.Farad ?? 0,
            ValueProp.Move, 3);
        WithTip(ElectronHoverTip.Farad);
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var xValue = ResolveEnergyXValue();
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this, play)
            .WithHitCount(xValue)
            .TargetingAllOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        await ElectronPlayerCmd.LoseFarad(choiceContext, Owner, xValue, this, play);
    }
}