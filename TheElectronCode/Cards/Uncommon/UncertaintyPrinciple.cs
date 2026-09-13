using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;
using TheElectron.TheElectronCode.Powers;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class UncertaintyPrinciple : ElectronCard
{
    public UncertaintyPrinciple() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithKeyword(ElectronKeywords.Drain);
        WithTip(ElectronHoverTip.Produce);
        WithTip(ElectronHoverTip.Stable);
        WithQuarkTip<TopQuark>();
        WithQuarkTip<BottomQuark>();
        WithVar(new QuarkCountVar(1));
        WithPower<QuantumLinkPower>(5, 2);
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
            await QuarkCmd.Produce<TopQuark>(choiceContext, Owner, this, play, true);

        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
            await QuarkCmd.Produce<BottomQuark>(choiceContext, Owner, this, play, true);

        var quarks = Owner.PlayerCombatState?.GetQuarkQueue()?.Quarks ?? [];

        foreach (var _ in quarks)
        {
            var enemy = Owner.RunState.Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
            if (enemy == null) break;
            await PowerCmd.Apply<QuantumLinkPower>(choiceContext, enemy,
                DynamicVars.Power<QuantumLinkPower>().BaseValue, Owner.Creature, this);
            await Cmd.CustomScaledWait(0.15f, 0.3f);
        }
    }
}