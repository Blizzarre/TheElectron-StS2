using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;

namespace TheElectron.TheElectronCode.Field;

public class ElectronField
{
    public static readonly SpireField<CardModel, int> DrainExcessEnergy = new(_ => 0);
}