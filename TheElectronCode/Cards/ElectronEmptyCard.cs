using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards;

public abstract class ElectronEmptyCard : ElectronCard
{
    protected ElectronEmptyCard(int cost, CardType type, CardRarity rarity, TargetType target) : base(cost, type, rarity, target)
    {
        WithTip(ElectronHoverTip.Empty);
        WithTags(ElectronTags.Empty);
    }

    // Set during SpendResource
    public bool IsPlayedAsEmpty { get; set; }
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);

        if (IsInCombat)
        {
            var isEmpty = Owner.PlayerCombatState?.Energy == 0;
            description.Add("IsEmpty", isEmpty);
            description.Add("IsNotEmpty", !isEmpty);
        }
        else
        {
            description.Add("IsEmpty", true);
            description.Add("IsNotEmpty", true);
        }
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsPlayedAsEmpty)
        {
            await OnPlayEmpty(choiceContext, cardPlay);
            return;
        }

        await OnPlayWrapper(choiceContext, cardPlay);
    }

    protected virtual Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnPlayEmpty(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }
}