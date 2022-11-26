using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XLua;

[LuaCallCSharp]

public class TimerManager : MonoSingleton<TimerManager>
{

    public int ServerTimer
    {
        get {
            return mServerTimer + (int)(Time.realtimeSinceStartup - validStartGameTime); }
    }
    public int mServerTimer = 0;
    public float validStartGameTime = 0;

    public delegate void OnChangeTime(int time);
    private static System.Random mRandom;
    /// <summary>
    /// 获取随机名称 通过在name后添加随机时间戳
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private string GetRandomName(string name)
    {
        if (mRandom == null)
        {
            mRandom = new System.Random(0);
        }
        System.Text.StringBuilder builder = new System.Text.StringBuilder(name);
        builder.Append(System.DateTime.Now.Ticks);
        builder.Append(mRandom.NextDouble());
        return builder.ToString();
    }
    protected class TimerUnit
    {
        public string mName = "";
        public float mSeconds = 0;
        public Action mCall;
        public Coroutine mCoroutine;
        public TimerUnit(float seconds, string name, Action call)
        {
            this.mSeconds = seconds;
            this.mName = name;
            this.mCall = call;
        }


        public void Dispose()
        {
            this.mCall = null;
            this.mCoroutine = null;
        }
    }

    #region 延时定时器
    private Dictionary<string, TimerUnit> mDictTimer = new Dictionary<string, TimerUnit>();

    /// <summary>
    /// 设置定时器 seconds后调用 call方法
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="name"></param>
    /// <param name="call"></param>
    public void SetTimer(string name, float seconds, Action call)
    {
        //if (mDictTimer.ContainsKey(name))
        //{
        //    Debug.LogError("错误: SetTimer 【重复name】=" + name);
        //    return;
        //}
        //需求 先取消上一个存在的定时器
        ClearTimer(name);

        TimerUnit unit = new TimerUnit(seconds, name, call);
        //启用协程
        unit.mCoroutine = StartCoroutine(_WaitForSeconds(unit));
        mDictTimer.Add(name, unit);
    }
    /// <summary>
    /// 设置定时器 seconds后调用 call方法
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="call"></param>
    public void SetTimer(float seconds, Action call)
    {
        SetTimer(GetRandomName("SetTimer_"), seconds, call);
    }
    /// <summary>
    /// 重置定时器
    /// </summary>
    /// <param name="name">指定定时器</param>
    /// <param name="seconds">0为使用原来的定时器时间</param>
    /// <param name="call">默认为原来的回调函数</param>
    public void ResetTimer(string name, float seconds = 0, Action call = null)
    {
        TimerUnit unit;
        if (!mDictTimer.TryGetValue(name, out unit))
        {
            Debug.LogError("错误: ResetTimer 【不存在name】=" + name);
            return;
        }
        StopCoroutine(unit.mCoroutine);
        if (seconds > 0)
        {
            unit.mSeconds = seconds;
        }
        if (call != null)
        {
            unit.mCall = call;
        }
        //启用协程
        unit.mCoroutine = StartCoroutine(_WaitForSeconds(unit));
    }
    /// <summary>
    /// 是否存在指定的定时器
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool HasTimer(string name)
    {
        return mDictTimer.ContainsKey(name);
    }
    /// <summary>
    /// 删除指定的延时定时器
    /// </summary>
    /// <param name="name"></param>
    public void ClearTimer(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        TimerUnit unit = null;
        if (mDictTimer.TryGetValue(name, out unit))
        {
            StopCoroutine(unit.mCoroutine);
            mDictTimer.Remove(name);
            unit.Dispose();
        }
    }
    /// <summary>
    /// 删除所有的定时器
    /// </summary>
    public void ClearTimers()
    {
        //第一种方式
        //List<TimerUnit> unitList = new List<TimerUnit>(mDictTimer.Values);
        //for (int i = 0; i < unitList.Count; ++i)
        //{
        //    ClearTimer(unitList[i].mName);
        //}

        //第二种方式
        foreach (KeyValuePair<string, TimerUnit> item in mDictTimer)
        {
            StopCoroutine(item.Value.mCoroutine);
            item.Value.Dispose();
        }
        mDictTimer.Clear();
    }
    /// <summary>
    /// 协程方法 等待n秒后运行
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    private IEnumerator _WaitForSeconds(TimerUnit unit)
    {
        yield return new WaitForSeconds(unit.mSeconds);

        Action call = unit.mCall;

        ClearTimer(unit.mName);
        call();
    }
    #endregion

