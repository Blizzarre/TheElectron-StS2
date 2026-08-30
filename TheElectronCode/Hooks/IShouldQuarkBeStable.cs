using MegaCrit.Sts2.Core.Entities.Players;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Hooks;

public interface IShouldQuarkBeStable
{
    public bool ShouldQuarkBeStable(QuarkModel quark);
}

public interface IAfterMakingQuarkStable
{
    public Task AfterMakingQuarkStable();
}