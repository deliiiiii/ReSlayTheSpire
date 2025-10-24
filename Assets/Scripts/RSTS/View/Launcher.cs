using System.Threading.Tasks;
using RSTS.CDMV;

namespace RSTS;
public class Launcher : Singleton<Launcher>
{
    async Task Start()
    {
        await Loader.LoadAll();
        MyFSM.Register(GameStateWrap.One, EGameState.Title, RefPoolSingle<GameData>.Register(() => new GameData()));
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