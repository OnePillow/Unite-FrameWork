using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Unite.FrameWork{
    public abstract class BaseManagerGenericEditor<T> : Editor where T : Component
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var manager = this.target as BaseManagerGeneric<T>;
            var buttonLabel = "注册所有" + manager.TargetName;
            if (manager.parent == null && GUILayout.Button("自动设置Parent"))
            {
                manager.parent = manager.transform;
            }
            if (GUILayout.Button(buttonLabel))
            {
                manager.RegisterAllTarget();
            }
        }
    }
    public abstract class BaseManagerGeneric<T_Object> : MonoBehaviour where T_Object : Component
    {
        [SerializeField] public Transform parent;
        [SerializeField] public List<T_Object> target;
        [SerializeField] public string use_tag;
        public int Count { get { return target.Count; } }

        private const string EMPTY_TAG = "";
        public abstract string TargetName { get; }
        void Start()
        {

        }

        public virtual void RegisterAllTarget()
        {
            var targets = parent.GetComponentsInChildren<T_Object>();
            if (!use_tag.Equals(EMPTY_TAG))
            {
                target = new List<T_Object>();
                foreach (var obj in targets)
                    if (obj.CompareTag(use_tag)) target.Add(obj);
                return;
            }
            else target = targets.ToList();
        }

    }
}