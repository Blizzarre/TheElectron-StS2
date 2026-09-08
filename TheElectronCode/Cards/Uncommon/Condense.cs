using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Uncommon;


public class Condense : ElectronEmptyCard
{
    public Condense() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTip(ElectronHoverTip.Produce);
        WithQuarkTip<TopQuark>();
        WithVar(new QuarkCountVar(1));
        WithDamage(10, 4);
        WithEnergy(1);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        
        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
        {
            await QuarkCmd.Produce<TopQuark>(choiceContext, Owner, this, play);
        }
    }

    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature, DynamicVars.Energy.BaseValue,
            Owner.Creature, this);
    }
}