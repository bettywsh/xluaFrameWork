using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerCamera : MonoBehaviour
{
    public Transform m_Camera;

    void Start()
    {
        // 获取场景里的main camera
        //m_Camera = Camera.main.transform;
    }

    // 用LateUpdate, 在每一帧的最后调整Canvas朝向
    void LateUpdate()
    {
        if (m_Camera == null)
        {
            return;
        }
        // 这里我的角色朝向和UI朝向是相反的，如果直接用LookAt()还需要把每个UI元素旋转过来。
        // 为了简单，用了下面这个方法。它实际上是一个反向旋转，可以简单理解为“负负得正”吧
        //transform.rotation = Quaternion.LookRotation(transform.position - m_Camera.position);

        transform.forward = m_Camera.transform.forward;
        transform.rotation = m_Camera.transform.rotation;

    }
}
