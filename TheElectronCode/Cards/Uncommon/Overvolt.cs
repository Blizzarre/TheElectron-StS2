using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class Overvolt : ElectronCard
{
    public Overvolt() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithVar("ExtraHits", 0, 1);
        WithCalculatedDamage(3, static (card, _) => card.Owner.PlayerCombatState?.Electron()?.Farad ?? 0);
        WithVar(new FaradVar("FaradLoss", 2));
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var hits = ResolveEnergyXValue() + DynamicVars["ExtraHits"].IntValue;
        
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this, play)
            .WithHitCount(hits)
            .TargetingAllOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        await ElectronPlayerCmd.LoseFarad(choiceContext, Owner, DynamicVars["FaradLoss"].BaseValue, this, play);
    }
}