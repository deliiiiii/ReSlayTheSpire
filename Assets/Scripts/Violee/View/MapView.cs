using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View;

class MapView : ViewBase<MapView>
{
    [Header("Title")]
    
    public required Light Light;
    public required GameObject TitlePnl;
    public required GameObject StartBox;
    public required InteractCasterTitle TitleCaster;
    public required Transform Hitobj;
    public required InteractReceiver StartIr;

    [Header("FirstBtnPnl")] 
    public required GameObject FirstBtnPnl;
    public required Button TitleBtn;
    public required Button TutorialBtn;
    public required Button SettingsBtn;
    public required Button ExitBtn;

    [Header("Second - SettingsPnl")]
    public required GameObject SettingsPnl;
    public required Button QuickKeyBtn;
    public required Text QuickKeyTxt;
    public required Button ShowBoxCostBtn;
    public required Text ShowBoxCostTxt;
    // public required Button DisablePauseBtn;
    // public required Text DisablePauseTxt;
    public required Button DreamCatcherGachaBtn;
    public required Text DreamCatcherGachaTxt;
    public required Button AddTiltWallBtn;
    public required Text AddTiltWallTxt;
    public required Button UseSmallMapBtn;
    public required Text UseSmallMapTxt;
    
    public required Button ReturnBtn;

    [Header("Input Pnl")]
    public required GameObject DevelopKeyPnl;
    
    [Header("Map")]
    public Text CostTxtPrefab = null!;
    
