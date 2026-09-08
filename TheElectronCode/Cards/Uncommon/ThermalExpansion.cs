using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class ThermalExpansion : ElectronEmptyCard
{
    public ThermalExpansion() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar("Amount", 2, 1);
        WithVar("EmptyAmount", 1);
    }

    protected override async Task BeforeOnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await QuarkCmd.AddSlots(Owner, DynamicVars["Amount"].IntValue);
    }

    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await QuarkCmd.AddSlots(Owner, DynamicVars["EmptyAmount"].IntValue);
    }
}