using System;
using System.Threading.Tasks;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

class GameView : ViewBase<GameView>
{
    static readonly WindowInfo fullMapWindow = new ()
    {
        WindowType = EWindowType.NormalUI,
        Des = "全屏地图"
    };
    static readonly WindowInfo sleepWindow = new ()
    {
        WindowType = EWindowType.WaitingSceneItem,
        Des = "休息..."
    };
    static readonly WindowInfo drawWindow = new ()
    {
        WindowType = EWindowType.NormalUI,
        Des = "选择房间装修中"
    };
    protected override void IBL()
    {
        fullMapWindow
            .OnAdd(() =>
            {
                NormalReticle.SetActive(false);
                FindReticle.SetActive(false);
                SceneItemInfoPnl.SetActive(false);
                ShowFullScreenMap();
            })
            .OnRemove(ShowMinimap);
        sleepWindow
            .OnAdd(() => SleepPnl.SetActive(true))
            .OnRemove(() => SleepPnl.SetActive(false));
        drawWindow
            .OnAdd(() => DrawPnl.SetActive(true))
            .OnRemove(() => DrawPnl.SetActive(false));
        
        GameManager.GeneratingMapState
            .OnEnter(() => LoadPnl.SetActive(true))
            .OnExit(() => LoadPnl.SetActive(false));
        GameManager.PlayingState
            .OnEnter(() =>
            {
                MiniItemPnl.SetActive(true);
                
                GameManager.WindowList.MyRemove(fullMapWindow);
                GameManager.WindowList.MyRemove(drawWindow);
                
                Binder.From(PlayerManager.StaminaCount).ToTxt(StaminaTxt).Immediate();
                Binder.From(PlayerManager.EnergyCount).ToTxt(EnergyTxt).Immediate();
                Binder.From(PlayerManager.GlovesCount).ToTxt(GlovesTxt).Immediate();
                Binder.From(PlayerManager.DiceCount).ToTxt(DiceTxt).Immediate();
            })
            .OnUpdate(dt =>
            {
                var cb = PlayerManager.InteractStream?.StartValue;
                PlayerManager.InteractStream?.RemoveOnEndAsync(GetUICb);
                PlayerManager.InteractStream?.OnEndAsync(GetUICb);
                NormalReticle.SetActive(cb == null);
                FindReticle.SetActive(cb != null);
                SceneItemInfoPnl.SetActive(cb != null);
                SceneItemInfoTxt.text = cb?.Description ?? "";
                SceneItemInfoTxt.color = cb?.Color ?? Color.black;

                ChangeFOV(dt);
                
                if (Input.GetKeyDown(KeyCode.Tab) && !GameManager.HasPaused)
                {
                    if (isMinimap)
                        GameManager.WindowList.MyAdd(fullMapWindow);
                    else if(!isMinimap)
                        GameManager.WindowList.MyRemove(fullMapWindow);
                }
            })
            .OnExit(() => MiniItemPnl.SetActive(false));

        GameManager.PauseWindow
            .OnAdd(() =>
            {
                NormalReticle.SetActive(false);
                FindReticle.SetActive(false);
                SceneItemInfoPnl.SetActive(false);
                PausePnl.SetActive(true);
            })
            .OnRemove(() =>
            {
                PausePnl.SetActive(false);
            });
        Binder.From(ContinueBtn).To(GameManager.UnPauseWindow);
    }

    [Header("Load & Pause")]
    public required GameObject LoadPnl;
    public required GameObject PausePnl;
    public required Button ContinueBtn;

    #region Minimap

