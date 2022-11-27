using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态抽象类
/// </summary>
public abstract class FSMState
{
    // 所属的状态机
    protected FSMSystem m_FSM = null;

    protected FSMState(FSMSystem fsm)
    {
        m_FSM = fsm;
    }

    /// <summary>
    /// 进入状态
    /// </summary>
    public virtual void OnEnter() { }
    /// <summary>
    /// 状态中进行的动作
    /// </summary>
    public abstract void Action();
    /// <summary>
    /// 检测状态转换
    /// </summary>
    public abstract void Check();
    /// <summary>
    /// 退出状态
    /// </summary>
    public virtual void OnExit() { }
}

/// <summary>
/// 状态ID
/// </summary>
public enum StateID
{
    Move,   // 移动
    Rotate  // 旋转
}