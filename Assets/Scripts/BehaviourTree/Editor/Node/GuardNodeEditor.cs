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
            OutputPort.tooltip = typeof(GuardNodeEditor).ToString();
            outputContainer.Add(OutputPort);
        }

        public override void OnRefreshTree()
        {
        }
        
    }
}