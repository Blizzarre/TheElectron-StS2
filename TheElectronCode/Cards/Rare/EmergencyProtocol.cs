using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Powers;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class EmergencyProtocol : ElectronCard
{
    public EmergencyProtocol() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip(ElectronKeywords.Drain);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<EmergencyProtocolPower>(choiceContext, Owner.Creature,
            1, Owner.Creature, this);
    }
}