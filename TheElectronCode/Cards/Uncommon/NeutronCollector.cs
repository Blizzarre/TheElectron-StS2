using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class NeutronCollector : ElectronCard
{
    public NeutronCollector() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar("Amount", 1);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        await PowerCmd.Apply<NeutronCollectorPower>(choiceContext, Owner.Creature,
            DynamicVars["Amount"].BaseValue, Owner.Creature, this);
    }
}