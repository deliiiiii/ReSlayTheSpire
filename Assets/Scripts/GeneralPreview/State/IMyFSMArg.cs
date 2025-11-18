public interface IMyFSMArg<in TFSM>
{
    public void Bind(TFSM fsm);
    public void Launch();
    public void UnInit();
}