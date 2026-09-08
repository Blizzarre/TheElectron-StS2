using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class Vortex : ElectronCard
{
    public Vortex() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(6, 2);
        WithCalculatedVar("SpinGain", 0, 1, static (card, _) =>
        {
            return card.Owner.PlayerCombatState?.GetQuarkQueue()?.Quarks.Select(q => q.Id.Entry).Distinct().Count() ?? 0;
        }, 0, 1);
        WithTip(typeof(SpinPower));
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .TargetingAllOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        
        await PowerCmd.Apply<SpinPower>(choiceContext, Owner.Creature,
            ((CalculatedVar)DynamicVars["SpinGain"]).Calculate(null), Owner.Creature, this);
    }
}