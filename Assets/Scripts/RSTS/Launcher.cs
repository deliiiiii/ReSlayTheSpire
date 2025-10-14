using System.Threading.Tasks;

namespace RSTS;
public class Launcher : Singleton<Launcher>
{
    async Task Start()
    {
        await Loader.LoadAll();
        MyFSM.Register(EGameState.Title);
    }

    void OnDestroy()
    {
        MyFSM.Release<EGameState>();
    }

    // void Update()
    // {
    //     // Sirenix.Utilities.Editor.GUIHelper.RequestRepaint();
    // }
}