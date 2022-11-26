using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GuidePassEvent : MonoBehaviour, IPointerClickHandler
{
    public bool isButton;
    //监听点击
    public void OnPointerClick(PointerEventData eventData)
    {
        RectGuidanceController rectGuidanceController = transform.GetComponent<RectGuidanceController>();
        if (transform.GetComponent<RectGuidanceController>().IsRect(eventData.position))
        {
            MessageManager.Instance.EventNotify(MessageConst.MsgGuideClickComplete);
            if (isButton)
            {
                rectGuidanceController.target.GetComponent<Button>().onClick.Invoke();
            }
            else
            {
                PassEvent(eventData, ExecuteEvents.submitHandler);
                PassEvent(eventData, ExecuteEvents.pointerClickHandler);
            }
        }
        //PassEvent(eventData, ExecuteEvents.pointerClickHandler);
    }

    //把事件透下去
    public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
        where T : IEventSystemHandler
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        GameObject current = data.pointerCurrentRaycast.gameObject;
        for (int i = 0; i < results.Count; i++)
        {
            if (current != results[i].gameObject)
            {
                ExecuteEvents.Execute(results[i].gameObject, data, function);
                break;
                //RaycastAll后ugui会自己排序，如果你只想响应透下去的最近的一个响应，这里ExecuteEvents.Execute后直接break就行。
            }
        }
    }

}
