using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class DemonCore : ElectronCard
{
    public DemonCore() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<SpinPower>(10, 4);
        WithVar("SlotLoss", 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<SpinPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<SpinPower>().BaseValue, Owner.Creature, this);

        await QuarkCmd.RemoveSlots(choiceContext, Owner, DynamicVars["SlotLoss"].IntValue);
    }
}