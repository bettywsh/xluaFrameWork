using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimationManager : Singleton<AnimationManager>
{
    public void PlayAnimation(string animName, Transform transform, Action onComplete = null)
    {
        Animation animation = transform.GetComponent<Animation>();
        if(animation == null)
        {
            UnityEngine.Debug.LogWarning($"try play:{animName} but no animation component!");
            return;
        }
        if (animation[animName] != null)
        {
            bool isPlay = animation.Play(animName);
            if (isPlay)
            {
                float delay = animation.GetClip(animName).length;
                TimerManager.Instance.SetTimer(animName, delay, () =>
                {
                        onComplete?.Invoke();
                });
            }
            else
            {
                onComplete?.Invoke();
            }
        }
        else
        {
            Debug.LogWarning(string.Format("播放动画[{1}]失败: 未能找到对应的AnimationClip",animName));
            onComplete?.Invoke();
        }
    }
}
