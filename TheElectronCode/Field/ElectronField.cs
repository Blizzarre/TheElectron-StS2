using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using static TheElectron.TheElectronCode.Extensions.PlayerCombatStateExtension;

namespace TheElectron.TheElectronCode.Field;

public class ElectronField
{
    public static readonly SpireField<CardModel, int> DrainExcessEnergy = new(_ => 0);
    
    public static readonly SpireField<PlayerCombatState, ElectronCombatState> ElectronCombatState = new(() => null);
}