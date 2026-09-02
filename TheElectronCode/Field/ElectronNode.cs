using BaseLib.Utils;
using MegaCrit.Sts2.Core.Assets;
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
}