using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Barbar.TreeDistance.Util
{
    public class TreeNode 
    {
        private IList<TreeNode> m_Children = new List<TreeNode>();
        private TreeNode m_Parent;

        public IList<TreeNode> Children
        {
            get { return new ReadOnlyCollection<TreeNode>(m_Children); }
        }

        public bool isRoot()
        {
            return m_Parent == null;
        }

        public TreeNode getRoot()
        {
            var root = this;
            while(root.m_Parent != null)
            {
                root = root.m_Parent;
            }
            return root;
        }

        public int getLevel()
        {
            var root = this;
            int level = 0;
            while(root.m_Parent != null)
            {
                root = root.m_Parent;
                level++;
            }
            return level;
        }

        public void Add(TreeNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (node.m_Parent != null)
            {
                node.m_Parent.m_Children.Remove(node);
            }
            node.m_Parent = this;
            m_Children.Add(node);
        }
    }
}