    public static void Init() => Instance.IBL();
    protected override void IBL()
    {
        costTxtPool = new ObjectPool<Text>(CostTxtPrefab, transform, 42);
        
        MapManager.DijkstraStream.OnBeginAsync(BindAllCostTxt);

        StartIr.GetInteractInfo = () =>
        {
            return new StartBoxInteractInfo
            {
                Description = "点击开始",
                Color = Color.cyan,
                Act = () => Task.FromResult(MapManager.GenerateStream.CallTriggerAsync()),
            };
        };

        // TODO DelayTickMouse is not used
        // async Task DelayTickMouse()
        // {
        //     await Task.Delay(CameraMono.DefaultEase);
        //     TitleCaster.gameObject.SetActive(true);
        // }
        
        // TODO BindState is deleted
        // GameState.TitleState
        //     .OnEnter(() =>
        //     {
        //         CameraMono.TitleVirtualCamera.LookAt = Hitobj;
        //         CameraMono.PlayerVirtualCamera.enabled = false;
        //         TitlePnl.SetActive(true);
        //         StartBox.SetActive(true);
        //         Light.gameObject.SetActive(false);
        //         _ = DelayTickMouse();
        //     })
        //     .OnUpdate(_ =>
        //     {
        //         if(TitleCaster.gameObject.activeSelf)
        //             TitleCaster.Tick();
        //         QuickKeyTxt.color = Configer.SettingsConfig.QuickKey ? Color.blue : Color.white;
        //         ShowBoxCostTxt.color = Configer.SettingsConfig.ShowBoxCost ? Color.blue : Color.white;
        //         // DisablePauseTxt.color = Configer.SettingsConfig.DisablePause ? Color.blue : Color.white;
        //         DreamCatcherGachaTxt.color = Configer.SettingsConfig.DreamCatcherGachaUp ? Color.blue : Color.white;
        //         AddTiltWallTxt.color = Configer.SettingsConfig.AddTiltWall ? Color.blue : Color.white;
        //         UseSmallMapTxt.color = Configer.SettingsConfig.UseSmallMap ? Color.blue : Color.white;
        //     })
        //     .OnExit(() =>
        //     {
        //         CameraMono.TitleVirtualCamera.LookAt = null;
        //         TitlePnl.SetActive(false);
        //         StartBox.SetActive(false);
        //         Light.gameObject.SetActive(true);
        //         TitleCaster.gameObject.SetActive(false);
        //     });
        //
        // GameState.PlayingState.OnEnter(() =>
        // {
        //     CameraMono.PlayerVirtualCamera.enabled = true;
        // });
        
        
        WindowManager.DrawWindow
            .OnAdd(() =>
            {
                var doorInteractInfo = PlayerMono.InteractInfo.Value as DoorInteractInfo;
                doorInteractInfo?.InsidePointDataList.ForEach(x =>
                {
                    x.Visit();
                    x.Flash(true);
                });
            })
            .OnRemove(() =>
            {
                var doorInteractInfo = PlayerMono.InteractInfo.Value as DoorInteractInfo;
                doorInteractInfo?.InsidePointDataList.ForEach(x => x.Flash(false));
            });
        
        TitleBtn.OnClickAsObservable()
            .TimeInterval()
            .Scan(0, (count, time) =>                               
                    time.Interval.TotalSeconds <= 0.2f
                        ? count + 1
                        : 1
            )
            .Where(count => count == 10)
            .Subscribe(_ =>
            {
                var buffAct = () => { Configer.SettingsConfig.IsDevelop = true; };
                buffAct.Invoke();
                BuffManager.AddWinBuff("Oh It's 萝符号!\n(已启用开发者模式.)", buffAct);
                SettingsBtn.gameObject.SetActive(true);
                DevelopKeyPnl.gameObject.SetActive(true);
                OpenSettingsPnl();
            })
            .AddTo(this);

        Binder.FromBtn(QuickKeyBtn).To(() => Configer.SettingsConfig.ReverseQuickKey());
        Binder.FromBtn(ShowBoxCostBtn).To(() => Configer.SettingsConfig.ReverseShowBoxCost());
        // Binder.From(DisablePauseBtn).To(() => Configer.SettingsConfig.ReverseDisablePause());
        Binder.FromBtn(DreamCatcherGachaBtn).To(() => Configer.SettingsConfig.ReverseDreamCatcherGachaUp());
        Binder.FromBtn(AddTiltWallBtn).To(() => Configer.SettingsConfig.ReverseAddTiltWall());
        Binder.FromBtn(UseSmallMapBtn).To(() => Configer.SettingsConfig.ReverseUseSmallMap());
        
        
        Binder.FromBtn(SettingsBtn).To(OpenSettingsPnl);
        // QuickKeyTg.onValueChanged.AddListener(Configer.SettingsConfig.SetQuickKey);
        // ShowBoxCostTg.onValueChanged.AddListener(Configer.SettingsConfig.SetShowBoxCost);
        // DisablePauseTg.onValueChanged.AddListener(Configer.SettingsConfig.SetDisablePause);
        // DreamCatcherGachaTg.onValueChanged.AddListener(Configer.SettingsConfig.SetDreamCatcherGachaUp);
        
        Binder.FromBtn(ReturnBtn).To(() =>
        {
            FirstBtnPnl.gameObject.SetActive(true);
            SettingsPnl.gameObject.SetActive(false);
        });
        Binder.FromBtn(ExitBtn).To(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }

    void OpenSettingsPnl()
    {
        FirstBtnPnl.gameObject.SetActive(false);
        SettingsPnl.gameObject.SetActive(true);
    }

    readonly HashSet<Text> costTxtSet = [];
    ObjectPool<Text> costTxtPool = null!;
    async Task DestroyAllCostTxt()
    {
        foreach (var text in costTxtSet)
        {
            costTxtPool.MyDestroy(text);
            await Configer.SettingsConfig.YieldFrames(multi : 1 / 16f);
        }
    }
    async Task BindAllCostTxt(GenerateParam param)
    {
        
        try
        {
            await DestroyAllCostTxt();
            if (!Configer.SettingsConfig.ShowBoxCost)
                return;
            var boxKList = param.BoxDataDic.Values;
            var allPoints = boxKList.SelectMany(x => x.PointDataMyDic.Values).ToList();
            foreach (var point in allPoints)
            {
                var txt = await costTxtPool.MyInstantiate(point.Pos3D + Vector3.up * 0.1f);
                txt.gameObject.SetActive(true);
                var b = Binder.FromObs(point.CostWall).To(v =>
                {
                    txt.text = v > 1e9 ? "∞" : point.CostWall.ToString();
                });
                b.Immediate();
                costTxtSet.Add(txt);
                await Configer.SettingsConfig.YieldFrames(multi : 1 / 8f);
            }
        }
        catch (Exception e)
        {
            MyDebug.LogError(e);
            throw;
        }
    }
}