using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using TheElectron.TheElectronCode.Cards;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Extensions;

public static class CardModelExtensions
{
    // Should glow the Drain color
    public static bool ShouldGlowPurple(this CardModel card)
    {
        return card.Keywords.Contains(ElectronKeywords.Drain) && card.Owner.PlayerCombatState!.Energy <
            card.EnergyCost.GetWithModifiers(CostModifiers.All);
    }

    // Should glow the Empty color
    public static bool ShouldGlowBlack(this CardModel card)
    {
        return card is ElectronEmptyCard { WouldBeEmpty: true, HasEnoughEnergy: false};
    }
}