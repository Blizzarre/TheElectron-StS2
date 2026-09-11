using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;
using TheElectron.TheElectronCode.Powers;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class NeutronCollector : ElectronCard
{
    public NeutronCollector() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(ElectronKeywords.Drain, UpgradeType.Add);
        WithTip(ElectronHoverTip.Produce);
        WithQuarkTip<UpQuark>();
        WithQuarkTip<DownQuark>();
        WithVar("Amount", 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<NeutronCollectorPower>(choiceContext, Owner.Creature,
            DynamicVars["Amount"].BaseValue, Owner.Creature, this);
    }
}