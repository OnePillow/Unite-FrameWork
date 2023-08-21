using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Unite.Framework.UI
{
    [System.Serializable]
    public class UISerializedObject
    {
        public string name;
        public string sourceType;
        public bool isReference;
        public object content;
    }
    
    public class UIRuntime
    {
        public UISerializedObject property;
        public List<UIRuntime> children;
    }
}