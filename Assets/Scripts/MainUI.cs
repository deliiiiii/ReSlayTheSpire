using System;
using UnityEngine.UI;
public interface IMain
{
    Action OnClickStart { get; set; }
    Action OnClickQuit { get; set; }
    Action OnLogicStart { get; set; }
}

public class MainUI : Singleton<MainUI>, IMain
{
    public Button btnStart;
    public Button btnQuit;
    public Action OnClickStart { get; set; }
    public Action OnClickQuit { get; set; }
    public Action OnLogicStart { get; set; }
    void Awake()
    {
        btnStart.onClick.AddListener(() => OnClickStart?.Invoke());
        btnQuit.onClick.AddListener(() => OnClickQuit?.Invoke());
        OnLogicStart = () => OnLogicStartCB();
    }


    void OnLogicStartCB()
    {
        btnStart.gameObject.SetActive(false);
        btnQuit.gameObject.SetActive(false);
    }
}