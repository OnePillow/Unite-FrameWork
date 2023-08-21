using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Unite.Framework.UI
{
    public class UIManager : MonoBehaviour
    {
        [System.Serializable]
        public class ConfigPathData
        {
            public Canvas canvas;
            public string path;
        }

        private static UIManager instance;
        public List<Canvas> popup;
        public List<Canvas> canvas;

        protected static Dictionary<string, IUIElement> m_UICanvas = new Dictionary<string, IUIElement>();
        protected static Stack<IPanel> panelStack = new Stack<IPanel>();

        public string configPath { get; set; }
        
        void Awake() 
        {
            instance = this;
        }
        
        public T Cast<T>(object obj, T targeetType)
        {
            return (T)obj;
        }

        public static void Popup(string popupName, string tittle = null, Sprite background = null, UnityAction OnConfirm = null, UnityAction OnCancel = null)
        {
            var popup = instance.popup
                            .Find(config => config.name == popupName)
                            .GetComponent<IPopup>();
            if (tittle is not null)     popup.Tittle.text = tittle;
            if (background is not null) popup.Background.sprite = background;
            if (OnConfirm != null)      popup.OnConfirm.AddListener(OnConfirm);
            if (OnCancel != null)       popup.OnCancel.AddListener(OnCancel);
            PanelPush(popup);
        }

        public static void Popup(string popupName, UnityAction OnConfirm = null, UnityAction OnCancel = null)
        {
            var popup = instance.popup
                            .Find(config => config.name == popupName)
                            .GetComponent<IPopup>();
            if (OnConfirm != null) popup.OnConfirm.AddListener(OnConfirm);
            if (OnCancel != null) popup.OnCancel.AddListener(OnCancel);
            PanelPush(popup);
        }

        public static void PanelPush(IPanel panel)
        {
            panel.Open();
            panelStack.Push(panel);
        }

        public static bool PanelPop()
        {
            if (!panelStack.TryPop(out var panel)) return false;
            panel.Close();
            return true;
        }

        public static void StackClear()
        {
            bool empty = false;
            while (empty is false) empty = PanelPop();
        }

        


        public static void Serialize()
        {
            
        }
    }
}