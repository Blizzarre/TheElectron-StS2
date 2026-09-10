using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class EventHorizon : ElectronCard, IAfterQuarksFused
{
    public EventHorizon() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(10, 4);
        WithVar("SlotCount", 1);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(ElectronHoverTip.Fuse);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        await QuarkCmd.AddSlots(Owner, DynamicVars["SlotCount"].IntValue);
    }

    public async Task AfterQuarksFused(PlayerChoiceContext choiceContext, Player player,
        IEnumerable<QuarkModel> fusedQuarks)
    {
        if (player != Owner) return;

        var handPile = PileType.Hand.GetPile(Owner);
        if (!handPile.Cards.Contains(this)) await CardPileCmd.Add(this, PileType.Hand);
    }
}