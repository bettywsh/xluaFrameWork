using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TMPro;

public enum VarType : byte
{
    GameObject,
    Transform,
    Image,
    Button,
    TMP_InputField,
    Toggle,
    Slider,
    TextMeshProUGUI,
    LoopHorizontalScrollRect,
    LoopVerticalScrollRect,
    CanvasGroup,

}

[Serializable]
    public class VarData
    {
        public string name;
        public VarType type;
        public GameObject objValue;
        public Transform tranValue;
        public Image imgValue;
        public TextMeshProUGUI txtValue;
        public Button btnValue;
        public Toggle toggleValue;
        public Slider sliderValue;
        public TMP_InputField inputValue;
        public LoopHorizontalScrollRect loopHorizontalValue;
        public LoopVerticalScrollRect loopVerticalValue;
        public CanvasGroup canvasValue;

        public Object GetValue()
        {
            switch (type)
            {
                case VarType.GameObject: return objValue;
                case VarType.Transform: return tranValue;
                case VarType.TextMeshProUGUI: return txtValue;
                case VarType.Image: return imgValue;
                case VarType.Button: return btnValue;
                case VarType.Toggle: return toggleValue;
                case VarType.Slider: return sliderValue;
                case VarType.TMP_InputField: return inputValue;
                case VarType.LoopHorizontalScrollRect: return loopHorizontalValue;
                case VarType.LoopVerticalScrollRect: return loopVerticalValue;
                case VarType.CanvasGroup: return canvasValue;
            default: return null;
            }
        }

        public void Set(VarData newData)
        {
            //Clear
            objValue = null;
            tranValue = null;
            txtValue = null;
            imgValue = null;
            btnValue = null;
            toggleValue = null;
            sliderValue = null;
            inputValue = null;
            loopHorizontalValue = null;
            loopVerticalValue = null;
            canvasValue = null;
            //Set
            name = newData.name;
            type = newData.type;
            switch (type)
            {
                case VarType.GameObject:
                    objValue = newData.objValue; break;
                case VarType.Transform:
                    tranValue = newData.tranValue; break;
                case VarType.TextMeshProUGUI:
                    txtValue = newData.txtValue; break;
                case VarType.Image:
                    imgValue = newData.imgValue; break;
                case VarType.Button:
                    btnValue = newData.btnValue; break;
                case VarType.Toggle:
                    toggleValue = newData.toggleValue; break;
                case VarType.Slider:
                    sliderValue = newData.sliderValue; break;
                case VarType.TMP_InputField:
                    inputValue = newData.inputValue; break;
                case VarType.LoopHorizontalScrollRect:
                    loopHorizontalValue = newData.loopHorizontalValue; break;
                case VarType.LoopVerticalScrollRect:
                    loopVerticalValue = newData.loopVerticalValue; break;
                case VarType.CanvasGroup:
                    canvasValue = newData.canvasValue; break;
            }
        }
    }

