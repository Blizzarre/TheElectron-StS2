using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Extensions;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class VacuumFluctuation : ElectronCard
{
    public VacuumFluctuation() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCalculatedBlock(0, 5, static (card, _) =>
        {
            var queue = card.Owner.PlayerCombatState?.GetQuarkQueue();
            if (queue == null) return 0;
            return queue.Capacity - queue.Quarks.Count;
        }, ValueProp.Move, 0, 2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, play);
    }
}