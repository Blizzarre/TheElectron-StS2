using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Entities;

public class QuarkQueue
{
    public const int MaxCapacity = 10;

    private Player Owner { get; }

    private readonly List<QuarkModel> _quarks = [];

    public IReadOnlyList<QuarkModel> Quarks => _quarks;

    public int Capacity { get; set; } = 3;

    public QuarkQueue(Player owner)
    {
        Owner = owner;
    }

    public void Clear()
    {
        _quarks.Clear();
    }

    public bool HasAny()
    {
        return _quarks.Count != 0;
    }

    public async Task<bool> TryEnqueue(QuarkModel quark)
    {
        if (Capacity == 0) return false;
        quark.AssertMutable();
        if (Quarks.Count >= Capacity) throw new InvalidOperationException("QuarkQueue is full");

        _quarks.Add(quark);
        await SmallWait();
        return true;
    }

    public bool Remove(QuarkModel quark)
    {
        return _quarks.Remove(quark);
    }

    public void Insert(int idx, QuarkModel quark)
    {
        if (idx > Capacity) throw new InvalidOperationException("idx cannot be greater than capacity");

        _quarks.Insert(idx, quark);
    }

    public bool IsFull()
    {
        return Quarks.Count >= Capacity;
    }
    
    private async Task SmallWait()
    {
        if (LocalContext.IsMe(Owner))
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        else
            await Cmd.Wait(0.05f);
    }
}