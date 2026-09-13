using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class EndlessAssault : ElectronCard, IAfterFaradLost
{
    public EndlessAssault() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeyword(ElectronKeywords.Drain);
        WithTip(ElectronHoverTip.Farad);
        WithDamage(12, 3);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    private bool WasFaradSpent { get; set; }

    protected override CardLocation GetResultLocationForCardPlay()
    {
        var locationForCardPlay = base.GetResultLocationForCardPlay();
        if (locationForCardPlay.pileType == PileType.Discard && WasFaradSpent)
            locationForCardPlay.pileType = PileType.Hand;
        return locationForCardPlay;
    }

    public Task AfterFaradLost(PlayerChoiceContext choiceContext, Player player, decimal amountLost,
        CardModel? cardSource = null,
        CardPlay? cardPlay = null)
    {
        if (player == Owner && cardSource == this && amountLost > 0) WasFaradSpent = true;

        return Task.CompletedTask;
    }


    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == this) WasFaradSpent = false;

        return Task.CompletedTask;
    }

    protected override void AfterCloned()
    {
        base.AfterCloned();
        WasFaradSpent = false;
    }
}