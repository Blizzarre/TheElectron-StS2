using MegaCrit.Sts2.Core.Entities.Players;

namespace TheElectron.TheElectronCode.Hooks;

public interface IAfterEnergyChanged
{
    public void AfterEnergyChanged(Player player, int energy);
}