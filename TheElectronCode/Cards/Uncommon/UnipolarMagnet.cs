using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TheElectron.TheElectronCode.Cards.Uncommon;


public class UnipolarMagnet : ElectronDepleteCard
{
    private const string CardsFromDiscardKey = "CardsFromDiscard";
    
    public UnipolarMagnet() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(1);
        WithVar(CardsFromDiscardKey, 1, 1);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars[CardsFromDiscardKey].IntValue);
        var cardModels = (await CardSelectCmd.FromCombatPile(choiceContext,
            PileType.Discard.GetPile(Owner), Owner, prefs)).ToList();
        foreach (var card in cardModels)
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    protected override async Task OnPlayDepleteBefore(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }
}