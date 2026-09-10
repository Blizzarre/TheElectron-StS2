using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheElectron.TheElectronCode.Combat;
using TheElectron.TheElectronCode.HoverTips;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class SplitTheAtom : ElectronCard
{
    public SplitTheAtom() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithTip(ElectronHoverTip.Fuse);
        WithDamage(13, 4);
        WithCalculatedVar("CalculatedHits", 1, static (card, _) =>
        {
            return CombatManager.Instance.History.Entries.OfType<QuarksFusedEntry>()
                .Count(e => e.Actor == card.Owner.Creature);
        });
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        var hitCount = (int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(play.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .Targeting(play.Target)
            .WithHitCount(hitCount)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }
}