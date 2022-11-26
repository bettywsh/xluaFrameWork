using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 有限状态机系统
/// </summary>
public class FSMSystem
{
    /// <summary>
    /// 状态机所属游戏物体
    /// </summary>
    public GameObject OwnerGo { get; private set; }

    // 用字典存储每个状态ID对应的状态
    private Dictionary<StateID, FSMState> m_StateMap = new Dictionary<StateID, FSMState>();

    public FSMSystem(GameObject ownerGo)
    {
        OwnerGo = ownerGo;
    }

    /// <summary>
    /// 当前状态ID
    /// </summary>
    public StateID CurrentStateID { get; private set; }

    /// <summary>
    /// 当前状态
    /// </summary>
    public FSMState CurrentState { get; private set; }

    /// <summary>
    /// 添加状态
    /// </summary>
    /// <param name="id">添加的状态ID</param>
    /// <param name="state">对应的状态对象</param>
    public void AddState(StateID id, FSMState state)
    {
        // 如果当前状态为空，就设置为默认状态
        if (CurrentState == null)
        {
            CurrentStateID = id;
            CurrentState = state;
        }
        if (m_StateMap.ContainsKey(id))
        {
            Debug.LogErrorFormat("状态ID:{0}已经存在，不能重复添加！", id);
            return;
        }
        m_StateMap.Add(id, state);
    }

    /// <summary>
    /// 移除状态
    /// </summary>
    /// <param name="id">要移除的状态ID</param>
    public void RemoveState(StateID id)
    {
        if (!m_StateMap.ContainsKey(id))
        {
            Debug.LogWarningFormat("状态ID:{0}不存在，不需要移除", id);
            return;
        }
        m_StateMap.Remove(id);
    }

    /// <summary>
    /// 改变状态
    /// </summary>
    /// <param name="id">需要转换到的目标状态ID</param>
    public void ChangeState(StateID id)
    {
        if (id == CurrentStateID) return;
        if (!m_StateMap.ContainsKey(id))
        {
            Debug.LogErrorFormat("状态ID:{0}不存在！", id);
            return;
        }
        if (CurrentState != null)
            CurrentState.OnExit();
        CurrentStateID = id;
        CurrentState = m_StateMap[id];
        CurrentState.OnEnter();
    }

    /// <summary>
    /// 更新，在状态机持有者物体的Update中调用
    /// </summary>
    public void Update()
    {
        CurrentState.Action();
        CurrentState.Check();
    }
}