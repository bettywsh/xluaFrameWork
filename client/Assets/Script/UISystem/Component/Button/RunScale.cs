using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;
using System;

namespace UTools
{
    public class RunScale : MonoBehaviour
    {

        public RectTransform    m_tweenTarget;
        public Vector3          m_v3Enter       = new Vector3(1.1f, 1.1f, 1.1f);
        public Vector3          m_v3Down        = new Vector3(1.1f, 1.1f, 1.1f);
        public float            m_fDuration     = .2f;

        private Vector3         m_v3Scale;
        Sequence seq;

        // Use this for initialization
        void Start()
        {
            if (m_tweenTarget == null)
            {
                m_tweenTarget = GetComponent<RectTransform>();
            }
            m_v3Scale = m_tweenTarget.localScale;
        }


        public void Run()
        {
            Scale(m_v3Down);
        }

        public void RunForever()
        {    
            Scale(m_v3Down);
            seq.SetLoops(-1);
        }
        
        void Scale(Vector3 to)
        {
            seq = DOTween.Sequence();
            seq.Insert(0, m_tweenTarget.DOScale(to, m_fDuration));
            seq.Insert(m_fDuration, m_tweenTarget.DOScale(Vector3.one, m_fDuration));
        }
        

        void OnDestroy()
        {
            seq.Kill();
        }

    }
}
