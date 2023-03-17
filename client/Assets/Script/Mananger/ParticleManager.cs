using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{

    Dictionary<string, GameObject> GameObjectCache = new Dictionary<string, GameObject>();
    private Transform particleRoot;
    
    public void PlayParticle(string path, Transform tf, string duration, bool cache)
    { 
        // ResManager.Instance.LoadAssetAsync();
    }
    
    public void PlayParticle(string path, Vector3 Pos, string duration, bool cache)
    { 
    
    }
    
    private void CreateRoot()
    {
        if (particleRoot == null)
        {
            var rootObj = new GameObject("ParticleRoot");
            particleRoot = rootObj.transform;
        }
    }

}