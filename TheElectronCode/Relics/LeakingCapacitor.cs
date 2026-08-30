using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Rooms;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Relics;

public class LeakingCapacitor : TheElectronRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    private const string UpQuarkKey = "UpQuark";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(FaradVar.defaultName, 2), // not actually using FaradVar here
        new(UpQuarkKey, 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ElectronHoverTipFactory.Static(ElectronHoverTip.Produce),
        ElectronHoverTipFactory.FromQuark<UpQuark>(),
        ElectronHoverTipFactory.Static(ElectronHoverTip.Farad)
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            await ElectronPlayerCmd.GainFarad(new ThrowingPlayerChoiceContext(), Owner, DynamicVars.Farad().IntValue);
        }
    }
    
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature) && Owner.PlayerCombatState is { TurnNumber: <= 1 })
        {
            for (var i = 0; i < DynamicVars[UpQuarkKey].IntValue; i++)
            {
                await QuarkCmd.Produce<UpQuark>(new BlockingPlayerChoiceContext(), Owner);
            }
        }
    }
}