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
    public required InteractCasterMouse MouseCaster;
    public required Transform Hitobj;
    public required InteractReceiver StartIr;
    public required Button TitleBtn;
    public required Button TutorialBtn;
    public required Button SettingsBtn;
    public required Button ExitBtn;
    
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

        async Task DelayTickMouse()
        {
            await Task.Delay(CameraMono.DefaultEase);
            MouseCaster.gameObject.SetActive(true);
        }
        
        GameManager.TitleState
            .OnEnter(() =>
            {
                CameraMono.TitleVirtualCamera.LookAt = Hitobj;
                CameraMono.PlayerVirtualCamera.enabled = false;
                TitlePnl.SetActive(true);
                StartBox.SetActive(true);
                Light.gameObject.SetActive(false);
                _ = DelayTickMouse();
            })
            .OnUpdate(_ =>
            {
                if(MouseCaster.gameObject.activeSelf)
                    MouseCaster.Tick();
            })
            .OnExit(() =>
            {
                CameraMono.TitleVirtualCamera.LookAt = null;
                CameraMono.PlayerVirtualCamera.enabled = true;
                TitlePnl.SetActive(false);
                StartBox.SetActive(false);
                Light.gameObject.SetActive(true);
                MouseCaster.gameObject.SetActive(false);
            });
        
        
        GameView.DrawWindow
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
                BuffManager.AddWinBuff("Oh It's 萝符号!\n(已启用开发者模式.)", buffAct);
            })
            .AddTo(this);
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
                var b = Binder.From(point.CostWall).To(v =>
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