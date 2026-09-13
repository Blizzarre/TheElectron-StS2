using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using TheElectron.TheElectronCode.Cards;
using TheElectron.TheElectronCode.Field;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Models;

public class ElectronSingletonModel() : CustomSingletonModel(HookType.Combat)
{
    // Cover case for when resource weren't spent for autoplaying cards
    // Drain doesn't need handling as it's only relevant when spending resource.
    public override Task BeforeCardAutoPlayed(CardModel card, Creature? target, AutoPlayType type)
    {
        var energy = card.Owner.PlayerCombatState?.Energy ?? 0;
        
        if (card is ElectronEmptyCard emptyCard)
        {
            emptyCard.HasPaidEnergyCost = true;
            
            if (energy == 0)
            {
                emptyCard.IsPlayedAsEmpty = true;
            }
            
            // Force refresh text
            NCard.FindOnTable(card)?.UpdateVisuals(card.Pile?.Type ?? PileType.Play, CardPreviewMode.Normal);
        }
        else if (card is ElectronDepleteCard depleteCard && energy == 0)
        {
            depleteCard.IsEnergyDepleted = true;
            
            // Force refresh text
            NCard.FindOnTable(card)?.UpdateVisuals(card.Pile?.Type ?? PileType.Play, CardPreviewMode.Normal);
        }
        
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay is {IsLastInSeries: true})
            ElectronField.DrainExcessEnergy[cardPlay.Card] = 0;

        if (cardPlay is { Card: ElectronEmptyCard emptyCard, IsLastInSeries: true })
        {
            emptyCard.IsPlayedAsEmpty = false;
            emptyCard.HasPaidEnergyCost = false;
        }

        if (cardPlay is { Card: ElectronDepleteCard depleteCard, IsLastInSeries: true })
            depleteCard.IsEnergyDepleted = false;

        return Task.CompletedTask;
    }
    
    public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
    {
        return ElectronField.SingleTurnDrain[card] && keywords.Add(ElectronKeywords.Drain);
    }
}