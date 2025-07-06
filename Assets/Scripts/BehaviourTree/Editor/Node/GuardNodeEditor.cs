using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class GuardNodeEditor : NodeBaseEditor<GuardNode>
    {
        public GuardNodeEditor(GuardNode nodeBase) : base(nodeBase)
        {
        }
     
        public Port OutputPort;
        protected override void DrawPort()
        {
            OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single,
                typeof(GuardNodeEditor));
            OutputPort.portName = "Guarding ↓";
            // outputPort.tooltip = typeof(IACDNodeEditor<ACDNode>).ToString();
            outputContainer.Add(OutputPort);
        }

        public override void OnRefreshTree()
        {
        }
        
    }
}