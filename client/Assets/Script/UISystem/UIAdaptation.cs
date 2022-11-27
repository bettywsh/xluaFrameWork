using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IPHONE
using UnityEngine.iOS;
#endif

public class UIAdaptation : MonoBehaviour
{
    public Transform Adaptation;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_IPHONE
        Debug.LogError(SystemInfo.deviceModel);
        if (SystemInfo.deviceModel == "iPhone10,3" ||
            SystemInfo.deviceModel == "iPhone10,4" || SystemInfo.deviceModel == "iPhone10,5" || SystemInfo.deviceModel == "iPhone10,6" || 
            SystemInfo.deviceModel == "iPhone11,1" || SystemInfo.deviceModel == "iPhone11,2" || SystemInfo.deviceModel == "iPhone11,3" ||
            SystemInfo.deviceModel == "iPhone11,4" || SystemInfo.deviceModel == "iPhone11,5" || SystemInfo.deviceModel == "iPhone11,6" ||
            SystemInfo.deviceModel == "iPhone12,1")
        {
            RectTransform rectTransform = transform as RectTransform;
            rectTransform.offsetMin = new Vector2(44f, 0f);
            rectTransform.offsetMax = new Vector2(-44f, 0f);

            GameObject canvasRoot = GameObject.Find("Canvas");
            RectTransform rect = canvasRoot.transform as RectTransform;
            //rect.sizeDelta
            RectTransform left = Adaptation.Find("Left") as RectTransform;

            left.offsetMin = new Vector2(0, 0);
            left.offsetMax = new Vector2((rect.sizeDelta.x - 44) * -1, 0);
            left.sizeDelta = new Vector2((rect.sizeDelta.x - 44) * -1, 0);
            RectTransform right = Adaptation.Find("Right") as RectTransform;
            right.offsetMin = new Vector2(rect.sizeDelta.x - 44, 0);
            right.offsetMax = new Vector2(0, 0);
            right.sizeDelta = new Vector2((rect.sizeDelta.x - 44) * -1, 0);
            Adaptation.gameObject.SetActive(true);
        }
#endif        
        //#if UNITY_ANDROID
        //        if (SystemInfo.deviceModel.Equals(""))
        //        {
        //            RectTransform rectTransform = transform as RectTransform;
        //            rectTransform.offsetMin = new Vector2(44f, 0f);
        //            rectTransform.offsetMax = new Vector2(-44f, 0f);
        //        }
        //#endif
    }

}
