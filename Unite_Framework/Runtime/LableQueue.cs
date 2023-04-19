using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace Unite.FrameWork
{
	public class LableQueue : BaseManagerGeneric<LableData>
	{
		[CustomEditor(typeof(LableQueue), true)]
		public class Editor : BaseManagerGenericEditor<LableData> 
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();
				var manager = target as LableQueue;
				if (GUILayout.Button("Ϊ����LableData�Զ�����Parent"))
				{
					foreach (var mgr in manager.tables) mgr.parent = mgr.transform;
				}
				if (GUILayout.Button("Ϊ����LableDataע��"))
				{
					foreach (var mgr in manager.tables) mgr.RegisterAllTarget();
				}
			}
		}
		[Tooltip("��ʵ���У�����ʱ��������")]
		public bool realQueue	= false;
		[Tooltip("����������ʱ���Զ�����������������")]
		public bool autoPop		= true;
		private List<LableData> tables { get { return target; } set { target = value; } }
		public  List<LableData> Tables { get { return target; } }
		public LableData Front {
			get
			{
				if (currentSize == 0) return null;
				return tables[currentSize - 1];
			}
		}
		public int Size { get { return currentSize; } }
		public override string TargetName { get { return "���ݱ�"; } }

		private int currentSize;
		private int maxSize;
		void Start()
		{
			maxSize		= tables.Count;
			currentSize	= tables.Count;
		}

		public virtual int Push(string[] infor, bool setAsLast = false)
		{
			var lable = tables[tables.Count - 1];
			if (!lable.information.Count.Equals(infor.Length) || tables == null
				|| maxSize == 0) 
			{ 
				Debug.LogError($"{this.name}:���д�СΪ0�����Ϊ��"); 
				return -1; 
			}
			if (currentSize == maxSize)
			{
				if (!autoPop) return 0; 
				if (setAsLast)
				{
					var index = 0;
					lable = tables[0];
					lable.transform.SetAsLastSibling();
					lable.information.ForEach((info) => { info.text = infor[index++]; });
				}
				else
				{
					for (int i = 1; i < tables.Count; ++i)
					{
						var data = tables[i - 1].information;
						var index = 0;
						foreach (var info in tables[i].information)
							data[index++].text = info.text;
					}
					lable = tables[currentSize - 1];
					lable.UpdateInformation(infor);
					print("enqueue1");
				}
			}
			else
			{
				lable = tables[currentSize];
				lable.gameObject.SetActive(true);
				lable.UpdateInformation(infor);
				currentSize += 1;
				print("enqueue2");
			}
			return 1;
		}

		public int Pop(bool setAsLast = false)
		{
			if (currentSize == 0 || !realQueue) { print("empty"); return 0; }
			if (setAsLast)
			{
				tables[0].gameObject.SetActive(false);
				tables[0].transform.SetAsLastSibling();
			}
			else
			{
				for (int i = 1; i < tables.Count; ++i)
				{
					var data = tables[i - 1].information;
					var index = 0;
					foreach (var info in tables[i].information)
						data[index++].text = info.text;
				}
				tables[currentSize - 1].gameObject.SetActive(false);
			}
			currentSize -= 1;
			print("pop: " + currentSize);
			return 1;
		}
	}
}