using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

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

    public static void AfterEnergyChanged(ICombatState combatState, Player player, int energy)
    {
        foreach (var model in combatState.IterateHookListeners().OfType<IAfterEnergyChanged>())
        {
            model.AfterEnergyChanged(player, energy);
        }
    }
}