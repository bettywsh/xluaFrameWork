using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;
using System;

namespace UTools
{
    public class ButtonScale : MonoBehaviour, IPointerDownHandler
    {

        public RectTransform    m_tweenTarget;
        public Vector3          m_v3Start       = new Vector3(1, 1, 1);
        public Vector3          m_v3Down        = new Vector3(1.1f, 1.1f, 1.1f);
        public float            m_fDuration     = .2f;

        private Vector3         m_v3Scale;
        private bool            m_bEnter        = false;
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


        public void OnPointerDown(PointerEventData eventData)
        {
            Scale(m_v3Down);
        }


        void Scale(Vector3 to)
        {
            seq = DOTween.Sequence();
            seq.Insert(0, m_tweenTarget.DOScale(to, m_fDuration));
            seq.Insert(m_fDuration, m_tweenTarget.DOScale(m_v3Start, m_fDuration));
        }

        void OnDestroy()
        {
            seq.Kill();
        }

    }
}
