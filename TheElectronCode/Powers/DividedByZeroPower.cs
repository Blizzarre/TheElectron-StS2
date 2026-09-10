using MegaCrit.Sts2.Core.Entities.Powers;

namespace TheElectron.TheElectronCode.Powers;

public class DividedByZeroPower : TheElectronPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}