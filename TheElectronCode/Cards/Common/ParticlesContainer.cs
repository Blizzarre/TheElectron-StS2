using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Common;

public class ParticlesContainer : ElectronEmptyCard
{
    public ParticlesContainer() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPower<SpinPower>(4, 1);
        WithPower<StabilityPower>(1, 1);
        WithTip(ElectronHoverTip.Stable);
    }

    protected override async Task BeforeOnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<SpinPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<SpinPower>().BaseValue, Owner.Creature, this);
    }

    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<StabilityPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<StabilityPower>().BaseValue, Owner.Creature, this);
    }
}