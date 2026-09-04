using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.CardSelection;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Common;

public class Shock : ElectronCard
{
    public Shock() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(ElectronKeywords.Drain);
        WithDamage(16, 4);
        WithCards(1, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        
        var cards = await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(ElectronCardSelectorPrefs.DrainSelectionPrompt, 0,DynamicVars.Cards.IntValue) {Cancelable = true},
            card => !card.Keywords.Contains(ElectronKeywords.Drain), this
        );
        foreach (var card in cards)
        {
            CardCmd.ApplyKeyword(card, ElectronKeywords.Drain);
        }
    }
}