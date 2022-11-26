using XLua;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// xuss
/// 
/// 20200514
/// </summary>

public class MessageManager : Singleton<MessageManager>
{
    private Dictionary<int, List<MessageHandler>> netHandlerDic = new Dictionary<int, List<MessageHandler>>();

    private Dictionary<string, List<MessageHandler>> eventHandlerDic = new Dictionary<string, List<MessageHandler>>();

    public void RegisterNetMessageHandler(int cmdID, MessageHandler message)
    {
        List<MessageHandler> list;
        if (!netHandlerDic.TryGetValue(cmdID, out list))
        {
            list = new List<MessageHandler>();
            netHandlerDic.Add(cmdID, list);
        }
        if (!list.Contains(message))
            list.Add(message);
    }

    public void RegisterEventMessageHandler(string eventName, MessageHandler message)
    {
        List<MessageHandler> list;
        if (!eventHandlerDic.TryGetValue(eventName, out list))
        {
            list = new List<MessageHandler>();
            eventHandlerDic.Add(eventName, list);
        }

        if (!list.Contains(message))
            list.Add(message);
    }


    public void RemoveNetMessage(int cmdID)
    {
        if (netHandlerDic.ContainsKey(cmdID))
        {
            netHandlerDic.Remove(cmdID);
        }
    }


    public void RemoveEventMessage(string eventName)
    {
        if (eventHandlerDic.ContainsKey(eventName))
        {
            eventHandlerDic.Remove(eventName);
        }
    }

    public void RemoveAllRegisterNet()
    {
        netHandlerDic.Clear();
    }


    public void RemoveAllRegisterEvent()
    {
        eventHandlerDic.Clear();
    }


    public void NetNotify(int id, params object[] msgData)
    {
        List<MessageHandler> handle;
        if (netHandlerDic.TryGetValue(id, out handle))
        {

            foreach (MessageHandler itemHand in handle)
            {
                itemHand(msgData);
            }

        }
    }

    public void EventNotify(string eventName,params object[] msgData)
    {
        List<MessageHandler> handle;

        if (eventHandlerDic.TryGetValue(eventName, out handle))
        {

            foreach (MessageHandler itemHand in handle)
            {

                itemHand(msgData);
            }

        }

    }


   
}