    [Header("Minimap")]
    public required RenderTexture TarTexture;
    public required CinemachineVirtualCamera MinimapCameraVirtual;
    public required Camera MinimapCamera;
    public required RawImage MinimapImg;
    public required RawImage FullScreenImg;
    public float ChangeSpeed = 1.2f;
    public float MiniSize = 12f;
    bool isMinimap => MinimapImg.enabled;
    void ChangeFOV(float dt)
    {
        var tarSize = isMinimap ? MiniSize : MapManager.MaxSize / 1.616f;
        if (!Mathf.Approximately(MinimapCameraVirtual.m_Lens.OrthographicSize, tarSize))
        {
            MinimapCameraVirtual.m_Lens.OrthographicSize = Mathf.Lerp(MinimapCameraVirtual.m_Lens.OrthographicSize,
                tarSize,
                ChangeSpeed * dt);
        }
    }
    void ShowMinimap()
    {
        RefreshTexture(256, 256);
        FullScreenImg.enabled = false;
        MinimapImg.enabled = true;
    }

    void ShowFullScreenMap()
    {
        
        // MyDebug.Log("ShowFullScreenMap " + Screen.height);
        RefreshTexture(Screen.width, Screen.height);
        FullScreenImg.enabled = true;
        MinimapImg.enabled = false;
    }
    
    void RefreshTexture(int width, int height)
    {
        TarTexture.Release();
        TarTexture.width = height;
        TarTexture.height = height;
        TarTexture.Create();
        MinimapCamera.targetTexture = TarTexture;
        MinimapImg.texture = TarTexture;
        FullScreenImg.texture = TarTexture;
    }
    

    #endregion
    
    
    #region MiniItem
    [Header("MiniItem")] 
    public required GameObject MiniItemPnl;
    public required Text StaminaTxt;
    public required Text EnergyTxt;
    public required Text GlovesTxt;
    public required Text DiceTxt;
    #endregion


    #region SceneItem
    [Header("SceneItem")]
    public required GameObject NormalReticle;
    public required GameObject FindReticle;
    public required GameObject SceneItemInfoPnl;
    public required Text SceneItemInfoTxt;

    public required GameObject SleepPnl;
    public required GameObject DrawPnl;
    public required Transform DrawBtnContent;
    async Task GetUICb(InteractInfo? cb)
    {
        if (cb == null)
            return;
        if (cb.IsSleep)
        {
            await FadeImageAlpha(cb.SleepTime);
        }
        else if (cb.IsOpenDoor)
        {
            GameManager.WindowList.MyAdd(drawWindow);
            GameManager.WindowList.MyAdd(fullMapWindow);
            DrawBtnContent.DisableAllChildren();
            var configs = cb.GetDrawConfigs();
            for (int i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                var go = DrawBtnContent.GetChild(i).gameObject;
                go.SetActive(true);
                go.GetComponent<Image>().sprite = config.Sprite;
                go.GetComponent<Button>().onClick.RemoveAllListeners();
                go.GetComponent<Button>().onClick.AddListener(() =>
                {
                    MapManager.DrawAtWall(cb.WallData, config);
                    GameManager.WindowList.MyRemove(drawWindow);
                    GameManager.WindowList.MyRemove(fullMapWindow);
                });
                go.GetComponentInChildren<Text>().text = config.DrawDes;
            }
        }
    }

    async Task FadeImageAlpha(float duration)
    {
        try
        {
            GameManager.WindowList.MyAdd(sleepWindow);
            var img = SleepPnl.GetComponent<Image>();
            var initAlpha = 0f;
            var half = duration * 0.5f;
    
            // Fade in
            for (float t = 0; t < half; t += Time.deltaTime)
            {
                var norm = t / half;
                var eased = Mathf.SmoothStep(0f, 1f, norm);
                var c = img.color;
                c.a = Mathf.Lerp(initAlpha, 1f, eased);
                img.color = c;
                await Task.Yield();
            }
            img.color.SetAlpha(1);
            // Fade out
            for (float t = 0; t < half; t += Time.deltaTime)
            {
                var norm = t / half;
                var eased = Mathf.SmoothStep(0f, 1f, norm);
                var c = img.color;
                c.a = Mathf.Lerp(1f, initAlpha, eased);
                img.color = c;
                await Task.Yield();
            }
            img.color.SetAlpha(initAlpha);
            GameManager.WindowList.MyRemove(sleepWindow);
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
        
    }
    #endregion
}