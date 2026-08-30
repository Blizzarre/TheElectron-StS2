using MegaCrit.Sts2.Core.HoverTips;

namespace TheElectron.TheElectronCode.Models.Quarks;

public class BottomQuark : QuarkModel
{
    public override FuseStat Stat => FuseStat.Draw;

    public override bool ShowLabel => true;
    
    public override decimal Value => ModifyQuarkValue(1m);
}