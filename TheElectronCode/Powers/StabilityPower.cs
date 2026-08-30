using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Powers;

public class StabilityPower : TheElectronPower, IShouldQuarkBeStable, IAfterMakingQuarkStable
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public bool ShouldQuarkBeStable(QuarkModel quark)
    {
        return quark.Owner.Creature == Owner && !quark.IsStable;
    }

    public async Task AfterMakingQuarkStable()
    {
        Flash();
        await PowerCmd.Decrement(this);
    }
}