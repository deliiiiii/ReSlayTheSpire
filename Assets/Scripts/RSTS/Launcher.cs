namespace RSTS;
public class Launcher : Singleton<Launcher>
{
    void Start()
    {
        MyFSM.Register(EGameState.Title);
    }

    void OnDestroy()
    {
        MyFSM.Release<EGameState>();
    }

    void Update()
    {
        // Sirenix.Utilities.Editor.GUIHelper.RequestRepaint();
    }
}