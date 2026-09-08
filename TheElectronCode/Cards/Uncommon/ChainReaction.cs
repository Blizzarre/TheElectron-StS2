using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.HoverTips;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class ChainReaction : ElectronDepleteCard
{
    public ChainReaction() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(12, 4);
        WithTip(ElectronHoverTip.Empty);
    }

    protected override async Task OnPlayWrapper(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override async Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        foreach (var card in PileType.Discard.GetPile(Owner).Cards.Where(c => c is ElectronEmptyCard).ToList())
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }
}