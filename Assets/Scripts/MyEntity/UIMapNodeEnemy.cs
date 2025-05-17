using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIMapNodeEnemy : UIMapNodeBase
{
    [SerializeField]Dropdown ddEnemy;
    string[] enemyTypes;


    // TODO dropdown选中时发送事件
    // public override void Bind()
    // {
    //     Binder.From(BtnEnter).SingleTo(BattleModel.EnterNextRoomBattle);
    // }
    public void SetEnemyType(string[] enemyTypes)
    {
        this.enemyTypes = enemyTypes;
        ddEnemy.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach(var enemyType in enemyTypes)
        {
            options.Add(new Dropdown.OptionData(enemyType));
        }
        ddEnemy.AddOptions(options);
    }
    public void SetCurSelectedEnemyType(string enemyType)
    {
        ddEnemy.value = Array.IndexOf(enemyTypes, enemyType);
    }
}

