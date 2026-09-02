using MegaCrit.Sts2.Core.HoverTips;

namespace TheElectron.TheElectronCode.Models.Quarks;

public class DownQuark : QuarkModel
{
    public override FuseStat Stat => FuseStat.Block;

    public override bool ShowLabel => true;

    public override decimal Value => ModifyQuarkValue(4m);

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];
}