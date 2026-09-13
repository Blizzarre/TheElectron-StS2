using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class NuclearBomb : ElectronCard
{
    public NuclearBomb() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithTip(ElectronHoverTip.Produce);
        WithTip(ElectronHoverTip.Stable);
        WithQuarkTip<DownQuark>();
        WithVar("BombDamage", 32, 8);
        WithVar(new QuarkCountVar(3));
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<NuclearBombPower>(choiceContext, Owner.Creature,
            DynamicVars["BombDamage"].BaseValue, Owner.Creature, this);
        
        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
            await QuarkCmd.Produce<DownQuark>(choiceContext, Owner, this, play, true);
    }
}