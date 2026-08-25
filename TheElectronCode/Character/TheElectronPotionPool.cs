using BaseLib.Abstracts;
using TheElectron.TheElectronCode.Extensions;
using Godot;

namespace TheElectron.TheElectronCode.Character;

public class TheElectronPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => TheElectron.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}