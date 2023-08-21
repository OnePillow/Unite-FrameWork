using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditorInternal;
using UnityEditor;
using System;
using System.Linq;

namespace Unite.Framework.UI
{
    public class ListData : MonoBehaviour, IDataUIElement, IUIDataCollection<string>
    {
        public class FormatStragety
        {
            Dictionary<int, System.Func<string, string>> stragety 
                            = new Dictionary<int, System.Func<string, string>>();

            public bool TryAddFormat(int type, System.Func<string, string> format)
            {
                if (type < 5) return false;

                return stragety.TryAdd(type, format);
            }

            public bool TryGetFormat(int type, out System.Func<string, string> format)
            {
                if (type < 5)
                {
                    format = null;
                    return true;
                }
                return stragety.TryGetValue(type, out format);
            }

            public bool TryRemoveFormat(int type)
            {
                if (!stragety.ContainsKey(type)) return false;

                stragety.Remove(type);
                return true;
            }
        }
        public FormatStragety m_formatStragety = new FormatStragety();
        public FormatStragety formatStragety => m_formatStragety;



        public virtual string sourceType => "ListData";
        public virtual string sourceReference { get; set; }
        public virtual bool reloadable { get; set; }
        public virtual bool isReference { get; set; }
        public UISerializedObject sourceObject => new UISerializedObject
        {
            name = this.name,
            sourceType = this.sourceType,
            isReference = false,
            content = data.ToArray()
        };
        public int data_type = (int)DataType.Field;
        public DataType dataType
        { 
            get
            {
                if (data_type >= 0 && data_type <= 5) 
                    return (DataType) data_type;
                
                return (DataType)data_type;
            }
            set
            {
                data_type = (data_type >= 0 && data_type <= 5) ? (int)value 
                                                               : (int)DataType.Custom;
                var index = 0;
                data.ForEach(info => texts[index++].text = Format(info));
            }
        }

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
                        if (this.gameObject.TryGetComponent<VerticalLayoutGroup>(out var vertical))
                            DestroyImmediate(vertical);
                        if (this.gameObject.GetComponent<HorizontalLayoutGroup>())
                            return;
                        this.gameObject.AddComponent<HorizontalLayoutGroup>();
                        return;
                    case LayoutType.Vertical:
                        if (this.gameObject.TryGetComponent<HorizontalLayoutGroup>(out var horizontal))
                            DestroyImmediate(horizontal);
                        if (this.gameObject.GetComponent<VerticalLayoutGroup>())
                            return;
                        this.gameObject.AddComponent<VerticalLayoutGroup>();
                        return;
                    default:
                        if (this.gameObject.TryGetComponent<LayoutGroup>(out var layout))
                            DestroyImmediate(layout);
                        return;
                }
            }
        }

        public List<Text> texts = new List<Text>();
        public List<string> data = new List<string>();
        public List<string> Data => data;
        public Dictionary<string, int> map { get; } = new Dictionary<string, int>();
        public string title 
        { 
            get => this.name; 
            set => this.name = value;
        }
        public Transform dataParent 
        {
            get => this.transform;
            set { }
        }

        public virtual string this[int index]
        {
            get => texts[index] ? data[index] : null;
            set
            {
                data[index] = value;
                texts[index].text = Format(data[index]);
            }
        }

        public virtual string this[string tittle]
        {
            get => map.TryGetValue(title, out var index) && index < Count ? this[index] : null;
            set
            {
                if (!map.TryGetValue(title, out var index) || index >= Count)
                {
                    throw new System.Exception("不存在的主键");
                }
                this[index] = value;
            }
        }

        public int Count => texts.Count;

        public bool IsReadOnly { get; } = false;

#if UNITY_EDITOR
        public ReorderableList textList;
        public bool propertyFoldout = true;
        public bool drawSelect = false;
        public bool alwaysUpdate = false;
        public bool typeDropdown = false;
#endif

        protected virtual void Awake()
        {
            var index = 0;
            foreach (var info in texts)
            {
                map.Add(info.name, index);
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
            if (data is not string[] content) return false;
            else if (content.Length != texts.Count)
            {
                Debug.LogError("data length is not match");
                return false;
            }

            var index = 0;
            foreach (var field in content)
            {
                texts[index].text = field;
                index += 1;
            }
            return true;
        }

        public void UpdateData(object[] info, int index = 0)
        {
            if (info.Length - index < this.Count)
            {
                throw new System.Exception("Data length is not match.");
            }

            data.ForEach(text => 
            {
                text = (string)(info[index++]);
            });
        }

        public virtual int IndexOf(string value)
        {
            int index = 0;
            foreach (var info in this)
            {
                if (info.Equals(value)) return index;
                ++index;
            }
            return -1;
        }

        public virtual void Insert(int index, string value)
        {
            Add(value);

            texts[this.Count - 1].rectTransform.SetSiblingIndex(index);
        }


        public virtual void RemoveAt(int index)
        {
            if (index < 0 || index > texts.Count)
            {
                Debug.LogError("Index out of range.");
                return;
            }
#if UNITY_EDITOR
            if (texts[index] != null)
            {
                map.Remove(texts[index].name);
                DestroyImmediate(texts[index].gameObject);
            }
#else
            map.Remove(texts[index].name);
            DestroyImmediate(texts[index].gameObject);
#endif
            texts.RemoveAt(index);
            data.RemoveAt(index);
        }

        public virtual void Add(string value = "")
        {
            var rt = new GameObject("数据 " + texts.Count).AddComponent<RectTransform>();
            rt.SetParent(dataParent);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);

            var tx = rt.gameObject.AddComponent<Text>();
            tx.text = value;
            tx.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            texts.Add(tx);
            data.Add(tx.text);
        }

        public virtual void Clear()
        {
            texts.Clear();
        }

        public virtual bool Contains(string value)
        {
            return IndexOf(value) != -1;
        }

        public virtual void CopyTo(string[] infor, int index)
        {
            if (infor.Length < texts.Count + index)
            {
                return;
            }

            foreach (var info in this)
            {
                infor[index] = info;
                index += 1;
            }
        }

        public virtual bool Remove(string value)
        {
            int index = IndexOf(value);
            if (index == -1) return false;

            RemoveAt(index);
            return true;
        }

        public virtual IEnumerator<string> GetEnumerator() => new DataEnumerator<string, string>(
            list: data,
            GetCurrent: (in IList<string> list, in int index) =>
            {
                return list[index];
            }
        );

        public string Format(string value)
        {
            switch (data_type)
            {
                case (int)DataType.Field:
                    return value;
                default:
                    if (formatStragety.TryGetFormat(data_type, out var format))
                    {
                        return format?.Invoke(value);
                    }
                    return "#";
            }
        }

        public bool TryAddFormat(int type, System.Func<string, string> format)
        {
            return formatStragety.TryAddFormat(type, format);
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
                map[match.Key] = int.MaxValue;
            }
            map.Add(primary, index);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
