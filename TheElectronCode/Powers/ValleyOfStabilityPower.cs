using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Powers;

public class ValleyOfStabilityPower : TheElectronPower, IShouldQuarkBeStable
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public bool ShouldQuarkBeStable(QuarkModel quark)
    {
        return quark.Owner.Creature == Owner && !quark.IsStable;
    }

    public Task AfterMakingQuarkStable()
    {
        Flash();
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player)
        {
            Flash();
            await QuarkCmd.Fuse(choiceContext, player);
        }
    }
}