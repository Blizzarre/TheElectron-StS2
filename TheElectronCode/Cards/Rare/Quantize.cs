using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using TheElectron.TheElectronCode.Powers;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class Quantize : ElectronDepleteCard
{
    public Quantize() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithKeywords(CardKeyword.Ethereal, ElectronKeywords.Drain);
        WithPower<QuantumLinkPower>(6, 1);
        WithPower<VulnerablePower>(2, 1);
        WithPower<WeakPower>(2, 1);
    }

    protected override async Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<QuantumLinkPower>(choiceContext, CombatState!.HittableEnemies,
            DynamicVars.Power<QuantumLinkPower>().BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, CombatState!.HittableEnemies,
            DynamicVars.Power<VulnerablePower>().BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<WeakPower>(choiceContext, CombatState!.HittableEnemies,
            DynamicVars.Power<WeakPower>().BaseValue, Owner.Creature, this);
    }
}