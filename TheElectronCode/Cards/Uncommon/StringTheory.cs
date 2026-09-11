using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class StringTheory : ElectronCard
{
    public StringTheory() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(typeof(QuantumLinkPower));
        WithVar("Amount", 1, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<StringTheoryPower>(choiceContext, Owner.Creature,
            DynamicVars["Amount"].BaseValue, Owner.Creature, this);
    }
}