using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableDirectorController : MonoBehaviour
{
    Action<PlayableDirector> Complete;
    PlayableDirector Playable;

    private void Awake()
    {
        Playable = GetComponent<PlayableDirector>();
    }
    public void OnComplete(Action<PlayableDirector> complete)
    {
        Complete = complete;
        Playable.stopped += PlayeComplete;
    }

    void PlayeComplete(PlayableDirector pb)
    {
        Complete(pb);
    }

    private void OnDestroy()
    {
        Playable.played -= PlayeComplete;
    }


}
