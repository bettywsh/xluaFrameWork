using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParticleDepth : MonoBehaviour {

    public int order;

    Canvas canvas;
    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        Renderer[] renders = GetComponentsInChildren<Renderer>();

        foreach (Renderer render in renders)
        {
            render.sortingOrder = order + canvas.sortingOrder;
        }
    }
}
