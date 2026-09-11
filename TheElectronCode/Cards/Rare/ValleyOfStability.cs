using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class ValleyOfStability : ElectronCard
{
    public ValleyOfStability() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithTip(ElectronHoverTip.Produce);
        WithTip(ElectronHoverTip.Stable);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<ValleyOfStabilityPower>(choiceContext, Owner.Creature,
            1, Owner.Creature, this);
    }
}