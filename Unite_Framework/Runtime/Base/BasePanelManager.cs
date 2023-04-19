using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Unite.FrameWork
{
    public abstract class BasePanelManager<T_Panel> : MonoBehaviour where T_Panel : BasePanel
    {
        #region JsonDefine
        public class JsonData
        {
            public class Data
            {
                public Dictionary<string, string> field = new();
                public void Add(string name, string value) { field.Add(name, value); }
                public string GetValue(string name)
                {
                    if (!field.ContainsKey(name)) return null;
                    return field[name];
                }
            }
            public Dictionary<string, Data> dict = new();
            public Data AddField(string name)
            {
                var data = new Data();
                dict.Add(name, data);
                return data;
            }
            public Data GetField(string name)
            {
                if (!dict.ContainsKey(name)) return null;
                return dict[name];
            }
        }
        #endregion
        protected static BasePanelManager<T_Panel>      instance    { set; get; }
        protected static Dictionary<string, T_Panel>    m_Registered  { set; get; }
        protected static Dictionary<string, T_Panel>    m_Showed    { set; get; }
        protected static PanelStack<T_Panel>            m_Stack     { get; set; }

        [HideInInspector] public string uiFieldPath;
        public Canvas canvas;
        public bool awakeInitPanel = false;
        public bool debug = false;

        /// <summary>
        /// 以 Json 格式保存所有 UI 字段
        /// </summary>
        public virtual void SaveDataFromJson() { SaveDataFromJson(uiFieldPath); }

        /// <summary>
        /// 以 Json 格式保存所有字段
        /// </summary>
        /// <param name="path">保存位置</param>
        /// <returns></returns>
        public virtual int SaveDataFromJson(string path)
        {
            if (!File.Exists(path)) return -1;
            JsonData jsonData = new();
            JsonSerializerSettings setting = new();
            setting.Formatting = Formatting.Indented;
            foreach (var value in m_Registered.Values)
            {
                var panel = value;
                var data = jsonData.AddField(panel.name);
                foreach (var text in panel.fields) data.Add(text.name, text.text);
            }
            string json = JsonConvert.SerializeObject(jsonData, setting);
            File.WriteAllText(path, json);
            Debug.Log("Save Json:\n" + json);
            return 1;
        }

        /// <summary>
        /// 以 Json 格式读取所有 UI 字段
        /// </summary>
        /// <param name="LoadDataFromJson(uiFieldPath"></param>
        public virtual void LoadDataFromJson() { LoadDataFromJson(uiFieldPath); }

        /// <summary>
        /// 以 Json 格式读取所有 UI 字段
        /// </summary>
        /// <param name="path">读取路径</param>
        /// <returns></returns>
        public virtual int LoadDataFromJson(string path)
        {
            if (!File.Exists(path)) return -1;
            string json = File.ReadAllText(path);
            Debug.Log("Load Json:\n" + json);
            var jsonData = JsonConvert.DeserializeObject<JsonData>(json);
            foreach (var value in m_Registered.Values)
            {
                var panel = value;
                var data = jsonData.GetField(panel.name);
                foreach (var text in panel.fields) text.text = data.GetValue(text.name);
            }
            return 1;
        }

        /// <summary>
        /// 将面板注册到管理器
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        public static int Regist(T_Panel panel)
        {
            string _name = panel.name;
            if (m_Registered.ContainsKey(_name))
                return -1;
            else m_Registered.Add(_name, panel);
            return 1;
        }

        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="panelName">面板名</param>
        /// <returns></returns>
        public static int Open(string panelName)
        {
            if (m_Registered.TryGetValue(panelName, out T_Panel panel) && panel != null)
            {
                panel.Open();
                return 1;
            }
            else return 0;
        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        /// <param name="panelName">面板名</param>
        /// <returns></returns>
        public static int Close(string panelName)
        {
            if (m_Registered.TryGetValue(panelName, out T_Panel panel) && panel != null)
            {
                panel.Close();
                return 1;
            }
            else return 0;
        }

        public static void UpdateShowed(string name, bool show)
        {
            if (!m_Registered.ContainsKey(name) || m_Registered[name] == null)
                Debug.LogError("面板未注册或不存在");
            else if (m_Showed.ContainsKey(name))
                m_Showed.Remove(name);
            else
                m_Showed.Add(name, m_Registered[name]);
        }

        public static void UpdateShowed(T_Panel panel, bool show)
        {
            UpdateShowed(panel.name, show);
        }

        public static void Push(T_Panel panel)      { m_Stack.Push(panel); }
        public static void Push(string panelName)   { m_Stack.Push(m_Registered[panelName]); }
        public static void Pop()                    { m_Stack.Pop(); }
        public static T_Panel Peek()                { return m_Stack.Peek(); }
        public void OpenPush(string name)           { m_Stack.Push(m_Registered[name], open: true); }
        public void OpenPush(T_Panel panel)         { m_Stack.Push(panel, open: true); }
        public void ClosePop(bool destroy = false)
        {
            m_Stack.Pop(close: true, self_destroy: destroy);
        }

        /// <summary>
        /// 创建面板
        /// </summary>
        /// <param name="name">面板名</param>
        /// <param name="parent">面板父物体</param>
        /// <returns></returns>
        public static T_Panel OpenNew(string name, bool show, Transform parent)
        {
            parent = parent ? parent : instance.canvas.transform;
            var panel = Instantiate(new GameObject(name), parent).AddComponent<T_Panel>();
            Regist(panel);
            if (show) m_Showed.Add(panel.name, panel);
            return panel;
        }

        public static bool IsOpen(T_Panel panel)
        {
            return m_Showed.ContainsValue(panel);
        }

        public static bool IsOpen(string name)
        {
            return m_Showed.ContainsKey(name);
        }

        protected void Awake()
        {
            if (awakeInitPanel)
            {
                GetPanels();
                InitPanels();
            }
        }

        /// <summary>
        /// 初始化所有面板
        /// </summary>
        public abstract void InitPanels();

        /// <summary>
        /// 注册所有面板
        /// </summary>
        public abstract void GetPanels();
    }
}