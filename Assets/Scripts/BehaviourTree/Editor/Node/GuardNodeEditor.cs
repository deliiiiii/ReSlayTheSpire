using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class GuardNodeEditor : NodeBaseEditor<GuardNode>
    {
        public GuardNodeEditor(GuardNode nodeBase) : base(nodeBase)
        {
        }
        // protected override void DrawPort()
        // {
        //     OutputGuardedPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single,
        //         typeof(GuardNodeEditor));
        //     OutputGuardedPort.portName = "Guarding ↓";
        //     OutputGuardedPort.tooltip = typeof(GuardNodeEditor).ToString();
        //     outputContainer.Add(OutputGuardedPort);
        // }
    }
}