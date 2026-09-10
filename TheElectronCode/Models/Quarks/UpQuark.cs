using MegaCrit.Sts2.Core.Localization;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Models.Quarks;

public class UpQuark : QuarkModel
{
    public override FuseStat Stat => FuseStat.Damage;

    public override bool ShowLabel => true;

    public override decimal Value => ModifyQuarkValue(5m);

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("IsAoe", Owner.Creature.HasPower<FissionCellPower>());
    }
}