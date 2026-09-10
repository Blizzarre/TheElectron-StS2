using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheElectron.TheElectronCode.Hooks;

namespace TheElectron.TheElectronCode.Powers;

public class PowerBankPower : TheElectronPower, IAfterEnergyLost
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _hasGainedEnergy;

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (Owner.Player == card.Owner && Owner.Player?.PlayerCombatState?.Energy == 0 && !_hasGainedEnergy)
        {
            Flash();
            await PlayerCmd.GainEnergy(Amount, Owner.Player);
            _hasGainedEnergy = true;
        }
    }

    // Safeguard for when LoseEnergy PlayerCmd was used.
    public async Task AfterEnergyLost(Player player, decimal energy)
    {
        if (Owner.Player == player && Owner.Player?.PlayerCombatState?.Energy == 0 && !_hasGainedEnergy)
        {
            Flash();
            await PlayerCmd.GainEnergy(Amount, Owner.Player);
            _hasGainedEnergy = true;
        }
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == Owner.Side && participants.Contains(Owner)) _hasGainedEnergy = false;

        return Task.CompletedTask;
    }
}