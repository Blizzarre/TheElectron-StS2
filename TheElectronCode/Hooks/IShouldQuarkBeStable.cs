using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Hooks;

public interface IShouldQuarkBeStable
{
    public bool ShouldQuarkBeStable(QuarkModel quark);
    public Task AfterMakingQuarkStable();
}