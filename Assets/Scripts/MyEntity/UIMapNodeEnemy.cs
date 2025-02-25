using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIMapNodeEnemy : UIMapNodeBase
{
    [SerializeField]Dropdown ddEnemy;
    string[] enemyTypes;


    //dropdown选中时发送事件
    public void Init()
    {
        ddEnemy.onValueChanged.AddListener((value)=>
        {
            MyCommand.Send(new OnSetNextRoomEnemyCommand(){EnemyType = enemyTypes[value]});
        });
        BtnEnter.onClick.AddListener(()=>
        {
            MyCommand.Send(new OnEnterNextRoomBattleCommand());
        });
    }
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

