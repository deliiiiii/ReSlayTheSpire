using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static MyFSM gameFSM;
    AssetBundle ab;
    GameObject prefabUICard;
    void Awake()
    {
        gameFSM = new MyFSM(typeof(TitleState));

        ab = AssetBundle.LoadFromFile("AssetBundles/StandaloneWindows/prefabs");
        prefabUICard = ab.LoadAsset<GameObject>("UICard");
        GameObject card = Instantiate(prefabUICard);

    }
    void Update()
    {
    }
}