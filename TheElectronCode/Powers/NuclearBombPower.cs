using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Powers;

public class NuclearBombPower : TheElectronPower, IAfterQuarksFused
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;


    public async Task AfterQuarksFused(PlayerChoiceContext choiceContext, Player player,
        IEnumerable<QuarkModel> fusedQuarks)
    {
        if (Owner.Player == player && fusedQuarks.Any())
        {
            Flash();
            await Cmd.CustomScaledWait(0.2f, 0.4f);
            foreach (var hittableEnemy in CombatState.HittableEnemies)
            {
                var instance = NCombatRoom.Instance;
                instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(hittableEnemy));
            }
            await Cmd.CustomScaledWait(0.2f, 0.4f);
            await CreatureCmd.Damage(choiceContext,CombatState.HittableEnemies, new DamageVar(Amount, ValueProp.Unpowered), Owner);
            await PowerCmd.Remove(this);
        }
    }
}