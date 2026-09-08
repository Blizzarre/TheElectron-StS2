using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class EndlessAssault : ElectronCard, IAfterFaradOrHpDrained
{
    public EndlessAssault() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithKeyword(ElectronKeywords.Drain);
        WithDamage(8, 3);
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
    
    protected override CardLocation GetResultLocationForCardPlay()
    {
        var locationForCardPlay = base.GetResultLocationForCardPlay();
        if (locationForCardPlay.pileType == PileType.Discard && _wasDrained)
            locationForCardPlay.pileType = PileType.Hand;
        _wasDrained = false;
        return locationForCardPlay;
    }

    private bool _wasDrained = false;

    public Task AfterFaradOrHpDrained(PlayerChoiceContext choiceContext, Player player, decimal amountDrained,
        CardModel? cardSource = null, CardPlay? cardPlay = null)
    {
        if (cardSource == this && cardPlay != null && amountDrained > 0)
        {
            _wasDrained = true;
        }

        return Task.CompletedTask;
    }
}