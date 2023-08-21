using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unite.Framework.UI
{
    public interface IUIElement
    {
        public string sourceType { get; }
        public string sourceReference { get; }
        public bool reloadable { get; }
        public UISerializedObject sourceObject { get; }
        public bool LoadSource<T>(T content);
    }

    public interface IDataUIElement : IUIElement { }

    public interface IPanel
    {
        public bool Visiable { get; set; }
        public void Open();
        public void Close(); 
    }

    public interface IPopup : IPanel
    {
        public Button.ButtonClickedEvent OnConfirm { get; }
        public Button.ButtonClickedEvent OnCancel { get; }
        public Text Tittle { get; }
        public Image Background { get; }
    }

    public enum LayoutType
    {
        Horizontal = 0,
        Vertical,
        None
    }

    public enum DataType : int
    {
        Field = 0,
        Custom = 5
    }
}