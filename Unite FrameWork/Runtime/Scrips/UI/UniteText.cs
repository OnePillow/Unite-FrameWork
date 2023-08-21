using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Unity.VisualScripting;

namespace Unite.Framework.UI
{
    public class UniteText : Text, IUIElement
    {
        public enum ReferenceType
        {
            Field = 0,
            Local,
            Url,
        }
        public ReferenceType referenceType;
        public string sourceType => "textField";
        public string url { get; set; }
        public string sourceReference
        {
            get
            {
                switch (referenceType)
                {
                    case ReferenceType.Field:
                        return this.text;
                    case ReferenceType.Local:
                        return Application.dataPath + "/" + url;
                    case ReferenceType.Url:
                        return url;
                    default:
                        return null;
                }
            }
        }
        public bool reloadable { get; set; }
        public bool isReference { get; set; }
        public UISerializedObject sourceObject => new UISerializedObject
        {
            name = this.name,
            sourceType = this.sourceType,
            isReference = this.referenceType is not ReferenceType.Field,
            content = isReference ? sourceReference : this.text
        };

        public bool LoadSource<T>(T data)
        {
            if (data is not string content) return false;

            if (!isReference)
            {
                this.text = content;
                return true;
            }

            StartCoroutine(GetField(sourceReference));
            return true;
        }

        IEnumerator GetField(string url)
        {
            using (var www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("url无效，或网络故障");
                    yield break;
                }
                this.text = www.downloadHandler.text;
            }
        }
    }
}