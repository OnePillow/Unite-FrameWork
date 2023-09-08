using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unite.Framework.UI
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public class UIElement : MonoBehaviour, IUIElement
    {
        public virtual string sourceType => null;
        public virtual string sourceReference { get => null; set { } }
        public virtual bool reloadable  { get => false; set { } }
        public virtual bool isReference { get => false; set { } }
        public virtual UISerializedObject sourceObject { get => null; set { } }
        public bool LoadSource<SourceData>(SourceData source) => false;
    }
}