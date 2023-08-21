using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Unite.Framework;

namespace Unite.Framework.UI
{
    [CustomEditor(typeof(UIManager), true)]
    public class UIManagerEditor : Editor
    {
        UIManager uiManager;
        ReorderableList canvas;
        ReorderableList popup;
        List<bool> treeFoldout;
        public override void OnInspectorGUI()
        {
            uiManager = target as UIManager;

            Init();
            if (treeFoldout is null) treeFoldout = new List<bool>();
            uiManager.configPath = EditorGUILayout.TextField("配置文件路径", uiManager.configPath);
            canvas.DoLayoutList();
            popup.DoLayoutList();
            var index = 0;
            var canvasProperty = serializedObject.FindProperty("canvas");
            foreach (var canvasElement in uiManager.canvas)
            {
                if (canvasElement is null 
                    || canvasProperty.GetArrayElementAtIndex(index).objectReferenceInstanceIDValue == 0) 
                    continue;
                DrawTree(canvasElement.transform);
                index += 1;
            }
        }

        void Init()
        {
            if (uiManager.canvas is null)   uiManager.canvas = new List<Canvas>();
            if (uiManager.popup is null)    uiManager.popup = new  List<Canvas>();
            if (canvas is null) canvas = new ReorderableList(uiManager.canvas, typeof(Canvas)) {                
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "UI界面");
                },
                drawElementCallback = (rect, index, isActived, isFocused) =>
                {
                    if (index >= uiManager.canvas.Count) return;
                    uiManager.canvas[index] = (Canvas)EditorGUI.ObjectField(rect, 
                                                                            uiManager.canvas[index],
                                                                            typeof(Canvas), true);
                },
                onAddCallback = (list) =>
                {
                    uiManager.canvas.Add(null);
                },
                onRemoveCallback = (list) =>
                {
                    if (list.index >= uiManager.canvas.Count) return;
                    uiManager.canvas.RemoveAt(list.index);
                }
            };
            if (popup is null) popup = new ReorderableList(uiManager.popup, typeof(Canvas)) {

            };
        }
        
        void DrawTree(Transform root)
        {
            var index = 0;
            UniteAlgorithm.TreeTraverse<Transform>(
                root,
                PreOrder: (Transform node, int level, out bool returnFlag) =>
                {
                    returnFlag = false;
                    EditorGUI.indentLevel = level;
                    //Debug.Log(node.name);
                    var content = node.name;
                    var isDataUIElement = false;
                    if (isDataUIElement = node.gameObject.TryGetComponent<IUIElement>(out var element))
                    {
                        content += " *reloadable*";
                    }

                    if (index >= treeFoldout.Count)
                    {
                        treeFoldout.Add(false);
                    }

                    if (node.childCount > 0 && (!isDataUIElement || node == root))
                    {
                        bool foldout = treeFoldout[index] = EditorGUILayout.Foldout(treeFoldout[index], content);
                        if (!foldout) returnFlag = true;
                    }
                    else
                    {
                        EditorGUILayout.LabelField(content);
                        returnFlag = isDataUIElement;
                    }
                    ++index;
                },
                GetChildren: (node) => 
                {
                    return node; 
                }
            );
            Debug.Log("-----");
        }

        [MenuItem("GameObject/UniteFramework/UI/UIManager", false, 21)]
        public static void Create()
        {
            var element = new GameObject().AddComponent<UIManager>();
            element.transform.parent = Selection.activeTransform;
            element.name = "UIManager";
        }
    }
}