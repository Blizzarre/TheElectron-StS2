using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using TheElectron.TheElectronCode.Character;
using TheElectron.TheElectronCode.Extensions;

namespace TheElectron.TheElectronCode.Potions;

[Pool(typeof(TheElectronPotionPool))]
public abstract class TheElectronPotion : CustomPotionModel
{
    public override string? CustomPackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();

    public override string? CustomPackedOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionOutlineImagePath();
}