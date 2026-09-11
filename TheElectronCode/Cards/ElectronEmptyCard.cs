using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Powers;
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
    
    // TODO Hook this?
    public bool AllEffectsOverride => Owner.Creature.HasPower<DividedByZeroPower>();

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
            var checkOverride = AllEffectsOverride;
            description.Add("IsEmpty", WouldBeEmpty || checkOverride);
            description.Add("HasEnoughEnergy", HasEnoughEnergy || checkOverride);
        }
        else
        {
            description.Add("IsEmpty", true);
            description.Add("HasEnoughEnergy", true);
        }
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await BeforeOnPlay(choiceContext, cardPlay);
        
        var checkOverride = AllEffectsOverride;
        if (IsPlayedAsEmpty || checkOverride) await OnPlayEmptyBefore(choiceContext, cardPlay);
        if (HasPaidEnergyCost || checkOverride) await OnPlayWrapper(choiceContext, cardPlay);
        if (IsPlayedAsEmpty || checkOverride) await OnPlayEmptyAfter(choiceContext, cardPlay);

        await AfterOnPlay(choiceContext, cardPlay);
    }

    protected virtual Task BeforeOnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
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

    protected virtual Task AfterOnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }
}