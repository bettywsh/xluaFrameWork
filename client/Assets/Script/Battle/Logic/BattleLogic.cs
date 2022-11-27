using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLogic: MonoBehaviour
{
    int curFrame = 0;
    float battleFrameTime = 0.066f;
    public void StartLogic()
    {
        //初始化玩家


        InvokeRepeating("UpdateFrame", 0, battleFrameTime);
    }

    void UpdateFrame()
    { 
    
    }

}
