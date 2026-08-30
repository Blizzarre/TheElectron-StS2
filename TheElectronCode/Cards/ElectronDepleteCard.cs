using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards;

public abstract class ElectronDepleteCard : ElectronCard
{
    protected ElectronDepleteCard(int cost, CardType type, CardRarity rarity, TargetType target) : base(cost, type, rarity, target)
    {
        WithTip(ElectronHoverTip.Deplete);
        WithTags(ElectronTags.Deplete);
    }

    // Set during SpendResource
    public bool IsEnergyDepleted { get; set; }

    protected override bool ShouldGlowGoldInternal => WouldDeplete;

    // For description cond
    protected bool WouldDeplete
    {
        get
        {
            if (IsInCombat)
            {
                return EnergyCost.GetWithModifiers(CostModifiers.All) >= (Owner.PlayerCombatState?.Energy ?? 0);
            }

            return false;
        }
    }
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);

        description.Add("WouldDeplete", !IsInCombat || WouldDeplete);
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsEnergyDepleted)
        {
            await OnPlayDepleteBefore(choiceContext, cardPlay);
        }
        
        await OnPlayWrapper(choiceContext, cardPlay);
        
        if (IsEnergyDepleted)
        {
            await OnPlayDepleteAfter(choiceContext, cardPlay);
        }
    }

    protected virtual Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Override this for Deplete effects that'll happen before the main OnPlayWrapper
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="play"></param>
    /// <returns></returns>
    protected virtual Task OnPlayDepleteBefore(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Override this for Deplete effects that'll happen after the main OnPlayWrapper
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="play"></param>
    /// <returns></returns>
    protected virtual Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }
}