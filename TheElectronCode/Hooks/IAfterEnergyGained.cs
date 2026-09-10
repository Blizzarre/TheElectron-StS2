using MegaCrit.Sts2.Core.Entities.Players;

namespace TheElectron.TheElectronCode.Hooks;

public interface IAfterEnergyGained
{
    public Task AfterEnergyGained(Player player, decimal energy);
}