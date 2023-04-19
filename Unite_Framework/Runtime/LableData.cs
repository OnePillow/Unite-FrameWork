using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;

namespace Unite.FrameWork
{
    public class LableData : BaseManagerGeneric<Text>
    {
        [CustomEditor(typeof(LableData), true)]
        public class Editor : BaseManagerGenericEditor<Text> { }
        public List<Text> information { get { return target; } set { target = value; } }
        public string[] Information {
            get
            {
                var info = new string[information.Count];
                for (int i = 0; i < info.Length; ++i) info[i] = information[i].text;
                return info;
            }
        }
        public UnityEvent ue;
        public override string TargetName { get { return "文本标签"; } }
        void Start()
        {
        }

        public void FUNC(LableData data) { }

        /// <summary>
        /// 更新所有标签
        /// 长度小于 information 时返回-1
        /// 长度大于 information 时返回0
        /// 长度等于 information 时返回1
        /// </summary>
        /// <param name="data">更新数据</param>
        /// <returns></returns>
        public virtual int UpdateInformation(string[] data)
        {
            if (data.Length < information.Count) return -1;
            else if (data.Length > information.Count) return 0;
            var index = 0;
            foreach (var info in information) info.text = data[index++];
            return 1;
        }

        public virtual void PassInformation(LableData data)
        {
            if (data == null || data.information.Count != information.Count) return;
            var index = 0;
            foreach (var info in data.information) info.text = information[index++].text;
        }
    }
}