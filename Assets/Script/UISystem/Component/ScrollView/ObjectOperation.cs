using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOperation
{
    public static Transform AddChildren(Transform parent, GameObject children)
    {
        Transform go = GameObject.Instantiate<GameObject>(children).transform;
        SetParent(parent, go);
        return go;
    }

    public static Transform SetParent(Transform parent, Transform go)
    {
        go.SetParent(parent, false);
        go.localPosition = Vector3.zero;
        go.localScale = Vector3.one;
        go.localEulerAngles = Vector3.zero;
        return go;
    }

    public static void SetMaterials(Renderer renderer, Material material1)
    {
        Material[] materials = new Material[]{ new Material(material1) };
        renderer.materials = materials;
    }
    public static void SetMaterials(Renderer renderer, Material material1, Material material2)
    {
        Material[] materials = new Material[] { new Material(material1), new Material(material2) };
        renderer.materials = materials;
    }
}
