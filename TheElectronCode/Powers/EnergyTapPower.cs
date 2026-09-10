using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Hooks;

namespace TheElectron.TheElectronCode.Powers;

public class EnergyTapPower : TheElectronPower, IAfterEnergyGained
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner.Player == player)
        {
            Flash();
            await ElectronPlayerCmd.GainFarad(choiceContext, player, Amount);
        }
    }

    public async Task AfterEnergyGained(Player player, decimal energy)
    {
        // Should only trigger during your turn
        if (Owner.Player == player && energy > 0 && CombatState.CurrentSide == Owner.Side)
        {
            // Should probably be fine...?
            Flash();
            await ElectronPlayerCmd.GainFarad(new BlockingPlayerChoiceContext(), player, Amount);
        }
    }
}