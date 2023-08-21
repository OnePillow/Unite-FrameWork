using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Unite.Framework.UI
{
    public class UIElementEditor : Editor
    {
        //[MenuItem("GameObject/UniteFramework/UI/UIElement", false, 0)]
        public static void Create()
        {
            var element = new GameObject().AddComponent<UIElement>();
            element.transform.parent = Selection.activeTransform;
        }
    }
}