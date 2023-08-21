using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unite.Framework
{
    public delegate IEnumerable GetChild<T>(T node);
    public delegate void TreeTraverseDelegate<T>(T node, int level, out bool returnFlag);
    public class UniteAlgorithm
    {
        //  root ���ڵ�
        //  Order ������ʽ
        //      - node: ��ǰ�ڵ�
        //      - level: ��ǰ�㼶
        //      - returnFlag: �˳���ǰ�ڵ������־
        //      - leftToRight:
        //          - true: ��������ӽڵ�
        //          - false: ��������ӽڵ�
        public static void TreeTraverse<T>(T root, GetChild<T> GetChildren, TreeTraverseDelegate<T> PreOrder, 
                                    TreeTraverseDelegate<T> PostOrder = null, int level = 0, bool leftToRight = true) where T : class
        {
            bool returnFlag = false;
            PreOrder.Invoke(root, level, out returnFlag);
            if (returnFlag) return;

            var children = GetChildren.Invoke(root);
            Debug.Log(children.ToString());
            foreach (var child in children)
            {
                Debug.Log(child.ToString());
                TreeTraverse<T>(child as T, GetChildren, PreOrder, PostOrder, level + 1, leftToRight);
            }
            PostOrder?.Invoke(root, level, out returnFlag);
        }
    }
}