using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unite.FrameWork {
	public class PanelStack<T_Panel> where T_Panel : BasePanel
	{
		protected Stack<T_Panel> m_stack;
		public int Count { get { return m_stack.Count; } }
		public virtual int Push(T_Panel panel, bool open = false)
		{
			if (m_stack == null) m_stack = new Stack<T_Panel>();
			if (m_stack == null) return -1;
			if (open)			 panel.Open();
			m_stack.Push(panel);
			return 1;
		}

		public virtual T_Panel Peek() { return m_stack.Peek(); }
		public virtual int Pop(bool close = true, bool self_destroy = false)
		{
			if (m_stack.Count == 0) return 0;
			var top = m_stack.Peek();
			m_stack.Pop();
			if (close)			top.Close();
			if (self_destroy)	Object.Destroy(top.gameObject);
			return 1;
		}
	}
}
