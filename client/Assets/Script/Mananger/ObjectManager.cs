using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{

    Dictionary<string, ObjectInfo> m_Object = new Dictionary<string, ObjectInfo>();

    public void LoadObject(string name, string relativePath, ResType resType)
    { 
    
    }
}


public class ObjectInfo
{ 

}