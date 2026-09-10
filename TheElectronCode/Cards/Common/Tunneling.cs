using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.HoverTips;

namespace TheElectron.TheElectronCode.Cards.Common;

public class Tunneling : ElectronCard
{
    public Tunneling() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 3);
        WithCards(1);
        WithVar("PutBack", 1);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars["PutBack"].IntValue);
        await CardPileCmd.Add(await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this), PileType.Draw,
            CardPilePosition.Top);
    }
}