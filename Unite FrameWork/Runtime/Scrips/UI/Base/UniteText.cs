using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.EditorTools;

namespace Unite.Framework.UI
{
/*     [CustomEditor(typeof(UniteText))]
    public class UnitTextEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            var ut = target as UniteText;
            var tx = ut.m_Text;
            if (tx == null) return;
            var obj = new SerializedObject(tx);
            var it = obj.GetIterator();
            var hasNext = it.NextVisible(true);
            while (hasNext)
            {
                EditorGUILayout.PropertyField(it);
                hasNext = it.NextVisible(false);
            }
            obj.ApplyModifiedProperties();
        }
    }

    public class UniteText : UIElement
    {
        public Text m_Text;

        public static void Create()
        {
            var ut = new GameObject("UniteText").AddComponent<UniteText>();
        }
    } */
}