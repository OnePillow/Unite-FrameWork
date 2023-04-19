using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Unite.FrameWork
{
    [CustomEditor(typeof(PanelManager), true)]
    public class PanelManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var manager = this.target as PanelManager;
            GUILayout.Label("UI字段目录:");
            manager.uiFieldPath = EditorGUILayout.TextArea(manager.uiFieldPath);
            if (GUILayout.Button("保存UI字段"))
            {
                manager.SaveDataFromJson();
            }
            if (GUILayout.Button("读取UI字段"))
            {
                manager.LoadDataFromJson();
            }
            if (GUILayout.Button("注册所有面板"))
            {
                manager.GetPanels();
                manager.InitPanels();
            }
        }
    }

    /// <summary>
    /// 基础面板管理器
    /// </summary>
    public class PanelManager : BasePanelManager<StandardPanel>
    {
        /// <summary>
        /// 面板管理器单例
        /// </summary>
        /// <value></value>
        public static PanelManager Instance
        {
            get { return instance as PanelManager; }
        }

        /// <summary>
        /// 弹窗预制体
        /// </summary>
        public List<PopupPanel> popups;

        /// <summary>
        /// 按键点击委托
        /// </summary>
        public delegate void OnClick();

        new void Awake()
        {
            base.Awake();
            instance = this;
        }

        public override void InitPanels()
        {
            foreach (var panel in m_Registered.Values) panel.Init();
        }
        public override void GetPanels()
        {
            if (m_Registered == null) m_Registered = new();
            else m_Registered.Clear();
            foreach (var panel in GameObject.FindObjectsOfType<StandardPanel>()) Regist(panel);
        }

        /// <summary>
        /// 使用预制体新建弹窗
        /// </summary>
        /// <param name="popupIndex">弹窗下标</param>
        /// <param name="content">弹窗内容</param>
        /// <param name="confirm">确认事件</param>
        /// <param name="cancel">取消事件</param>
        /// <returns></returns>
        public static PopupPanel NewPopup(int popupIndex, string content,
                                OnClick confirm = null, OnClick cancel = null)
        {
            var popup = Instantiate(Instance.popups[popupIndex], Instance.canvas.transform);
            if (confirm != null) popup.confirm.onClick.AddListener(() => { confirm(); });
            if (cancel != null) popup.cancel.onClick.AddListener(() => { cancel(); });
            return popup;
        }
    }
}