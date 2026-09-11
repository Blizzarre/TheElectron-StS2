using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class HiggsBoson : ElectronCard
{
    public HiggsBoson() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithVar("Amount", 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<HiggsBosonPower>(choiceContext, Owner.Creature,
            DynamicVars["Amount"].BaseValue, Owner.Creature, this);
    }
}