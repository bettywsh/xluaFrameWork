using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResLoader
{
    private List<string> ResList = new List<string>();

    public void AddABName(string abName)
    {
        ResList.Add(abName);
    }

    public void Dispose()
    {
        for (int i = 0; i < ResList.Count; i++)
        {
            AssetBundleManager.Instance.UnloadAssetBundle(ResList[i], true);
        }
    }

    ~ResLoader()
    {
        Dispose();
    }
}
