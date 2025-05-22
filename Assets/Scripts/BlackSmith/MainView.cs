
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
        Binder.From(MainModel.CurMine).ToTxt(TxtMine).Immediate();
        
        Binder.From(MainModel.CurMineData.Count).ToTxt(TxtMineCount).Format("F0").Immediate().Fluent(10);
        
        Binder.From(MainModel.CurMineData.Progress)
            .ToImg(ImgMineFill, (v) => v * 1f / Configer.MainConfig.MineCostDic[MainModel.CurMine]).Immediate().Fluent(8f);
    }
    
}

    
}