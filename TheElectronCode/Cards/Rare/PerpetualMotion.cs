using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class PerpetualMotion : ElectronCard
{
    public PerpetualMotion() : base(0, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip(ElectronHoverTip.Farad);
        WithVar("ExtraAmount", 0, 1);
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var amount = ResolveEnergyXValue() + DynamicVars["ExtraAmount"].BaseValue;

        if (amount > 0)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

            await PowerCmd.Apply<PerpetualMotionPower>(choiceContext, Owner.Creature, amount, Owner.Creature, this);
        }
    }
}