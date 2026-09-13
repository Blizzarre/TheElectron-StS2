using MegaCrit.Sts2.Core.Entities.Powers;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.Models;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Powers;

public class PerpetualMotionPower : TheElectronPower, IModifyQuarkValueAdditive
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public decimal ModifyQuarkValueAdditive(QuarkModel quark, decimal value)
    {
        if (quark.Owner == Owner.Player && quark is UpQuark or DownQuark) return value + Amount;

        return value;
    }
}