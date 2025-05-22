
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace BlackSmith
{

public class MainView : ViewBase
{
    public Text TxtCoin;
    
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
        Binder.From(MainModel.Coin).ToTxt(TxtCoin).Format("F0").Immediate().Fluent(20f);
        
        Binder.From(BtnMine).To(MainModel.OnClickBtnMine);
        Binder.From(MainModel.CurMine).ToTxt(TxtMine).Immediate();
        
        Binder.From(MainModel.CurMineData.Count).ToTxt(TxtMineCount).Format("F0").Immediate().Fluent(10);
        Binder.From(MainModel.CurMineData.Progress).ToImg(ImgMineFill, (v) => v * 1f / Configer.MainConfig.MineCostDic[MainModel.CurMine]).Immediate().Fluent(8f);

        Binder.From(MainModel.CurMineData.Count).To((v) =>
        {
            BtnWeapon.interactable = v >= (int)MainModel.CurWeapon.Value;
        }).Immediate();
        Binder.From(BtnWeapon).To(MainModel.OnClickBtnWeapon);
        Binder.From(MainModel.CurWeapon).ToTxt(TxtWeapon).Immediate();
        Binder.From(MainModel.CurWeaponData.Count).ToTxt(TxtWeaponCount).Format("F0").Immediate().Fluent(10);
        Binder.From(MainModel.CurWeaponData.Progress).ToImg(ImgWeaponFill, (v) => v * 1f / Configer.MainConfig.WeaponCostDic[MainModel.CurMine][MainModel.CurWeapon]).Immediate().Fluent(8f);
        
        Binder.From(MainModel.CurWeaponData.Count).To((v) =>
        {
            BtnEnchant.interactable = v >= (int)MainModel.CurEnchant.Value;
        }).Immediate();
        Binder.From(BtnEnchant).To(MainModel.OnClickBtnEnchant);
        Binder.From(MainModel.CurEnchant).ToTxt(TxtEnchant).Immediate();
        Binder.From(MainModel.CurEnchant).To((_) => TxtEnchantPrice.text = Configer.MainConfig.EnchantPriceDic[MainModel.CurMine][MainModel.CurWeapon][MainModel.CurEnchant].ToString("F0")).Immediate();
        Binder.From(MainModel.CurEnchantData.Progress).ToImg(ImgEnchantFill, (v) => v * 1f / Configer.MainConfig.EnchantCostDic[MainModel.CurMine][MainModel.CurWeapon][MainModel.CurEnchant]).Immediate().Fluent(8f);
    }
    
}

    
}