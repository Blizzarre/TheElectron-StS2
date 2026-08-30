using Godot;

namespace TheElectron.TheElectronCode.Nodes.Quarks;

public partial class NQuarkVisuals : Node2D
{
    private Node2D? _visual;
    
    public override void _Ready()
    {
        _visual = GetNode<Node2D>("%Visuals");
    }
}