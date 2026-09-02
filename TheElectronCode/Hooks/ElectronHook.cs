using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Hooks;

public class ElectronHook
{
    private static async Task Dispatch<T>(ICombatState combatState, Func<T, Task> action) where T : class
    {
        foreach (var model in combatState.IterateHookListeners().OfType<T>())
        {
            var abstractModel = (AbstractModel)(object)model;
            await action(model);
            abstractModel.InvokeExecutionFinished();
        }
    }

    private static async Task Dispatch<T>(IEnumerable<AbstractModel> models, Func<T, Task> action) where T : class
    {
        foreach (var model in models.OfType<T>())
        {
            var abstractModel = (AbstractModel)(object)model;
            await action(model);
            abstractModel.InvokeExecutionFinished();
        }
    }

    private static async Task Dispatch<T>(ICombatState combatState, PlayerChoiceContext choiceContext,
        Func<T, Task> action) where T : class
    {
        foreach (var model in combatState.IterateHookListeners().OfType<T>())
        {
            var abstractModel = (AbstractModel)(object)model;
            choiceContext.PushModel(abstractModel);
            await action(model);
            abstractModel.InvokeExecutionFinished();
            choiceContext.PopModel(abstractModel);
        }
    }

    private static TResult Aggregate<T, TResult>(ICombatState combatState, TResult seed,
        Func<T, TResult, TResult> action) where T : class
    {
        return combatState.IterateHookListeners().OfType<T>()
            .Aggregate(seed, (curr, model) => action(model, curr));
    }

    public static decimal ModifyQuarkValue(ICombatState combatState, QuarkModel quark, decimal amount)
    {
        var amountBeforeMult = Aggregate<IModifyQuarkValueAdditive, decimal>(combatState, amount,
            (model, current) => model.ModifyQuarkValueAdditive(quark, current));

        var mult = Aggregate<IModifyQuarkValueMultAdd, decimal>(combatState, 1m,
            (model, currentMult) => model.ModifyQuarkValueMultAdd(quark, currentMult));

        mult *= Aggregate<IModifyQuarkValueMult, decimal>(combatState, 1m,
            (model, currentMult) => model.ModifyQuarkValueMult(quark, currentMult));

        return amountBeforeMult * mult;
    }

    public static decimal ModifyFaradGain(ICombatState combatState, Player player, decimal originalAmount,
        ValueProp props, CardModel? cardSource,
        out IEnumerable<AbstractModel> modifiers)
    {
        var modifyingModels = new List<AbstractModel>();
        var res = Aggregate<IModifyFaradGain, decimal>(combatState, originalAmount, (model, current) =>
        {
            var next = model.ModifyFaradGain(player, current, props, cardSource);
            if (next != current) modifyingModels.Add((AbstractModel)model);
            return next;
        });
        modifiers = modifyingModels;
        return res;
    }

    public static Task AfterModifyingFaradGain(IEnumerable<AbstractModel> modifiers)
    {
        return Dispatch<IAfterModifyingFaradGain>(modifiers,
            model => model.AfterModifyingFaradGain());
    }

    public static Task AfterFaradGained(ICombatState combatState, PlayerChoiceContext choiceContext, Player player,
        decimal amountLost, CardModel? cardSource = null, CardPlay? cardPlay = null)
    {
        return Dispatch<IAfterFaradGained>(combatState,
            model => model.AfterFaradGained(choiceContext, player, amountLost, cardSource, cardPlay));
    }

    public static Task AfterFaradLost(ICombatState combatState, PlayerChoiceContext choiceContext, Player player,
        decimal amountLost, CardModel? cardSource = null, CardPlay? cardPlay = null)
    {
        return Dispatch<IAfterFaradLost>(combatState,
            model => model.AfterFaradLost(choiceContext, player, amountLost, cardSource, cardPlay));
    }

    public static Task AfterFaradOrHpDrained(ICombatState combatState, PlayerChoiceContext choiceContext, Player player,
        decimal amountLost, CardModel? cardSource = null, CardPlay? cardPlay = null)
    {
        return Dispatch<IAfterFaradOrHpDrained>(combatState,
            model => model.AfterFaradOrHpDrained(choiceContext, player, amountLost, cardSource, cardPlay));
    }

    public static bool ShouldQuarkBeStable(ICombatState combatState, QuarkModel quark, out AbstractModel? modifier)
    {
        foreach (var model in combatState.IterateHookListeners().OfType<IShouldQuarkBeStable>())
        {
            if (!model.ShouldQuarkBeStable(quark)) continue;
            modifier = (AbstractModel)model;
            return true;
        }

        modifier = null;
        return false;
    }

    public static Task AfterMakingQuarkStable(AbstractModel modifier)
    {
        return Dispatch<IAfterMakingQuarkStable>([modifier], model => model.AfterMakingQuarkStable());
    }

    public static Task AfterQuarksFused(ICombatState combatState, PlayerChoiceContext choiceContext, Player player,
        IEnumerable<QuarkModel> fusedQuarks)
    {
        return Dispatch<IAfterQuarksFused>(combatState, choiceContext,
            model => model.AfterQuarksFused(choiceContext, player, fusedQuarks));
    }
}