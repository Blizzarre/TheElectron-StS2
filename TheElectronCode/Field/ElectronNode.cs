using BaseLib.Utils;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using TheElectron.TheElectronCode.Nodes;
using TheElectron.TheElectronCode.Nodes.Quarks;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Field;

public class ElectronNode
{
    public static readonly AddedNode<NCombatUi, NFaradCounter> NFaradCounter = new(ui =>
    {
        var faradCounter = PreloadManager.Cache.GetScene(ElectronResource.NFaradCounterPath)
            .Instantiate<NFaradCounter>();
        ui.AddChild(faradCounter);
        return faradCounter;
    });


    public static readonly SpireField<NCreature, NQuarkManager> NQuarkManager = new(() => null);

    public static readonly AddedNode<NCard, NElectronCardIndicator> NElectronCardIndicator = new(card =>
    {
        var indicator = PreloadManager.Cache.GetScene(ElectronResource.NElectronCardIndicatorPath)
            .Instantiate<NElectronCardIndicator>()
            .WithData(card);
        var cardContainer = card.GetChild(0)!;
        cardContainer.AddChild(indicator);
        cardContainer.MoveChild(indicator, cardContainer.GetNode("%EnergyIcon").GetIndex());
        return indicator;
    });
}