using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Hooks;

public interface IAfterQuarksFused
{
    public Task AfterQuarksFused(PlayerChoiceContext choiceContext, Player player, IEnumerable<QuarkModel> fusedQuarks);
}