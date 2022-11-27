using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 摄像机根节点
/// </summary>

public class CameraRoot : MonoBehaviour
{

    const string MAIN_CAMERA_POINT_NAME = "MainCamera";

    Camera _MainCamera;
    Transform _MainCameraTran;

    Vector3 _movePos;

    // Start is called before the first frame update
    void Start()
    {
        _MainCamera = transform.Find(MAIN_CAMERA_POINT_NAME).GetComponent<Camera>();

    }

    public Camera GetMainCamera
    {
        get
        {
            if(_MainCamera == null)
            {
                _MainCameraTran = transform.Find(MAIN_CAMERA_POINT_NAME);

                _MainCamera = _MainCameraTran.GetComponent<Camera>();
            }

            return _MainCamera;
        }
    }


    public void SetCameraPos(Vector3 pos,Vector3 rotate)
    {
        transform.position = pos;
        _MainCameraTran.rotation.SetEulerRotation(rotate);
    }


    public void MoveTo(Vector3 tarPos,float time,Action finishAction )
    {
        _movePos = _MainCamera.transform.position;
        DOTween.To(() => _movePos, x => _movePos = x, tarPos, time).OnComplete(()=>
        {
            if (finishAction != null)
                finishAction();
        });
    }

}
