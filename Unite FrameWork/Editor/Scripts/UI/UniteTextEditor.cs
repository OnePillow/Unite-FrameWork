using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Graphs;
using Codice.Client.BaseCommands.Import;

namespace Unite.Framework.UI
{
    [CustomEditor(typeof(UniteText), true)]
    public class UniteTextEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var uniteText = target as UniteText;
            var property = serializedObject.GetIterator();
            property.NextVisible(true);
            do
            {
                EditorGUILayout.PropertyField(property);
            }
            while (property.NextVisible(false));

            if (uniteText.referenceType is not UniteText.ReferenceType.Field)
            {
                uniteText.isReference = true;
                uniteText.url = EditorGUILayout.TextField("url", uniteText.url);
            }
            else uniteText.isReference = false;
            uniteText.reloadable = EditorGUILayout.Toggle("Reloadable", uniteText.reloadable);

            serializedObject.ApplyModifiedProperties();
        }
    }
}