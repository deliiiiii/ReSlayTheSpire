using UnityEditor.Experimental.GraphView;

namespace BehaviourTree
{
    public class GuardNodeEditor : NodeBaseEditor<GuardNode>
    {
        public GuardNodeEditor(GuardNode nodeBase) : base(nodeBase)
        {
        }
     
        public Port outputPort;
        protected override void DrawPort()
        {
            outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single,
                typeof(GuardNodeEditor));
            outputPort.portName = "Guarding ↓";
            // outputPort.tooltip = typeof(IACDNodeEditor<ACDNode>).ToString();
            outputContainer.Add(outputPort);
        }

        public override void OnRefreshTree()
        {
        }
        
    }
}