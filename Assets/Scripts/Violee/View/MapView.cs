using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public required InteractReceiver StartIr;
    public required Button TutorialBtn;
    public required Button SettingsBtn;
    public required Button ExitBtn;
    
    [Header("Map")]
    public Text CostTxtPrefab = null!;
    protected override void IBL()
    {
        costTxtPool = new ObjectPool<Text>(CostTxtPrefab, transform, 42);
        if (Configer.SettingsConfig.ShowBoxCost)
            MapManager.DijkstraStream.OnBeginAsync(BindAllCostTxt);

        MouseCaster.Init();
        StartIr.GetInteractInfo = () =>
        {
            return new StartBoxInteractInfo
            {
                Description = "点击开始",
                Color = Color.cyan,
                Act = () => Task.FromResult(MapManager.GenerateStream.CallTriggerAsync()),
            };
        };
        

        GameManager.TitleState
            .OnEnter(() =>
            {
                CameraMono.PlayerVirtualCamera.enabled = false;
                TitlePnl.SetActive(true);
                StartBox.SetActive(true);
                Light.gameObject.SetActive(false);
            })
            .OnExit(() =>
            {
                CameraMono.PlayerVirtualCamera.enabled = true;
                TitlePnl.SetActive(false);
                StartBox.SetActive(false);
                Light.gameObject.SetActive(true);
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

        GameManager.EnterTitle();
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