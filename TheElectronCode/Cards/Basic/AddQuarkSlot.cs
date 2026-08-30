using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;

namespace TheElectron.TheElectronCode.Cards.Basic;


public class AddQuarkSlot : ElectronCard
{
    public AddQuarkSlot() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithVar("Slot", 2);
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await QuarkCmd.AddSlots(Owner, DynamicVars["Slot"].IntValue);
    }
}