
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace BlackSmith
{

public class MainView : ViewBase
{
    public Button BtnMine;
    public Text TxtMine;
    public Text TxtMineCount;
    public Image ImgMineFill;


    public Text TxtMineOfWeapon;
    public Button BtnWeapon;
    public Text TxtWeapon;
    public Text TxtWeaponCount;
    public Image ImgWeaponFill;

    public Text TxtMineOfWeaponOfEnchant;
    public Button BtnEnchant;
    public Text TxtEnchant;
    public Text TxtEnchantPrice;
    public Image ImgEnchantFill;
    
    
    
    public override void IBL()
    {
        MainModel.InitData();
        Bind(); 
    }

    void Bind()
    {
        Binder.From(BtnMine).To(MainModel.OnClickBtnMine);
        Binder.From(MainModel.MainData.CurMine).ToTxt(TxtMine).Immediate();
        
        var b = Binder.From(MainModel.MainData.CurMineData.Count).ToTxt(TxtMineCount).Format("F0");
        b.Immediate();
        b.Fluent(10);
        
        var b2 = Binder.From(MainModel.MainData.CurMineData.Progress)
            .ToImg(ImgMineFill, (v) => v * 1f / Configer.Instance.MainConfig.MineCostDic[MainModel.MainData.CurMine]);
        b2.Immediate();
        b2.Fluent(1.5f);
    }
    
}

    
}