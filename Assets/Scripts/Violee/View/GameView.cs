using System;
using System.Collections.Generic;
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
        // WindowType = EWindowType.NormalUI,
        Des = "全屏地图"
    };
    static readonly WindowInfo sleepWindow = new ()
    {
        // WindowType = EWindowType.WaitingSceneItem,
        Des = "休息..."
    };
    public static readonly WindowInfo DrawWindow = new ()
    {
        // WindowType = EWindowType.NormalUI,
        Des = "选择房间装修中"
    };
    protected override void IBL()
    {
        GameManager.WindowList.OnAdd += w =>
        {
            NormalReticle.SetActive(false);
            FindReticle.SetActive(false);
            SceneItemInfoPnl.SetActive(false);
            if (w is BuffWindowInfo buffWindow)
            {
                var windowIns = Instantiate(BuffWindowPrefab, OverlapWindowTransform);
                buffWindow.BuffWindowIns = windowIns.gameObject;
                windowIns.CloseButton.onClick.AddListener(() =>
                {
                    GameManager.WindowList.MyRemove(buffWindow);
                });
                windowIns.DesTxt.text = buffWindow.Des;
                windowIns.gameObject.SetActive(true);
            }
            else if (w is MusicWindowInfo musicWindow)
            {

            }
        };

        GameManager.WindowList.OnRemove += w =>
        {
            if (w is BuffWindowInfo buffWindow)
            {
                Destroy(buffWindow.BuffWindowIns);
            }
        };
        
        fullMapWindow
            .OnAdd(ShowFullScreenMap)
            .OnRemove(ShowMinimap);
        sleepWindow
            .OnAdd(() => SleepPnl.SetActive(true))
            .OnRemove(() => SleepPnl.SetActive(false));
        DrawWindow
            .OnAdd(() => DrawPnl.SetActive(true))
            .OnRemove(() => DrawPnl.SetActive(false));
        GameManager.PauseWindow
            .OnAdd(() => PausePnl.SetActive(true))
            .OnRemove(() =>
            {
                PausePnl.SetActive(false);
            });
        GameManager.WatchingClockWindow
            .OnAdd(() => CameraMono.SceneItemVirtualCamera.gameObject.SetActive(true))
            .OnRemove(() =>
            {
                CameraMono.SceneItemVirtualCamera.gameObject.SetActive(false);
                ExitWatchingItemBtn.gameObject.SetActive(false);
            });

        Binder.From(PlayerManager.InteractInfo).To(info =>
        {
            NormalReticle.SetActive(info == null);
            FindReticle.SetActive(info != null);
            SceneItemInfoPnl.SetActive(info != null);
            SceneItemInfoTxt.text = info?.Description ?? "";
            SceneItemInfoTxt.color = info?.Color ?? Color.black;
        }).Immediate();
        PlayerManager.OnClickInteract += info =>
        {
            try
            {
                _ = GetUICb(info);
            }
            catch (Exception e)
            {
                MyDebug.LogError(e);
                throw;
            }
        };
        
        GameManager.GeneratingMapState
            .OnEnter(() => LoadPnl.SetActive(true))
            .OnExit(() => LoadPnl.SetActive(false));
        GameManager.PlayingState
            .OnEnter(() =>
            {
                MiniItemPnl.SetActive(true);
                ShowMinimap();
                
                Binder.From(PlayerManager.StaminaCount).ToTxt(StaminaTxt).Immediate();
                Binder.From(PlayerManager.EnergyCount).ToTxt(EnergyTxt).Immediate();
                Binder.From(PlayerManager.GlovesCount).ToTxt(GlovesTxt).Immediate();
                Binder.From(PlayerManager.DiceCount).ToTxt(DiceTxt).Immediate();
                Binder.From(PlayerManager.DiceCount).To(v =>
                {
                    RedrawBtn.interactable = v > 0;
                }).Immediate();
            })
            .OnUpdate(dt =>
            {
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
        
        Binder.From(ContinueBtn).To(() => GameManager.WindowList.MyRemove(GameManager.PauseWindow));
        Binder.From(ExitWatchingItemBtn).To(async () =>
        {
            try
            {
                ExitWatchingItemBtn.gameObject.SetActive(false);
                CameraMono.SceneItemVirtualCamera.gameObject.SetActive(false);
                await Task.Delay(CameraMono.PlayerEase);
                GameManager.WindowList.MyRemove(GameManager.WatchingClockWindow);
            }
            catch (Exception e)
            {
                MyDebug.LogError(e);
                throw;
            }
        });
        
        Binder.From(RedrawBtn).To(() =>
        {
            PlayerManager.DiceCount.Value--;
            showDrawConfigsAct();
        });
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
    float maxSize => Mathf.Max(Configer.BoxConfigList.Width, Configer.BoxConfigList.Height) * BoxHelper.BoxSize;
    void ChangeFOV(float dt)
    {
        
        var tarSize = isMinimap ? MiniSize : maxSize / 1.616f;
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
    public required Button RedrawBtn;
    public required Transform DrawBtnContent;

    public required Button ExitWatchingItemBtn;
    
    #region OverlapWindow
    [Header("OverlapWindow")]
    public required Transform OverlapWindowTransform;
    public required BuffWindow BuffWindowPrefab;
    public required GameObject BGMWindow;
    #endregion
    
    
    Action showDrawConfigsAct = () => { };
    async Task GetUICb(InteractInfo info)
    {
        if (info is SceneItemInteractInfo itemInfo)
        {
            if(itemInfo.SceneItemData.IsSleep)
                await FadeImageAlpha(itemInfo.SceneItemData.SleepTime);
            if (itemInfo.SceneItemData.HasCamera)
            {
                GameManager.WindowList.MyAdd(GameManager.WatchingClockWindow);
                await Task.Delay(CameraMono.SceneItemEase);
                ExitWatchingItemBtn.gameObject.SetActive(true);
            }
        }
        else if (info is DoorInteractInfo doorInfo)
        {
            GameManager.WindowList.MyAdd(DrawWindow);
            GameManager.WindowList.MyAdd(fullMapWindow);
            DrawBtnContent.DisableAllChildren();
            showDrawConfigsAct = () =>
            {
                var configs = doorInfo.GetDrawConfigs() ?? [];
                for (int i = 0; i < configs.Count; i++)
                {
                    var config = configs[i];
                    var go = DrawBtnContent.GetChild(i).gameObject;
                    go.SetActive(true);
                    go.GetComponent<Image>().sprite = config.Sprite;
                    go.GetComponent<Button>().onClick.RemoveAllListeners();
                    go.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        MapManager.DrawAtWall(doorInfo.InsidePointDataList, doorInfo.WallData, config);
                        GameManager.WindowList.MyRemove(DrawWindow);
                        GameManager.WindowList.MyRemove(fullMapWindow);
                    });
                    go.GetComponentInChildren<Text>().text = config.DrawDes;
                }
            };
            showDrawConfigsAct();
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