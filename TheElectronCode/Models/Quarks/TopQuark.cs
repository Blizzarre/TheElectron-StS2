using MegaCrit.Sts2.Core.HoverTips;

namespace TheElectron.TheElectronCode.Models.Quarks;

public class TopQuark : QuarkModel
{
    public override FuseStat Stat => FuseStat.Energy;

    public override bool ShowLabel => true;
    
    public override decimal Value => ModifyQuarkValue(1m);

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Energy)
    ];
}