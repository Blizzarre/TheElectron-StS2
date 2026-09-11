using MegaCrit.Sts2.Core.Entities.Powers;

namespace TheElectron.TheElectronCode.Powers;

public class AlephNullPower : TheElectronPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}