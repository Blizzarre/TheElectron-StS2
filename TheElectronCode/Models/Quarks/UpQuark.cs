namespace TheElectron.TheElectronCode.Models.Quarks;

public class UpQuark : QuarkModel
{
    public override FuseStat Stat => FuseStat.Damage;

    public override bool ShowLabel => true;

    public override decimal Value => ModifyQuarkValue(5m);
}