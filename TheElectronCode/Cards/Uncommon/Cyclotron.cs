using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class Cyclotron : ElectronCard
{
    public Cyclotron() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar("Amount", 1);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithTip(ElectronHoverTip.Fuse);
        WithTip(typeof(SpinPower));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<CyclotronPower>(choiceContext, Owner.Creature,
            DynamicVars["Amount"].BaseValue, Owner.Creature, this);
    }
}