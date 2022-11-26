using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Sweet : MonoBehaviour
{
    void Start()
    {

    }
    void Do()
    {
        
    }

    #region Animation动画操作
    /// <summary>
    /// 添加Animation动画播放完成回调
    /// </summary>
    /// <param name="onCompleted"></param>
    public void AddAnimationListener(Action onCompleted)
    {
        AddAnimationListener(GetComponent<Animation>(), onCompleted);
    }
    /// <summary>
    /// 添加Animation动画播放完成回调
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="onCompleted"></param>
    public void AddAnimationListener(Animation anim, Action onCompleted)
    {
        StartCoroutine(_OnAnimationUpdate(anim, onCompleted));
    }
    private IEnumerator _OnAnimationUpdate(Animation anim, Action onCompleted)
    {
        while (anim && anim.isPlaying)
        {
            yield return null;
        }

        if (onCompleted != null)
            onCompleted();
    }
    #endregion

    #region ParticleSystem动画操作
    /// <summary>
    /// 添加ParticleSystem动画播放完成回调
    /// </summary>
    /// <param name="onCompleted"></param>
    public void AddParticleSystemListener(Action onCompleted)
    {
        AddParticleSystemListener(GetComponent<ParticleSystem>(), onCompleted);
    }
    /// <summary>
    /// 添加ParticleSystem动画播放完成回调
    /// </summary>
    /// <param name="particleSystem"></param>
    /// <param name="onCompleted"></param>
    public void AddParticleSystemListener(ParticleSystem particleSystem, Action onCompleted)
    {
        StartCoroutine(_OnParticleSystemUpdate(particleSystem, onCompleted));
    }
    private IEnumerator _OnParticleSystemUpdate(ParticleSystem particleSystem, Action onCompleted)
    {
        while (particleSystem && particleSystem.isPlaying)
        {
            yield return null;
        }

        if (onCompleted != null)
            onCompleted();
    }
    #endregion
}
