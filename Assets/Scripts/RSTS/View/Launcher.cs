using System.Threading.Tasks;

namespace RSTS;
public class Launcher : Singleton<Launcher>
{
    async Task Start()
    {
        await Loader.LoadAll();
        MyFSM.Register(GameStateWrap.One, EGameState.Title, new SlotData());
    }

    void OnDestroy()
    {
        MyFSM.Release(GameStateWrap.One);
    }

    // void Update()
    // {
    //     // Sirenix.Utilities.Editor.GUIHelper.RequestRepaint();
    // }
}