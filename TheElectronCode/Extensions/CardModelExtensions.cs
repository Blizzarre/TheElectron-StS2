using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using TheElectron.TheElectronCode.Cards;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Extensions;

public static class CardModelExtensions
{
    extension(CardModel card)
    {
        // Should glow the Drain color
        public bool ShouldGlowPurple()
        {
            return card.Keywords.Contains(ElectronKeywords.Drain) && card.Owner.PlayerCombatState!.Energy <
                card.EnergyCost.GetWithModifiers(CostModifiers.All);
        }

        // Should glow the Empty color
        public bool ShouldGlowBlack()
        {
            return card is ElectronEmptyCard { WouldBeEmpty: true, HasEnoughEnergy: false};
        }

        public bool CanDrain()
        {
            return !card.Keywords.Intersect([CardKeyword.Unplayable, ElectronKeywords.Drain]).Any();
        }
    }


}