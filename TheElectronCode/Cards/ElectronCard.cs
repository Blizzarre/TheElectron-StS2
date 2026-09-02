using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using TheElectron.TheElectronCode.Character;
using TheElectron.TheElectronCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Cards;

[Pool(typeof(TheElectronCardPool))]
public abstract class ElectronCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    ConstructedCardModel(cost, type, rarity, target)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();
        }
    }
    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();
        }
    }

    public override string BetaPortraitPath
    {
        get
        {
            var path = $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : "beta/card.png".CardImagePath();
        }
    }

    protected void WithTip(ElectronHoverTip electronTip)
    {
        switch (electronTip)
        {
            case ElectronHoverTip.Empty or ElectronHoverTip.Deplete:
                WithTip(new TooltipSource(card => ElectronHoverTipFactory.Static(electronTip,
                    loc => loc.Add("energyPrefix", EnergyIconHelper.GetPrefix(card)))));
                break;
            default:
                WithTip(new TooltipSource(_ => ElectronHoverTipFactory.Static(electronTip)));
                break;
        }
    }

    protected void WithQuarkTip<T>() where T : QuarkModel
    {
        WithTip(new TooltipSource(_ => ElectronHoverTipFactory.FromQuark<T>()));
    }
}