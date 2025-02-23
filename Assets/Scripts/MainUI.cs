using System;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField]
    Button btnStart;
    [SerializeField]
    Button btnQuit;
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