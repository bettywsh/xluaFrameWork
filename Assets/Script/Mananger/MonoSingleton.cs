using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{

    protected static T m_instance = null;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject go = GameObject.Find("Singleton");
                if (go == null)
                {
                    go = new GameObject("Singleton");
                    DontDestroyOnLoad(go);
                }
                m_instance = go.GetComponent<T>();
                if (m_instance == null)
                {
                    m_instance = go.AddComponent<T>();
                    //(m_instance as MonoSingleton<T>).Init();
                }               
            }
            return m_instance;
        }
    }

    public virtual void Init()
    {

    }

    public virtual void Dispose()
    {

    }


}
