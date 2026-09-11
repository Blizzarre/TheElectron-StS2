using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Ancient;

public class Entangle : ElectronDepleteCard
{
    public Entangle() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithPower<QuantumLinkPower>(9, 3);
        WithVar(new FaradVar(3).WithUpgrade(1));
    }

    protected override async Task BeforeOnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await PowerCmd.Apply<QuantumLinkPower>(choiceContext, play.Target,
            DynamicVars.Power<QuantumLinkPower>().BaseValue, Owner.Creature, this);
    }

    protected override async Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad.BaseValue, this, play);
    }
}