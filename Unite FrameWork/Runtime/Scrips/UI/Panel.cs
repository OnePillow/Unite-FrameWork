using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Unite.Framework.UI
{
    public class Panel : UIElement, IPanel
    {
        public event System.Action OnOpen;
        public event System.Action OnClose;
        private bool visiable;
        public bool Visiable
        { 
            get => visiable;
            set
            {
                visiable = value;
                this.gameObject.SetActive(value);
            } 
        }

        public void Open() 
        {
            Visiable = true;
            OnOpen?.Invoke();
        }

        public void Close() 
        {
            Visiable = false;
            OnClose?.Invoke();
        } 
    }
}