using MegaCrit.Sts2.Core.HoverTips;
using TheElectron.TheElectronCode.Hooks;

namespace TheElectron.TheElectronCode.Models.Quarks;

public class CharmQuark : QuarkModel, IModifyQuarkValueMultAdd
{
    public override FuseStat Stat => FuseStat.None;

    public override bool ShowLabel => false;

    public override decimal Value => 0;

    public decimal ModifyQuarkValueMultAdd(QuarkModel quark, decimal mult)
    {
        return mult + 1;
    }
}