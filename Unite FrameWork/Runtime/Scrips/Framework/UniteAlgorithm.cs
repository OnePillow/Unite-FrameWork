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
        //  root 根节点
        //  Order 遍历方式
        //      - node: 当前节点
        //      - level: 当前层级
        //      - returnFlag: 退出当前节点遍历标志
        //      - leftToRight:
        //          - true: 正序遍历子节点
        //          - false: 逆序遍历子节点
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