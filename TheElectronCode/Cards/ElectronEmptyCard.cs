using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards;

public abstract class ElectronEmptyCard : ElectronCard
{
    protected ElectronEmptyCard(int cost, CardType type, CardRarity rarity, TargetType target) : base(cost, type,
        rarity, target)
    {
        WithTip(ElectronHoverTip.Empty);
        WithTags(ElectronTags.Empty);
    }

    // Set during SpendResource
    public bool IsPlayedAsEmpty { get; set; }

    public bool HasPaidEnergyCost { get; set; }

    public bool WouldBeEmpty
    {
        get
        {
            if (IsInCombat) return Owner.PlayerCombatState?.Energy == 0;
            return true;
        }
    }

    public bool HasEnoughEnergy
    {
        get
        {
            if (IsInCombat)
                return EnergyCost.GetWithModifiers(CostModifiers.All) <= (Owner.PlayerCombatState?.Energy ?? 0) ||
                       Keywords.Contains(ElectronKeywords.Drain);
            return true;
        }
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);

        if (IsInCombat)
        {
            description.Add("IsEmpty", WouldBeEmpty);
            description.Add("HasEnoughEnergy", HasEnoughEnergy);
        }
        else
        {
            description.Add("IsEmpty", true);
            description.Add("HasEnoughEnergy", true);
        }
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsPlayedAsEmpty)
        {
            await OnPlayEmptyBefore(choiceContext, cardPlay);
        }
        if (HasPaidEnergyCost)
        {
            await OnPlayWrapper(choiceContext, cardPlay);
        }
        if (IsPlayedAsEmpty)
        {
            await OnPlayEmptyAfter(choiceContext, cardPlay);
        }
    }

    protected virtual Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }
    
    protected virtual Task OnPlayEmptyBefore(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }
}