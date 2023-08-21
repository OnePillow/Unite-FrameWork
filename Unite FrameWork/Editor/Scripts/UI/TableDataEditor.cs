using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;

namespace Unite.Framework.UI
{
    [CustomEditor(typeof(TableData))]
    public class TableDataEditor : Editor
    {
        TableData table;
        Editor previourEditor;

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            Init();
            table.layoutType = (LayoutType)EditorGUILayout.EnumPopup("布局类型", table.layoutType);

            var selectIndex = table.drawList.index;
            if (table.data.Count > 0 && selectIndex < table.data.Count)
            {
                var layout = table.data[selectIndex].GetComponent<LayoutGroup>();
                SerializedObject serialized = null;
                switch (table.data[selectIndex].layoutType)
                {
                    case LayoutType.Horizontal:
                        serialized = new SerializedObject(layout as HorizontalLayoutGroup);
                        break;
                    case LayoutType.Vertical:
                        serialized = new SerializedObject(layout as VerticalLayoutGroup);
                        break;
                }
                if (serialized is null) return;

                var field = serialized.GetIterator();
                field.NextVisible(true);
                do
                {
                    EditorGUILayout.PropertyField(field);
                } while (field.NextVisible(false));
                serialized.ApplyModifiedProperties();
            }
            table.drawList.DoLayoutList();
            DrawSelect();
        }

        void DrawMap()
        {
            EditorGUILayout.LabelField("Map");
            ++EditorGUI.indentLevel;
            foreach (var pair in table.map)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("key", pair.Key);
                EditorGUILayout.LabelField("value", pair.Value.ToString());
                GUILayout.EndHorizontal();
            }
            --EditorGUI.indentLevel;
        }

        void DrawSelect()
        {
            if (table.Count == 0) return;

            var index = table.drawList.index;
            Editor.CreateCachedEditor(table[index], typeof(ListDataEditor), ref previourEditor);
            ++EditorGUI.indentLevel;
            if (table.propertyFoldout = EditorGUILayout.Foldout(table.propertyFoldout, "属性:" + table[table.drawList.index].name, true))
                previourEditor.OnInspectorGUI();
            --EditorGUI.indentLevel;
        }

        void Init()
        {
            if (table is null)      table = target as TableData;

            if (table.data is null) table.data = new List<ListData>();

            var data = serializedObject.FindProperty("data");
            for (int i = 0; i < table.data.Count; ++i)
            {
                if (table.data[i] != null 
                        && data.GetArrayElementAtIndex(i).objectReferenceInstanceIDValue != 0) continue;
                table.RemoveAt(i);
            }
            
            if (table.drawList is not null) return;

            table.drawList = new ReorderableList(table.data, typeof(ListData))
            {
                drawHeaderCallback = (rect) =>
                {
                    if (table.Count == 0) return;;
                    var curRect = new Rect(rect);
                    var width = curRect.width /= table[0].Count + 1;
                    curRect.x = width + 28;
                    EditorGUILayout.BeginHorizontal();
                    for (int i = 0; i < table[0].Count; ++i)
                    {
                        foreach (var list in table)
                        {
                            list.texts[i].name = EditorGUI.TextField(curRect, table[0].texts[i].name);
                            list.OnPrimaryChange(list.texts[i].name, i);
                        }
                        curRect.x += width;
                    }
                    EditorGUILayout.EndHorizontal();
                },
                drawElementCallback = (rect, index, isActived, isFocusd) =>
                {
                    var curRect = new Rect(rect);
                    var width = curRect.width /= table[0].Count + 1;
                    EditorGUI.LabelField(curRect, table[index].title);

                    table.OnPrimaryChange(table[index].title, index);

                    curRect.x = width + 30;
                    for (int i = 0; i < table[index].Count; ++i)
                    {
                        table[index][i] = EditorGUI.TextField(curRect, table[index][i]);
                        curRect.x += width;
                    }
                },
                onReorderCallbackWithDetails = (list, oldIndex, newIndex) =>
                {
                    table[newIndex].transform.SetSiblingIndex(newIndex);
                },
                onAddCallback = (list) => 
                {
                    table.Add();
                    list.index = list.count - 1;
                },
                onRemoveCallback = (tList) =>
                {
                    table.RemoveAt(tList.index--);
                    tList.index = Mathf.Max(0, tList.index);
                }
            };
        }

        [MenuItem("GameObject/UniteFramework/UI/TableData", false, 12)]
        public static void Create()
        {
            var element = new GameObject().AddComponent<TableData>();
            element.transform.parent = Selection.activeTransform;
            element.name = "TableData";
        }
    }
}