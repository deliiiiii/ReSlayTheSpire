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
    protected override void IBL()
    {
        fullMapWindow
            .OnAdd(_ =>
            {
                NormalReticle.SetActive(false);
                FindReticle.SetActive(false);
                SceneItemInfoPnl.SetActive(false);
                ShowFullScreenMap();
            })
            .OnRemove(_ => ShowMinimap());
        
        GameManager.GeneratingMapState
            .OnEnter(() => LoadPnl.SetActive(true))
            .OnExit(() => LoadPnl.SetActive(false));
        GameManager.PlayingState
            .OnEnter(() =>
            {
                MiniItemPnl.SetActive(true);
                
                GameManager.WindowList.MyRemove(fullMapWindow);
                
                Binder.From(PlayerManager.StaminaCount).ToTxt(StaminaTxt).Immediate();
                Binder.From(PlayerManager.EnergyCount).ToTxt(EnergyTxt).Immediate();
                Binder.From(PlayerManager.GlovesCount).ToTxt(GlovesTxt).Immediate();
                Binder.From(PlayerManager.DiceCount).ToTxt(DiceTxt).Immediate();
            })
            .OnLateUpdate(dt =>
            {
                var cb = PlayerManager.InteractStream?.Value;
                PlayerManager.InteractStream?.OnEndAsync(GetUICb);
                NormalReticle.SetActive(cb == null);
                FindReticle.SetActive(cb != null);
                SceneItemInfoPnl.SetActive(cb != null);
                SceneItemInfoTxt.text = cb?.Description ?? "";
                SceneItemInfoTxt.color = cb?.Color ?? Color.black;

                ChangeFOV(dt);
                
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    if (isMinimap && !GameManager.HasWindow)
                        GameManager.WindowList.MyAdd(fullMapWindow);
                    else if(!isMinimap)
                        GameManager.WindowList.MyRemove(fullMapWindow);
                }
            })
            .OnExit(() => MiniItemPnl.SetActive(false));

        GameManager.PauseWindow
            .OnAdd(_ =>
            {
                NormalReticle.SetActive(false);
                FindReticle.SetActive(false);
                SceneItemInfoPnl.SetActive(false);
                PausePnl.SetActive(true);
            })
            .OnRemove(_ =>
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
        // 设置gameObject的 RectTransform长宽
        FullScreenImg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height, Screen.height);
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
        FullScreenImg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(height, height);
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
    async Task GetUICb(InteractInfo cb)
    {
        if (cb.IsSleep)
        {
            await FadeImageAlpha(SleepPnl, cb.SleepTime);
        }
    }

    static async Task FadeImageAlpha(GameObject go, float duration)
    {
        try
        {
            GameManager.WindowList.MyAdd(sleepWindow);
            go.SetActive(true);
            var img = go.GetComponent<Image>();
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
            go.SetActive(false);
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