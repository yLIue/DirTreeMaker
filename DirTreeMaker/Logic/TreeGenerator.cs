using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace DirTreeMaker.Logic
{
    public class Node
    {
        public string ?title { get; set; }
        public int indentLevel { get; set; }
        public List<Node> children { get; set; } = new List<Node>();

    }
    internal class TreeGenerator
    {
        private const string CROSS = "├── ";
        private const string CORNER = "└── ";
        private const string VERTICAL = "│   ";
        private const string SPACE = "    ";

        public String GenerateTree(String _input)
        {
            var lines = _input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var roots = BuildTree(lines);
            var str = ResolveTree(roots).ToString();
            return str;
        }

        // 解析树
        public StringBuilder ResolveTree(List<Node> _roots)
        {
            StringBuilder strBu = new StringBuilder();
            foreach(var root in _roots)
            {
                strBu.AppendLine(root.title);
                for(int i = 0; i < root.children.Count; i++)
                {
                    bool isLast = (i == root.children.Count - 1);
                    ResolveChildren(strBu, root.children[i], "", isLast);
                }
            }
            return strBu;
        }
        public void ResolveChildren(StringBuilder _strBu,Node _node, String _prefix, bool _isLast)
        {
            _strBu.Append(_prefix);
            _strBu.Append(_isLast ? CORNER : CROSS);
            _strBu.AppendLine(_node.title);
            var nextPrefix = _prefix + (_isLast ? SPACE : VERTICAL);
            for (int i = 0; i < _node.children.Count; i++)
            {
                bool isLast = (i == _node.children.Count - 1);
                ResolveChildren(_strBu, _node.children[i], nextPrefix, isLast);
            }
        }
        public List<Node> BuildTree(String[] _lines)
        {
            var roots = new List<Node>();
            var stack = new Stack<Node>();

            foreach (var line in _lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                int layer = 0;
                layer = line.TakeWhile(c => c == '\t').Count();
                var node = new Node { title = line.Trim(),indentLevel = layer };

                if (stack.Count == 0)
                {
                    roots.Add(node);
                    stack.Push(node);
                }
                else
                {
                    while(stack.Count > 0 && stack.Peek().indentLevel >= layer)
                        stack.Pop();

                    if (stack.Count > 0)
                        stack.Peek().children.Add(node);
                    else
                        roots.Add(node);

                    stack.Push(node);
                }
            }
            return roots;
        }
    }
}