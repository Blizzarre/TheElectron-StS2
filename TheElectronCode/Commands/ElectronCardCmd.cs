using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using TheElectron.TheElectronCode.Field;

namespace TheElectron.TheElectronCode.Commands;

public static class ElectronCardCmd
{
    public static void ApplySingleTurnDrain(CardModel card)
    {
        card.AssertMutable();
        ElectronField.SingleTurnDrain[card] = true;
        NCard.FindOnTable(card)?.UpdateVisuals(card.Pile?.Type ?? PileType.None, CardPreviewMode.Normal);
    }
}