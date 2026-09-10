using MegaCrit.Sts2.Core.Entities.Players;

namespace TheElectron.TheElectronCode.Hooks;

public interface IAfterEnergyLost
{
    public Task AfterEnergyLost(Player player, decimal energy);
}