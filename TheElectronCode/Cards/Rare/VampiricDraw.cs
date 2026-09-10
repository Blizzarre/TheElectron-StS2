using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Saves.Runs;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class VampiricDraw : ElectronDepleteCard
{
    private const int BaseFarad = 1;
    private const string IncreaseKey = "Increase";

    [SavedProperty]
    private int CurrentFarad
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            DynamicVars.Farad.BaseValue = field;
        }
    } = BaseFarad;

    [SavedProperty]
    private int IncreasedFarad
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public VampiricDraw() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithTip(ElectronHoverTip.Fuse);
        WithDamage(15, 2);
        WithVar(new FaradVar(CurrentFarad));
        WithVar(IncreaseKey, 1, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad.BaseValue, this, play);
    }

    protected override Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var increase = DynamicVars[IncreaseKey].IntValue;
        BuffFromPlay(increase);
        if (DeckVersion is not VampiricDraw deckVersion) return Task.CompletedTask;
        deckVersion.BuffFromPlay(increase);
        return Task.CompletedTask;
    }

    private void BuffFromPlay(int extraFarad)
    {
        IncreasedFarad += extraFarad;
        UpdateFarad();
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        UpdateFarad();
    }

    private void UpdateFarad()
    {
        CurrentFarad = BaseFarad + IncreasedFarad;
    }
}