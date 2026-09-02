using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using TheElectron.TheElectronCode.Entities;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Field;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Commands;

public static class QuarkCmd
{
    /// <summary>
    /// Add Quark slots to the player. Can only have a maximum of <see cref="QuarkQueue.MaxCapacity"/> (10) slots.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="amount"></param>
    /// <param name="isTempSlot"></param>
    /// <returns></returns>
    public static Task AddSlots(Player player, int amount, bool isTempSlot = false)
    {
        if (CombatManager.Instance.IsOverOrEnding) return Task.CompletedTask;

        var queue = player.PlayerCombatState?.GetQuarkQueue();
        var visualAmountAdded = queue?.AddCapacity(amount, isTempSlot) ?? 0;
        var nCreature = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        if (nCreature != null)
        {
            var quarkManager = ElectronNode.NQuarkManager[nCreature];
            quarkManager?.AddSlotAnim(visualAmountAdded);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Produce a Quark given a Quark type <c>T</c>. Will automatically Fuse Quarks if all slots are filled after Produce.
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="player"></param>
    /// <param name="card"></param>
    /// <param name="cardPlay"></param>
    /// <param name="isStable"></param>
    /// <typeparam name="T"></typeparam>
    public static async Task Produce<T>(PlayerChoiceContext choiceContext, Player player,
        CardModel? card = null, CardPlay? cardPlay = null,
        bool isStable = false) where T : QuarkModel
    {
        var model = ModelDb.Get<T>().ToMutable();
        model.IsStable = isStable;
        await Produce(choiceContext, model, player, card, cardPlay);
    }

    /// <summary>
    /// Produce a Quark given a Quark model. Will automatically Fuse Quarks if all slots are filled after Produce.
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="quark"></param>
    /// <param name="player"></param>
    /// <param name="card"></param>
    /// <param name="cardPlay"></param>
    public static async Task Produce(PlayerChoiceContext choiceContext, QuarkModel quark, Player player,
        CardModel? card = null, CardPlay? cardPlay = null)
    {
        if (!CombatManager.Instance.IsOverOrEnding && player.Creature.CombatState != null)
        {
            var combatState = player.Creature.CombatState;
            var quarkQueue = player.PlayerCombatState?.GetQuarkQueue();
            if (quarkQueue == null) return;
            quark.AssertMutable();

            quark.Owner = player;
            // Add Slots if player have no slots
            if (player.Character is not Character.TheElectron && quarkQueue.Capacity == 0)
                await AddSlots(player, QuarkQueue.DefaultCapacity);

            // Hook to modify quark stability.
            if (!quark.IsStable && ElectronHook.ShouldQuarkBeStable(combatState, quark, out var model))
            {
                quark.IsStable = true;
                if (model != null) await ElectronHook.AfterMakingQuarkStable(model);
            }

            // Add temp slot
            if (quark.IsStable)
            {
                quark.HasStableSlot = true;
                await AddSlots(player, 1, true);
            }

            if (await quarkQueue.TryEnqueue(quark))
            {
                // TODO Play sfx
                var nCreature = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
                if (nCreature != null)
                {
                    var quarkManager = ElectronNode.NQuarkManager[nCreature];
                    quarkManager?.AddQuarkAnim();
                }

                if (quarkQueue.IsFull())
                {
                    await Cmd.CustomScaledWait(0.25f, 0.4f);
                    await Fuse(choiceContext, player, card, cardPlay);
                }
                // TODO after quark produced hook
            }
        }
    }

    public static async Task Fuse(PlayerChoiceContext choiceContext, Player player, CardModel? card = null,
        CardPlay? cardPlay = null)
    {
        if (!CombatManager.Instance.IsOverOrEnding && player.Creature.CombatState != null)
        {
            var combatState = player.Creature.CombatState;
            var quarkQueue = player.PlayerCombatState?.GetQuarkQueue();
            if (quarkQueue == null || !quarkQueue.HasAny()) return;

            await quarkQueue.FuseQuarks(choiceContext);

            // TODO after fused hook
            
        }
    }
}