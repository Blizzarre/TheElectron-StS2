using MegaCrit.Sts2.Core.HoverTips;
using TheElectron.TheElectronCode.Hooks;

namespace TheElectron.TheElectronCode.Models.Quarks;

public class StrangeQuark : QuarkModel, IModifyQuarkValueMult
{
    public override FuseStat Stat => FuseStat.None;

    public override bool ShowLabel => false;

    public override decimal Value => 0;

    public decimal ModifyQuarkValueMult(QuarkModel quark, decimal mult)
    {
        if (quark != this) return 0;

        return mult;
    }
}