    #region 间隔定时器
    private Dictionary<string, TimerUnit> mDictInterval = new Dictionary<string, TimerUnit>();

    /// <summary>
    /// 设置间隔器 每隔seconds秒后调用一次 call方法
    /// </summary>
    /// <param name="seconds">每隔seconds秒后调用一次</param>
    /// <param name="name"></param>
    /// <param name="call"></param>
    public void SetInterval(string name, float seconds, Action call)
    {
        if (mDictInterval.ContainsKey(name))
        {
            Debug.LogError("错误: SetInterval 【重复name】=" + name);
            return;
        }

        TimerUnit unit = new TimerUnit(seconds, name, call);
        //启用协程
        unit.mCoroutine = StartCoroutine(_WaitForInterval(unit));
        mDictInterval.Add(name, unit);
    }
    /// <summary>
    /// 设置间隔器 每隔seconds秒后调用一次 call方法
    /// </summary>
    /// <param name="seconds">每隔seconds秒后调用一次</param>
    /// <param name="call"></param>
    public void SetInterval(float seconds, Action call)
    {
        SetInterval(GetRandomName("SetInterval_"), seconds, call);
    }
    /// <summary>
    /// 重置间隔器
    /// </summary>
    /// <param name="name">指定间隔器</param>
    /// <param name="seconds">0为使用原来的定时器间隔</param>
    /// <param name="call">默认为原来的回调函数</param>
    public void ResetInterval(string name, float seconds = 0, Action call = null)
    {
        TimerUnit unit;
        if (!mDictInterval.TryGetValue(name, out unit))
        {
            Debug.LogError("错误: ResetInterval 【不存在 name】=" + name);
            return;
        }
        StopCoroutine(unit.mCoroutine);
        if (seconds > 0)
        {
            unit.mSeconds = seconds;
        }
        if (call != null)
        {
            unit.mCall = call;
        }
        //启用协程
        unit.mCoroutine = StartCoroutine(_WaitForInterval(unit));
    }
    /// <summary>
    /// 是否存在指定的间隔器
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool HasInterval(string name)
    {
        return mDictInterval.ContainsKey(name);
    }
    /// <summary>
    /// 删除指定的间隔器
    /// </summary>
    /// <param name="name"></param>
    public void ClearInterval(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        TimerUnit unit = null;
        if (mDictInterval.TryGetValue(name, out unit))
        {
            StopCoroutine(unit.mCoroutine);
            mDictInterval.Remove(name);
            unit.Dispose();
        }
    }
    /// <summary>
    /// 删除所有的间隔器
    /// </summary>
    public void ClearIntervals()
    {
        //第一种方式
        //List<TimerUnit> unitList = new List<TimerUnit>(mDictInterval.Values);
        //for (int i = 0; i < unitList.Count; ++i)
        //{
        //    ClearInterval(unitList[i].mName);
        //}

        //第二种方式
        foreach (KeyValuePair<string, TimerUnit> item in mDictInterval)
        {
            StopCoroutine(item.Value.mCoroutine);
            item.Value.Dispose();
        }
        mDictInterval.Clear();
    }
    /// <summary>
    /// 协程方法 间隔n秒后调用一次
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    private IEnumerator _WaitForInterval(TimerUnit unit)
    {
        while (true)
        {
            yield return new WaitForSeconds(unit.mSeconds);
            unit.mCall();
        }
    }
    #endregion

    #region 倒计时
    //基于 间隔定时器
    /// <summary>
    /// 设置倒计时
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="times">倒计时数</param>
    /// <param name="interval">间隔数</param>
    /// <param name="call">回调方法 会传递当前倒计时数</param>
    public void SetCountDown(string name, int times, int interval, Action<int> call)
    {
        //如果存在该倒计时 则删除 以便后面重新设置
        if (HasInterval(name))
        {
            ClearInterval(name);
        }

        call(times);
        SetInterval(name, interval, () =>
        {
            times -= interval;

            if (times <= 0)
            {
                ClearInterval(name);
            }

            call(times);
        });
    }
    public void SetCountDown(string name, int times, Action<int> call)
    {
        SetCountDown(name, times, 1, call);
    }
    #endregion

    #region 时间戳
    public void SetServerTimer(int time)
    {
        validStartGameTime = Time.realtimeSinceStartup;
        mServerTimer = time;
    }

    #endregion

    public class Timer
    {
        public float time;
        public float updateTime = 1;
        public OnChangeTime changeTime;
    }

}
