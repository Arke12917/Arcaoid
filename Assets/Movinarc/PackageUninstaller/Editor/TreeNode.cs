using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

namespace Movinarc
{
    public class TreeNode
    {
        public TreeNode()
        {
            children = new List<TreeNode>();
            lastCheckedState = isChecked;
        }
        //dynamic
        public TreeNode root;

        public bool isTreeEmpty
        {
            get
            {
                return root.children == null || root.children.Count <= 0;
            }
        }

        public bool isExpanded = false;
        public bool isRoot = false;

        public TreeNode parent;
        public string name;
        public bool isChecked;
        public bool lastCheckedState;
        //Primary Key - dynamic
        public string path;

        //dynamic
        public bool isDirectory;

        public List<TreeNode> children;

        public List<string> pathList()
        {
            return new List<string>(path.Split(new []{ '/' }, System.StringSplitOptions.RemoveEmptyEntries));
        }

        public List<string> pathList(string path)
        {
            return new List<string>(path.Split(new []{ '/' }, System.StringSplitOptions.RemoveEmptyEntries));
        }

        public List<TreeNode> parents
        {
            get
            {
                return new List<TreeNode>(getParents(this));
            }
        }

        private IEnumerable<TreeNode> getParents(TreeNode node)
        {
            if (parent != null)
            {
                yield return parent;
                getParents(parent);
            }
        }

        public TreeNode AddNode(TreeNode node)
        {
            children.Add(node);
            return node;
        }

        public TreeNode AddNode(string name, bool isChecked)
        {
            TreeNode node = new TreeNode();
            node.parent = this;
            node.name = name;
            node.isChecked = isChecked;
            node.lastCheckedState = isChecked;
            children.Add(node);
            return node;
        }

        bool Exists()
        {
            return false;
        }

        public bool PathExists(string path)
        {
            return checkPathExists(pathList(path), root);
        }

        private bool checkPathExists(List<string> pathList, TreeNode fromNode)
        {
            if (pathList != null)
            {
                if (pathList.Count > 0)
                {
                    if (isTreeEmpty)
                        return false;
                    foreach (var ch in fromNode.children)
                    {
                        string n0 = pathList[0];
                        if (ch.name.Equals(n0, StringComparison.OrdinalIgnoreCase))
                        {
                            pathList.RemoveAt(0);
                            return checkPathExists(pathList, ch);
                      
                        }
                    }
                }

            }
            if (pathList.Count == 0)
                return true;
            else
                return false;
        }

        public TreeNode GetNodeInPath(string path, TreeNode root)
        {
            return checkGetNodeInPath(pathList(path), root);
        }

        private TreeNode checkGetNodeInPath(List<string> pathList, TreeNode fromNode)
        {

            if (pathList != null)
            {
                if (pathList.Count > 0)
                {
                    string n0 = pathList[0];

                    foreach (var child in fromNode.children)
                    {
                        if (child.name.Equals(n0, StringComparison.OrdinalIgnoreCase))
                        {
                            pathList.RemoveAt(0);
                            return checkGetNodeInPath(pathList, child);
                        }
                    }
                }

            }
            if (pathList.Count == 0)
                return fromNode;
            else
                return null;
        }

        public TreeNode GetParentNodeInPath(string path, TreeNode root)
        {
            return checkGetParentNodeInPath(pathList(path), root);
        }

        private TreeNode checkGetParentNodeInPath(List<string> pathList, TreeNode fromNode)
        {

            if (pathList != null)
            {
                if (pathList.Count > 0)
                {
                    pathList.RemoveAt(pathList.Count - 1);
                    return GetNodeInPath(listToPath(pathList), fromNode);
                }
            }
            return root;
        }

        public static string listToPath(List<string> list)
        {
            string joint = String.Join("/", list.ToArray());
            joint = iHateSlashes(joint);
            return joint;
        }

        public static string iHateSlashes(string which)
        {
            if (which.Length > 0)
            {
                if (which[which.Length - 1] == '/')
                    which = which.Remove(which.Length - 1);
                if (which[0] == '/')
                    which = which.Remove(0);
            }
            return which;
        }

        public bool ExistsInChildren(string name, bool recursive)
        {
            return false;
        }

        public List<TreeNode> FindByName(string name)
        {
            return null;
        }
    }

 }
