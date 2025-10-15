using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

class GameView : ViewBase<GameView>
{
    public static void Init() => Instance.IBL();
    protected override void IBL()
    {
        WindowManager.WindowList.OnAdd?.AddListener(w =>
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
                    WindowManager.WindowList.MyRemove(buffWindow);
                });
                windowIns.DesTxt.text = buffWindow.Des;
                windowIns.gameObject.SetActive(true);
            }
        });

        WindowManager.WindowList.OnRemove?.AddListener(w =>
        {
            if (w is BuffWindowInfo buffWindow)
            {
                Destroy(buffWindow.BuffWindowIns);
            }
        });
        
        WindowManager.FullMapWindow
            .OnAdd(ShowFullScreenMap)
            .OnRemove(ShowMinimap);
        WindowManager.SleepWindow
            .OnAdd(() => SleepPnl.SetActive(true))
            .OnRemove(() => SleepPnl.SetActive(false));
        WindowManager.DrawWindow
            .OnAdd(() => DrawPnl.SetActive(true))
            .OnRemove(() => DrawPnl.SetActive(false));
        WindowManager.VioleTWindow
            .OnAdd(() =>
            {
                RefreshDic();
                VioleTBtn.interactable = false;
                VioleTPnl.SetActive(true);
            })
            .OnRemove(() =>
            {
                VioleTBtn.interactable = true;
                VioleTPnl.SetActive(false);
            });
        WindowManager.VioleTWindow.GetWord = () => VioleTTxt.text;
        WindowManager.DicWindow
            .OnAdd(() =>
            {
                DicScrollPnl.SetActive(true);
            })
            .OnRemove(() =>
            {
                DicScrollPnl.SetActive(false);
            });
        WindowManager.WinWindow.OnAddEventWithArg += async void (w) =>
        {
            try
            {
                AudioMono.PlayWinLoop();
                WinWordTxt.text = w.GetWord();
                PauseOrWinPnl.SetActive(true);
                WinPnl.SetActive(true);
                await FadeIn(PauseOrWinPnl.GetComponent<Image>(), 2f);
                WinPnlL1.SetActive(true);
                await Task.Delay(1000);
                WinPnlL2.SetActive(true);
                await Task.Delay(800);
                WinPnlL3.SetActive(true);
                await Task.Delay(800);
                WinWordTxt.gameObject.SetActive(true);
                await Task.Delay(1000);
                WinPnlRight.SetActive(true);
                await Task.Delay(2000);
                RunOutPnl.SetActive(true);
                await Task.Delay(2000);
                WinCountPnl.SetActive(true);
                ReturnToTitleBtn.gameObject.SetActive(true);
            }
            catch (Exception e)
            {
                MyDebug.LogError(e);
                throw;
            }
        };
        WindowManager.WinWindow.OnRemove(() =>
            {
                PauseOrWinPnl.SetActive(false);
                WinPnl.SetActive(false);
                WinPnlL1.SetActive(false);
                WinPnlL2.SetActive(false);
                WinPnlL3.SetActive(false);
                WinWordTxt.gameObject.SetActive(false);
                WinPnlRight.SetActive(false);
                RunOutPnl.SetActive(false);
                WinCountPnl.SetActive(false);
                ReturnToTitleBtn.gameObject.SetActive(false);
            });
            
        WindowManager.PauseWindow
            .OnAdd(() =>
            {
                PauseOrWinPnl.GetComponent<Image>().color = PauseOrWinPnl.GetComponent<Image>().color.SetAlpha(0.9f);
                PauseOrWinPnl.SetActive(true);
                PausePnl.SetActive(true);
                ReturnToTitleBtn.gameObject.SetActive(true);
            })
            .OnRemoveWithArg(w =>
            {
                PauseOrWinPnl.SetActive(false);
                PausePnl.SetActive(false);
                ReturnToTitleBtn.gameObject.SetActive(false);
                if(w.TarState == GameState.TitleState)
                    GameState.EnterTitle();
            });
        WindowManager.WatchingClockWindow
            .OnAdd(() => CameraMono.SceneItemVirtualCamera.gameObject.SetActive(true))
            .OnRemove(() =>
            {
                CameraMono.SceneItemVirtualCamera.gameObject.SetActive(false);
                ExitWatchingItemBtn.gameObject.SetActive(false);
            });
        
        ScrambleView.ExchangeWindow.OnExchangeEnd += RefreshDic;

        Binder.From(MainItemMono.WinCount).To(v => WinCountTxt.text = v.ToString());
        Binder.From(MapManager.DoorCount).To(v => DoorCountTxt.text = v.ToString());
        
        Binder.From(PlayerMono.InteractInfo).To(info => {
            NormalReticle.SetActive(GameState.IsPlaying && info == null);
            FindReticle.SetActive(info != null);
            SceneItemInfoPnl.SetActive(info != null);
            SceneItemInfoTxt.text = info?.Description ?? "";
            SceneItemInfoTxt.color = info?.Color ?? Color.black;
        }).Immediate();
        PlayerMono.OnClickInteract += info =>
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
        
        GameState.GeneratingMapState
            .OnEnter(() => LoadPnl.SetActive(true))
            .OnExit(() => LoadPnl.SetActive(false));
        GameState.PlayingState
            .OnEnter(() =>
            {
                MiniItemPnl.SetActive(true);
                EnableMapObj();
                ShowMinimap();
            })
            .OnUpdate(dt =>
            {
                StaminaTxt.text = MainItemMono.StaminaCount.ToString();
                EnergyTxt.text = MainItemMono.EnergyCount.ToString();
                CreativityTxt.text = MainItemMono.CreativityCount.ToString();
                // VioleeTxt.text = MainItemMono.VioleeCount.ToString();
                RedrawBtn.interactable = MainItemMono.CreativityCount >= MainItemMono.CheckCreativityCost(1);
                RedrawCostTxt.text = MainItemMono.CheckCreativityCost(1).ToString();
                ChangeFOV(dt);
                
                if (Input.GetKeyDown(KeyCode.Tab) && !WindowManager.HasPaused)
                {
                    if (isMinimap)
                        WindowManager.WindowList.MyAdd(WindowManager.FullMapWindow);
                    else if(!isMinimap)
                        WindowManager.WindowList.MyRemove(WindowManager.FullMapWindow);
                }
            })
            .OnExit(() =>
            {
                DisableMapObj();
                MiniItemPnl.SetActive(false);
            });
        

        var conBuffInsDic = new Dictionary<BuffData, GameObject>();
        BuffManager.OnAddConBuff += conBuff =>
        {
            // MyDebug.Log("GameView OnAddConBuff " + conBuff.ConBuffType);
            var conBuffIns = Instantiate(ConsistentBuffPrefab, ConsistentBuffIconPnl.transform);
            conBuffIns.Image.sprite = Configer.ConBuffConfigList.BuffConfigDic[conBuff.ConBuffType].Sprite;
            conBuffIns.DetailTxt.text = conBuff.Des;
            conBuffIns.DetailPnlShown = Instantiate(conBuffIns.DetailPnl, ConsistentBuffDetailPnl);
            conBuffIns.OnPointerEnterEvt += () =>
            {
                conBuffIns.DetailPnlShown.transform.position = conBuffIns.DetailPnl.transform.position;
                conBuffIns.DetailPnlShown.transform.SetAsLastSibling();
                conBuffIns.DetailPnlShown.GetComponent<RectTransform>().sizeDelta
                    = conBuffIns.DetailPnlShown.GetComponent<RectTransform>().sizeDelta;
            };
            conBuffIns.gameObject.SetActive(true);
            conBuffInsDic.Add(conBuff, conBuffIns.gameObject);
        };

        BuffManager.OnRemoveConBuff += conBuff =>
        {
            Destroy(conBuffInsDic[conBuff]);
            conBuffInsDic.Remove(conBuff);
        };
        MainItemMono.OnChangeVioleT += cList =>
        {
            VioleTTxt.text = string.Join("", cList);
            VioleTInfoTxt.gameObject.SetActive(cList.Count == 0);
        };
        MainItemMono.OnGainVioleT += OnGainVioleT;
        AudioMono.OnUnPauseLoop += clip =>
        {
            MusicWindow.BGMTxt.text = clip.name;
            MusicWindow.gameObject.SetActive(false);
            MusicWindow.gameObject.SetActive(true);
        };

        
        Binder.From(MinimapBtn).To(() => WindowManager.WindowList.MyAdd(WindowManager.FullMapWindow));
        Binder.From(RedrawBtn).To(() =>
        {
            MainItemMono.CostCreativity(MainItemMono.CheckCreativityCost(1));
            showDrawConfigsAct();
        });
        Binder.From(ContinueBtn).To(() =>
        {
            WindowManager.PauseWindow.TarState = GameState.PlayingState;
            WindowManager.WindowList.MyRemove(WindowManager.PauseWindow);
        });
        Binder.From(ReturnToTitleBtn).To(() =>
        {
            WindowManager.PauseWindow.TarState = GameState.TitleState;
            WindowManager.WindowList.MyRemove(WindowManager.PauseWindow);
            WindowManager.WindowList.MyRemove(WindowManager.WinWindow);
        });
        Binder.From(ExitWatchingItemBtn).To(async () =>
        {
            try
            {
                ExitWatchingItemBtn.gameObject.SetActive(false);
                CameraMono.SceneItemVirtualCamera.gameObject.SetActive(false);
                await Task.Delay(CameraMono.PlayerEase);
                WindowManager.WindowList.MyRemove(WindowManager.WatchingClockWindow);
            }
            catch (Exception e)
            {
                MyDebug.LogError(e);
                throw;
            }
        });
        Binder.From(VioleTBtn).To(() => WindowManager.WindowList.MyAdd(WindowManager.VioleTWindow));
        Binder.From(VioleTPnlCloseBtn).To(() => WindowManager.WindowList.MyRemove(WindowManager.VioleTWindow));
        Binder.From(DicScrollOpenBtn).To(() => WindowManager.WindowList.MyAdd(WindowManager.DicWindow));
        Binder.From(DicScrollCloseBtn).To(() => WindowManager.WindowList.MyRemove(WindowManager.DicWindow));

        Binder.From(ResetWinCountBtn).To(() => MainItemMono.WinCount.Value = 0);
        InitDic();
    }

    [Header("Load & Pause")]
    public required GameObject LoadPnl;
    public required GameObject PauseOrWinPnl;
    public required GameObject PausePnl;
    public required GameObject WinPnl;
    public required Button ContinueBtn;
    public required Button ReturnToTitleBtn;
    public required GameObject WinPnlL1;
    public required GameObject WinPnlL2;
    public required GameObject WinPnlL3;
    public required Text WinWordTxt;
    public required GameObject WinPnlRight;
    public required GameObject RunOutPnl;
    public required GameObject WinCountPnl;
    public required Text WinCountTxt;
    public required Button ResetWinCountBtn;
    
    
    #region Minimap

    [Header("Minimap")]
    public required RenderTexture TarTexture;
    public required CinemachineVirtualCamera MinimapCameraVirtual;
    public required Camera MinimapCamera;
    public required RawImage MinimapImg;
    public required RawImage FullScreenImg;
    public required Button MinimapBtn; 
    public float ChangeSpeed = 1.2f;
    public float MiniSize = 12f;
    public float FullSizeMulti = 1f;
    bool isMinimap => MinimapImg.enabled;
    float maxSize => Mathf.Max(Configer.BoxConfigList.Width, Configer.BoxConfigList.Height) * BoxHelper.BoxSize;
    void ChangeFOV(float dt)
    {
        
        var tarSize = isMinimap ? MiniSize : maxSize * FullSizeMulti;
        if (!Mathf.Approximately(MinimapCameraVirtual.m_Lens.OrthographicSize, tarSize))
        {
            MinimapCameraVirtual.m_Lens.OrthographicSize = Mathf.Lerp(MinimapCameraVirtual.m_Lens.OrthographicSize,
                tarSize,
                ChangeSpeed * dt);
        }
    }

    void EnableMapObj()
    {
        FullScreenImg.gameObject.SetActive(true);
        MinimapImg.gameObject.SetActive(true);
    }
    void DisableMapObj()
    {
        FullScreenImg.gameObject.SetActive(false);
        MinimapImg.gameObject.SetActive(false);
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
    public required Text CreativityTxt;
    // public required Text VioleeTxt;
    public required GameObject VioleTPnl;
    public required Button VioleTBtn;
    public required Text VioleTTxt;
    public required Text VioleTInfoTxt;
    public required Button VioleTPnlCloseBtn;
    public required Text DoorCountTxt;
    #endregion

    
    #region ConsistentBuff
    [Header("ConsistentBuff")] 
    public required Transform ConsistentBuffIconPnl;
    public required Transform ConsistentBuffDetailPnl;
    public required ConsistentBuffIcon ConsistentBuffPrefab;
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
    public required Text RedrawCostTxt;
    public required Transform DrawBtnContent;

    public required Button ExitWatchingItemBtn;
    
    
    #region OverlapWindow
    [Header("OverlapWindow")]
    public required Transform OverlapWindowTransform;
    public required BuffWindow BuffWindowPrefab;
    public required MusicWindow MusicWindow;
    public required LetterIcon LetterIcon;
    
    void OnGainVioleT(char c)
    {
        VioleTInfoTxt.gameObject.SetActive(false);
        LetterIcon.gameObject.SetActive(true);
        LetterIcon.Letter.text = c.ToString();
        LetterIcon.OnComplete = () => VioleTTxt.text += c;
    }
    
    #endregion
    
    
    Action showDrawConfigsAct = () => { };
    async Task GetUICb(InteractInfo info)
    {
        try
        {
            if (info is SceneItemInteractInfo itemInfo)
            {
                if (itemInfo.SceneItemData.IsSleep)
                {
                    WindowManager.WindowList.MyAdd(WindowManager.SleepWindow);
                    await FadeIn(SleepPnl.GetComponent<Image>(), itemInfo.SceneItemData.SleepTime / 2);
                    await FadeOut(SleepPnl.GetComponent<Image>(), itemInfo.SceneItemData.SleepTime / 2);
                    WindowManager.WindowList.MyRemove(WindowManager.SleepWindow);
                }
                    
                if (itemInfo.SceneItemData.HasCamera)
                {
                    WindowManager.WindowList.MyAdd(WindowManager.WatchingClockWindow);
                    await Task.Delay(CameraMono.SceneItemEase);
                    ExitWatchingItemBtn.gameObject.SetActive(true);
                }
            }
            else if (info is DoorInteractInfo doorInfo)
            {
                WindowManager.WindowList.MyAdd(WindowManager.DrawWindow);
                WindowManager.WindowList.MyAdd(WindowManager.FullMapWindow);
                DrawBtnContent.DisableAllChildren();
                showDrawConfigsAct = () =>
                {
                    var configs = doorInfo.GetDrawConfigs() ?? [];
                    for (int i = 0; i < configs.Count; i++)
                    {
                        var config = configs[i];
                        RoomIcon roomIcon = DrawBtnContent.GetChild(i).GetComponent<RoomIcon>();
                        roomIcon.RoomImg.sprite = config.Sprite;
                        roomIcon.TitleTxt.text = config.DrawTitle;
                        roomIcon.DesPnl.SetActive(false);
                        roomIcon.DesTxt.text = config.DrawDes;
                        roomIcon.Btn.onClick.RemoveAllListeners();
                        roomIcon.Btn.onClick.AddListener(() =>
                        {
                            MapManager.DrawAtWall(doorInfo.InsidePointDataList, doorInfo.WallData, config);
                            WindowManager.WindowList.MyRemove(WindowManager.DrawWindow);
                            WindowManager.WindowList.MyRemove(WindowManager.FullMapWindow);
                        });
                        roomIcon.gameObject.SetActive(true);
                    }
                };
                showDrawConfigsAct();
            }
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }

    static async Task FadeIn(Image img, float duration)
    {
        try
        {
            var initAlpha = 0;
            var half = duration;
            for (float t = 0; t < half; t += Time.deltaTime)
            {
                var norm = t / half;
                var eased = Mathf.SmoothStep(0, 1, norm);
                var c = img.color;
                c.a = Mathf.Lerp(initAlpha, 1, eased);
                img.color = c;
                await Task.Yield();
            }
            img.color.SetAlpha(1f);
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }

    public async Task FadeOut(Image img, float duration)
    {
        try
        {
            var initAlpha = 1;
            var half = duration;
            for (float t = 0; t < half; t += Time.deltaTime)
            {
                if (img == null)
                    return;
                var norm = t / half;
                var eased = Mathf.SmoothStep(0, 1, norm);
                var c = img.color;
                c.a = Mathf.Lerp(0, initAlpha, 1 - eased);
                img.color = c;
                await Task.Yield();
            }
            img.color.SetAlpha(0);
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }
    #endregion


    #region Scramble
    [Header("Scramble")]
    public required GameObject DicScrollPnl;
    public required Button DicScrollOpenBtn;
    public required Button DicScrollCloseBtn;
    public required Transform DicScrollContent;
    public required WordLine WordLinePrefab;
    public required GameObject RedPoint;
    
    static readonly List<WordLine> wordLineList = [];

    void InitDic()
    {
        Configer.DicConfig.WordList.ForEach(info =>
        {
            var wordIns = Instantiate(WordLinePrefab, DicScrollContent);
            wordIns.InitWithWord(info.Word);
            wordIns.OnWin += Win;
            wordIns.gameObject.SetActive(true);
            wordLineList.Add(wordIns);
        });
    }

    void RefreshDic()
    {
        var anyFit = false;
        wordLineList.ForEach(w =>
        {
            anyFit |= w.RefreshGottenLetter(WindowManager.VioleTWindow.GetWord());
        });
        RedPoint.SetActive(anyFit);
    }

    void Win(string word)
    {
        MainItemMono.WinCount.Value++;
        WindowManager.WinWindow.GetWord = () => word;
        GameState.EnterWinning();
    }
    #endregion
}