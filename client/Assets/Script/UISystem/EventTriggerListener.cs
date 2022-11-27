using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using XLua;

[LuaCallCSharp]

public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
{
    public delegate void VoidDelegate(GameObject go);

    public delegate void VoidDrag(GameObject go, PointerEventData eventData);
    //public delegate void BoolDelegate(GameObject go, bool state);
    //public delegate void FloatDelegate(GameObject go, float delta);
    //public delegate void VectorDelegate(GameObject go, Vector2 delta);
    //public delegate void ObjectDelegate(GameObject go, GameObject obj);
    //public delegate void KeyCodeDelegate(GameObject go, KeyCode key);
    //public delegate void VoidDelegate(GameObject go, PointerEventData data);
    public VoidDelegate onClick;
    public VoidDrag onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    //public VoidDelegate onSelect;
    //public VoidDelegate onUpdateSelect;

    public VoidDrag onBeginDrag;
    public VoidDrag onDrag;
    public VoidDrag onEndDrag;

    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }

    static public EventTriggerListener Get(Transform transform)
    {
        return Get(transform.gameObject);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {        
        if (onClick != null) onClick(gameObject);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(gameObject, eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(gameObject);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(gameObject);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(gameObject);
    }
    //public override void OnSelect(BaseEventData eventData)
    //{
    //    if (onSelect != null) onSelect(gameObject);
    //}
    //public override void OnUpdateSelected(BaseEventData eventData)
    //{
    //    if (onUpdateSelect != null) onUpdateSelect(gameObject);
    //}

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null) onBeginDrag(gameObject,eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag(gameObject, eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null) onEndDrag(gameObject, eventData);
    }

}