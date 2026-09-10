using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Ancient;

public class ArtificialStar : ElectronCard
{
    public ArtificialStar() : base(2, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithVar(new EnergyVar("Amount", 1));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<ArtificialStarPower>(choiceContext, Owner.Creature,
            DynamicVars["Amount"].BaseValue, Owner.Creature, this);
    }
}