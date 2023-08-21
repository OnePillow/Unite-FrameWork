using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.Overlays;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Unite.Framework.UI
{
    public class TableData : MonoBehaviour, IDataUIElement, IUIDataCollection<ListData>
    {
        public string sourceType => "TableData";
        public string sourceReference { get; set; }
        public bool reloadable { get; set; }
        public UISerializedObject sourceObject
        {
            get
            {
                var obj = new UISerializedObject()
                {
                    name = this.name,
                    sourceType = this.sourceType,
                    isReference = false
                };

                var content = new string[Count][];
                var index = 0;
                foreach (var list in this)
                {
                    content[index] = list.data.ToArray();
                    index += 1;
                }
                obj.content = content;
                return obj;
            }
        }
        public List<ListData> data;
        public List<ListData> Data => data;
        public Dictionary<string, int> map { get; } = new Dictionary<string, int>();
        protected LayoutType m_layoutType;
        public LayoutType layoutType
        {
            get => m_layoutType;
            set
            {
                m_layoutType = value;
                switch (m_layoutType)
                {
                    case LayoutType.Horizontal:
                        data.ForEach(list => list.layoutType = LayoutType.Vertical);
                        if (this.gameObject.TryGetComponent<VerticalLayoutGroup>(out var vertical))
                            DestroyImmediate(vertical);
                        if (this.gameObject.GetComponent<HorizontalLayoutGroup>())
                        {
                            return;
                        }
                        var h_layout = this.gameObject.AddComponent<HorizontalLayoutGroup>();
                        h_layout.childControlWidth = true;
                        h_layout.childControlHeight = true;
                        return;
                    case LayoutType.Vertical:
                        data.ForEach(list => list.layoutType = LayoutType.Horizontal);
                        if (this.gameObject.TryGetComponent<HorizontalLayoutGroup>(out var horizontal))
                            DestroyImmediate(horizontal);
                        if (this.gameObject.GetComponent<VerticalLayoutGroup>())
                            return;
                        var v_layout = this.gameObject.AddComponent<VerticalLayoutGroup>();
                        v_layout.childControlWidth = true;
                        v_layout.childControlHeight = true;
                        return;
                    default:
                        if (this.gameObject.TryGetComponent<LayoutGroup>(out var layout))
                            DestroyImmediate(layout);
                        return;
                }
            }
        }
        public int Count => data.Count;
        public bool IsReadOnly => true;
        public Transform dataParent 
        { 
            get => this.transform; 
            set {} 
        }
        public ListData this[int index]
        {
            get => data[index];
            set => data[index] = value;
        }

        public virtual ListData this[string primary]
        {
            get
            {
                return map.TryGetValue(primary, out var index) && index < Count ? this[index] : null;
            }
            set
            {
                if (!map.TryGetValue(primary, out var index) || index >= Count)
                {
                    throw new System.Exception("不存在的主键");
                }
                this[index] = value;
            }
        }

#if UNITY_EDITOR
        public ReorderableList drawList;
        public bool propertyFoldout = true;
#endif

        void Awake()
        {
            var index = 0;
            foreach (var list in data)
            {
                map.Add(list.name, index);
                index += 1;
            }
        }

        void Start()
        {

        }

        void Update()
        {
            
        }

        public bool LoadSource<T>(T data)
        {
            if (data is not string[][] content)
            {
                Debug.LogError("数据类型错误");
                return false;
            }
            else if (content.Length != this.Count)
            {
                Debug.LogError("数据长度错误");
                return false;
            }
            else if (!CheckDataSize(content))
            {
                Debug.LogError("数据长度错误");
                return false;
            }

            var index = 0;
            foreach (var list in this)
            {
                var cur = 0;
                foreach (var field in content[index])
                {
                    list[cur] = field;
                    cur += 1;
                }
                index += 1;
            }

            return true;
        }

        bool CheckDataSize(string[][] content)
        {
            var index = 0;
            foreach (var list in this)
            {
                if (list.Count == content[index].Length) continue;
                return false;
            }
            return true;
        }

        public void UpdateData(object[] info, int index = 0)
        {
            if (info.Length - index < this.Count)
            {
                throw new System.Exception("Data length is not match.");
            }
        }

        public virtual int IndexOf(ListData value)
        {
            int index = 0;
            foreach (var info in this)
            {
                if (info.Equals(value)) return index;
                ++index;
            }
            return -1;
        }

        public virtual void Insert(int index, ListData value)
        {
            Add(value);
            data[this.Count - 1].transform.SetSiblingIndex(index);
        }


        public virtual void RemoveAt(int index)
        {
            if (index < 0 || index > data.Count)
            {
                Debug.LogError("Index out of range.");
                return;
            }
#if UNITY_EDITOR
            if (data[index] != null)
            {
                map.Remove(data[index].name);
                DestroyImmediate(data[index].gameObject);
            }
#else
            map.Remove(data[index].name);
            DestroyImmediate(data[index].gameObject);
#endif
            data.RemoveAt(index);
        }

        public virtual void Add(ListData value = null)
        {
            if (data.Count > 0)
            {
                var obj = Instantiate(data[Count - 1]);
                var tr = obj.GetComponent<RectTransform>();
                tr.SetParent(dataParent);
                data.Add(obj.GetComponent<ListData>());
                return;
            }
            var rt = new GameObject("列表 " + data.Count).AddComponent<RectTransform>();
            rt.SetParent(dataParent.GetComponent<RectTransform>());
            print(rt.parent);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            data.Add(rt.gameObject.AddComponent<ListData>());

            OnPrimaryChange(rt.name, data.Count - 1);
        }

        public virtual void Clear()
        {
            data.Clear();
        }

        public virtual bool Contains(ListData value)
        {
            return IndexOf(value) != -1;
        }

        public virtual void CopyTo(ListData[] infor, int index)
        {
            if (infor.Length < data.Count + index)
            {
                return;
            }

            foreach (var info in this)
            {
                infor[index] = info;
                index += 1;
            }
        }

        public virtual bool Remove(ListData value)
        {
            int index = IndexOf(value);
            if (index == -1) return false;

            RemoveAt(index);
            return true;
        }

        public void OnPrimaryChange(string primary, int index)
        {
            if (map.TryGetValue(primary, out var value) && value == index) return;
            else if (map.ContainsKey(primary))
            {
                map[primary] = index;
                return;
            }
            
            var match = map.FirstOrDefault(pair => pair.Value.Equals(index));
            if (!match.Equals(default(KeyValuePair<string, int>)))
            {
                map.Remove(match.Key);
            }
            map.Add(primary, index);
        }

        public virtual IEnumerator<ListData> GetEnumerator() => new DataEnumerator<ListData, ListData>(
            list: data,
            GetCurrent: (in IList<ListData> list, in int index) => list[index]
        );

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
