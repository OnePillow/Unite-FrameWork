using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Unite.Framework;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace Unite.Framework.UI
{
    public class SourceHelper
    {
        public static void HandleLoad(IUIElement element, object content)
        {
            switch (element.sourceType)
            {
                case "ListData":
                    var json = JsonConvert.SerializeObject(content, Formatting.Indented);
                    Debug.Log(json);
                    var list = JsonConvert.DeserializeObject<string[]>(json);
                    element.LoadSource<string[]>(list);
                    break;
                case "TableData":
                    json = JsonConvert.SerializeObject(content);
                    Debug.Log(json);
                    var table = JsonConvert.DeserializeObject<string[][]>(json);
                    element.LoadSource<string[][]>(table);
                    break;
            }
        }
    }

    public class CanvasLoader : MonoBehaviour, IUIElement
    {
        public string sourceType { get; } = "Canvas";
        public string sourceReference { get; } = "";
        public bool Reloadable;
        public bool reloadable => Reloadable;
        public UISerializedObject sourceObject => Serialize();

        public bool LoadSource<T>(T content)
        {
            if (content is not UIRuntime) return false;

            var root = content as UIRuntime;
            var elementStack = new Stack<UIRuntime>();
            elementStack.Push(root);
            UniteAlgorithm.TreeTraverse(
                root: this.transform, 
                PreOrder: (Transform node, int depth, out bool returnFlag) => {
                    returnFlag = elementStack.Count == 0;
                    if (returnFlag) return;

                    Debug.Log("UI " + node.name);
                    var element = elementStack.Pop();
                    Debug.Log(element.property.name + " - " + node.name);
                    if (node != this.transform 
                        && node.TryGetComponent<IDataUIElement>(out var data)
                        && node.name.Equals(element.property.name))
                    {
                        SourceHelper.HandleLoad(data, element.property.content);
                        returnFlag = true;
                        return;
                    }
                    foreach (var child in element.children)
                    {
                        elementStack.Push(child);
                    }
                },
                GetChildren: (node) =>
                {
                    var children = new List<Transform>();
                    foreach (Transform child in node) children.Add(child);
                    return children;
                }
            );

            return true;
        }

        void Start()
        {
            var file = Application.dataPath + "/Canvas_Config/" + this.name + ".config";
            if (!File.Exists(file)) File.Create(file).Close();
            /*var json = JsonConvert.SerializeObject(Serialize().content, Formatting.Indented);
            File.WriteAllText(file, json);*/
            var json = File.ReadAllText(file);
            var content = JsonConvert.DeserializeObject<UIRuntime>(json);
            print(json);
            LoadSource<UIRuntime>(content);
        }

        void Update()
        {
            
        }

        UISerializedObject Serialize()
        {
            var content = new UIRuntime();
            var elementStack = new Stack<UIRuntime>();
            var parent = new List<UIRuntime>();
            elementStack.Push(content);
            UniteAlgorithm.TreeTraverse(
                root: this.transform,
                leftToRight: false,
                PreOrder: (Transform node, int depth, out bool returnFalg) =>
                {
                    returnFalg = false;
                    var element = elementStack.Pop();
                    element.property = new UISerializedObject() { name = node.name };
                    element.children = new List<UIRuntime>();
                    if (element != content && node.TryGetComponent<IUIElement>(out var data))
                    {
                        element.property = data.sourceObject;
                        returnFalg = true;
                        return;
                    }
                    //Debug.Log("have a baby" + node.name);
                    foreach (var child in node)
                    {
                        var obj = new UIRuntime();
                        element.children.Add(obj);
                        elementStack.Push(obj);
                    }
                },
                GetChildren: (node) =>
                {
                    var children = new List<Transform>();
                    for (int i = node.childCount - 1; i >= 0; --i)
                    {
                        children.Add(node.GetChild(i));
                    }
                    return null;
                }
            );
            return new UISerializedObject() { name = this.name, content = content };
        }
    }
}