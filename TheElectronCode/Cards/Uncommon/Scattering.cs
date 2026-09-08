using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class Scattering : ElectronCard
{
    public Scattering() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(4);
        WithVar("Hits", 3, 1);
        WithPower<QuantumLinkPower>(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var results = (await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .WithHitCount(DynamicVars["Hits"].IntValue)
            .TargetingRandomOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext)).Results;
        
        foreach (var result in results.SelectMany(r => r))
        {
            await PowerCmd.Apply<QuantumLinkPower>(choiceContext, result.Receiver,
                DynamicVars.Power<QuantumLinkPower>().BaseValue, Owner.Creature, this);
        }
    }
}