public class VarPrefab : MonoBehaviour
{
    #region AutoBindDict
    public static readonly Dictionary<string, Func<GameObject, VarData>> AutoBindDict = new Dictionary<string, Func<GameObject, VarData>>
    {
        {"obj", go => new VarData {type = VarType.GameObject, objValue = go}},
        {"tsf", go => new VarData {type = VarType.Transform, tranValue = go.transform}},
        {
            "img", go =>
            {
                var value = go.GetComponent<Image>();
                if (value)
                    return new VarData { type = VarType.Image, imgValue = value};
                return null;
            }
        },
        {
            "txt", go =>
            {
                var value = go.GetComponent<TextMeshProUGUI>();
                if (value)
                    return new VarData {type = VarType.TextMeshProUGUI, txtValue = value};
                return null;
            }
        },
        {
            "btn", go =>
            {
                var value = go.GetComponent<Button>();
                if (value)
                    return new VarData {type = VarType.Button, btnValue = value};
                return null;
            }
        },
        {
            "tgl", go =>
            {
                var value = go.GetComponent<Toggle>();
                if (value)
                    return new VarData {type = VarType.Toggle, toggleValue = value};
                return null;
            }
        },
        {
            "sld", go =>
            {
                var value = go.GetComponent<Slider>();
                if (value)
                    return new VarData {type = VarType.Slider, sliderValue = value};
                return null;
            }
        },
        {
            "ipt", go =>
            {
                var value = go.GetComponent<TMP_InputField>();
                if (value)
                    return new VarData {type = VarType.TMP_InputField, inputValue = value};
                return null;
            }
        },
        {
            "lhs", go =>
            {
                var value = go.GetComponent<LoopHorizontalScrollRect>();
                if (value)
                    return new VarData {type = VarType.LoopHorizontalScrollRect, loopHorizontalValue = value};
                return null;
            }
        },
        {
            "lvs", go =>
            {
                var value = go.GetComponent<LoopVerticalScrollRect>();
                if (value)
                    return new VarData {type = VarType.LoopVerticalScrollRect, loopVerticalValue = value};
                return null;
            }
        },
        {
            "cng", go =>
            {
                var value = go.GetComponent<CanvasGroup>();
                if (value)
                    return new VarData {type = VarType.CanvasGroup, canvasValue = value};
                return null;
            }
        },
    };
    #endregion
    
    [SerializeField]
    [HideInInspector]
    private int m_selectedIndex = -1;    
    private Dictionary<VarType, string> varKeys = new Dictionary<VarType, string>();
    public List<string> varTypes 
    {
        get
        {
            TryInit();
            return new List<string>(varKeys.Values);
        }
    }

    public VarData[] GetVarArray()
    {
        return varData.ToArray();
    }

    private void TryInit()
    {
        if (varKeys.Count == 0)
        {
            varKeys.Add(VarType.GameObject, "objValue");
            varKeys.Add(VarType.Transform, "tranValue");
            varKeys.Add(VarType.TextMeshProUGUI, "txtValue");
            varKeys.Add(VarType.Image, "imgValue");
            varKeys.Add(VarType.Button, "btnValue");
            varKeys.Add(VarType.Toggle, "toggleValue");
            varKeys.Add(VarType.Slider, "sliderValue");
            varKeys.Add(VarType.TMP_InputField, "inputValue");
            varKeys.Add(VarType.LoopHorizontalScrollRect, "loopHorizontalValue");
            varKeys.Add(VarType.LoopVerticalScrollRect, "loopVerticalValue");
            varKeys.Add(VarType.CanvasGroup, "canvasValue");
        }
    }
    
    public string GetVarNameByType(VarType varType)
    {
        TryInit();
        if (varKeys.ContainsKey(varType))
        {
            return varKeys[varType];
        }
        return null;
    }
        
    [SerializeField]
    [HideInInspector]
    public List<VarData> varData;
    
    public void AutoBind()
    {
        DeepSearch(transform);
    }

    private void DeepSearch(Transform tran)
    {
        if (tran.name[0] == '#')
        {
            string objName = tran.name.Substring(1);
            string[] varTypes = objName.Split('_')[0].Split(',');
            foreach (var varType in varTypes)
            {
                if (AutoBindDict.TryGetValue(varType, out var func))
                {
                    var newData = func(tran.gameObject);
                    if (newData != null)
                    {
                        newData.name = varType + objName.Split('_')[1];
                        bool needAdd = true;
                        foreach (var data in varData)
                        {
                            if (data.name == newData.name)
                            {
                                needAdd = false;
                                if (data.GetValue() != newData.GetValue())
                                {
                                    data.Set(newData);
                                    Debug.Log($"Auto bind replace var! name: {data.name}");
                                }

                                break;
                            }
                        }

                        if (needAdd)
                            varData.Add(newData);
                    }
                }
            }
        }
        for (int i = 0, count = tran.childCount; i < count; i++)
            DeepSearch(tran.GetChild(i));
    }
}
