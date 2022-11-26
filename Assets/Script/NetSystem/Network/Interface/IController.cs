using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void MessageHandler(object[] msgDatas);
public delegate void OnRegHandlerEvent(string msgName, MessageHandler handler);

public interface IController
{
    OnRegHandlerEvent onRegHandler { get; set; }
    OnRegHandlerEvent onRemoveHandler { get; set; }

    KeyValuePair<string, MessageHandler>[] regedHandlers { get; }

    void onInit();
    void onRemove();
    void registerMessageHandler(string msgName, MessageHandler handler);
    void removeMessageHandler(string msgName);
    void removeAllMessageHandler();

    void notify(string msgName, params object[] msgDatas);
}
