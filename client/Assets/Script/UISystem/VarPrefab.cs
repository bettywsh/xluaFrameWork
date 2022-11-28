using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TMPro;


[Serializable]
public class VarData
{
    public string objName;
    public GameObject objValue;
}

public class VarPrefab : MonoBehaviour
{

    
    [SerializeField]
    [HideInInspector]
    private int m_selectedIndex = -1;

    public VarData[] GetVarArray()
    {
        return varData.ToArray();
    }
    
        
    [SerializeField]
    [HideInInspector]
    public List<VarData> varData;


    public void AutoBind()
    {
        var newData = new VarData();
        newData.objName = "transform";
        newData.objValue = transform.gameObject;
        varData.Add(newData);
        DeepSearch(transform);
    }

    private void DeepSearch(Transform tran)
    {
        if (tran.name.Substring(0,2) == "##")
            return;
        if (tran.name[0] == '#')
        {
            string objName = tran.name.Substring(1);     
            var newData = new VarData();
            newData.objName = objName;
            newData.objValue = tran.gameObject;
            varData.Add(newData);
        }
        for (int i = 0, count = tran.childCount; i < count; i++)
            DeepSearch(tran.GetChild(i));
    }
}
