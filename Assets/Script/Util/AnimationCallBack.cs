using System;
using System.Collections;
using UnityEngine;

public class AnimationCallBack : MonoBehaviour
{
    Action animationCb;
    string clipName;

    public void Play(string clipName, Action animationCb)
    {
        this.clipName = clipName;
        this.animationCb = animationCb;
        StartCoroutine(PlayAndWaitForAnim());
    }
    IEnumerator PlayAndWaitForAnim()
    {
        Animation anim = GetComponent<Animation>();
        anim.Play(clipName);
        while (anim.IsPlaying(clipName))
        {
            yield return null;
        }
        if (this.animationCb != null)
        {
            this.animationCb();
        }
    }
}
