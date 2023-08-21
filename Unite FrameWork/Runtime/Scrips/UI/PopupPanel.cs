using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Security.Cryptography.X509Certificates;

namespace Unite.Framework.UI
{
    public class PopupPanel : Panel, IPopup
    {
        [SerializeField]
        protected Text tittle;
        [SerializeField]
        protected Image background;
        [SerializeField]
        protected Button confirmButton;
        [SerializeField]
        protected Button cancelButton;

        public Button.ButtonClickedEvent OnConfirm => confirmButton.onClick;
        public Button.ButtonClickedEvent OnCancel => cancelButton.onClick;
        public Text Tittle => tittle;
        public Image Background => background;
    }
}
