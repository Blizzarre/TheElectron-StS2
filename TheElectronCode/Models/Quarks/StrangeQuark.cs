using MegaCrit.Sts2.Core.HoverTips;
using TheElectron.TheElectronCode.Hooks;

namespace TheElectron.TheElectronCode.Models.Quarks;

public class StrangeQuark : QuarkModel
{
    public override FuseStat Stat => FuseStat.SelfDamage;

    public override bool ShowLabel => true;

    public override decimal Value => ModifyQuarkValue(2);

    // public decimal ModifyQuarkValueMult(QuarkModel quark, decimal mult)
    // {
    //     if (quark != this) return 0;
    //
    //     return mult;
    // }
}