using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace TheElectron.TheElectronCode.Powers;

public class QuantumFormPower : TheElectronPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    private class Data
    {
        public CardModel? OwnerCard;
    }

    
    protected override object InitInternalData()
    {
        return new Data();
    }

    public void SetOwnerCard(CardModel card)
    {
        GetInternalData<Data>().OwnerCard = card;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        
        if (!CombatManager.Instance.IsInProgress || cardPlay.Card.Owner != Owner.Player)
            return Task.CompletedTask;

        // Actual amount to decrement by
        var effectiveAmount = Amount;
        
        var internalData = GetInternalData<Data>();
        if (card == internalData.OwnerCard)
        {
            // Minus 1 for self play
            effectiveAmount -= 1;
            internalData.OwnerCard = null;
            if (effectiveAmount <= 0)
            {
                return Task.CompletedTask;
            }
        }
        
        var rng = Owner.Player.RunState.Rng.CombatCardSelection;
        var cards = PileType.Hand.GetPile(Owner.Player).Cards;
        
        // Priority 1: cards that are currently cost this power stack or more. 
        // eg. 2 stack will prioritize cards that cost 2 or more.
        var cardsAllMod = cards.Select(c => (card: c, cost: c.EnergyCost.GetWithModifiers(CostModifiers.All))).ToList();
        var cardsLocMod = cards.Select(c => (card: c, cost: c.EnergyCost.GetWithModifiers(CostModifiers.Local))).ToList();
        
        var selectedCard = rng.NextItem(cardsAllMod.Where(c => c.cost >= effectiveAmount)).card;
        if (selectedCard == null)
        {
            // Priority 2: similar to (1) but check with local modifiers instead.
            selectedCard = rng.NextItem(cardsLocMod.Where(c => c.cost >= effectiveAmount)).card;
        }
        if (selectedCard == null)
        {
            // Priority 3: Now check for any cards that cost more than 0 with all mod
            selectedCard = rng.NextItem(cardsAllMod.Where(c => c.cost > 0)).card;
        }
        if (selectedCard == null)
        {
            // Priority 4: Now check for any cards that cost more than 0 with local mod
            selectedCard = rng.NextItem(cardsLocMod.Where(c => c.cost > 0)).card;
        }
        if (selectedCard == null)
        {
            // Priority 5: Any card that cost more than 0
            selectedCard = rng.NextItem(cards.Where(c => c.EnergyCost.GetWithModifiers(CostModifiers.None) > 0));
        }

        if (selectedCard != null)
        {
            Flash();
            selectedCard.EnergyCost.AddThisTurnOrUntilPlayed(-effectiveAmount);
        }

        return Task.CompletedTask;
    }
}