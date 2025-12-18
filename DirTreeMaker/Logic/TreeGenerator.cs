using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirTreeMaker.Logic
{
    public class Node
    {
        public string Name { get; set; }
        public int IndentLevel { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();
    }

    internal class TreeGenerator
    {
        // 定义树状符号常量，方便修改
        private const string CROSS = "├── ";
        private const string CORNER = "└── ";
        private const string VERTICAL = "│   ";
        private const string SPACE = "    ";

        public String GenerateTree(String _input)
        {
            if (string.IsNullOrWhiteSpace(_input)) return string.Empty;

            // 统一换行符并分割
            var lines = _input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var roots = BuildRoot(lines);

            StringBuilder sb = new StringBuilder();

            // 处理每一个根节点（通常只有一个，但支持多个）
            foreach (var root in roots)
            {
                // 修正点：根节点直接打印名字，不带连接线，符合你的示例输出
                sb.AppendLine(root.Name);

                // 渲染该根节点下的所有子节点
                for (int i = 0; i < root.Children.Count; i++)
                {
                    bool isLastChild = i == root.Children.Count - 1;
                    RenderNode(sb, root.Children[i], "", isLastChild);
                }
            }
            return sb.ToString();
        }

        // 补全的核心逻辑：递归渲染
        private void RenderNode(StringBuilder sb, Node node, string prefix, bool isLast)
        {
            // 1. 打印前缀
            sb.Append(prefix);

            // 2. 打印连接符（如果是最后一个子节点用 L形，否则用 T形）
            sb.Append(isLast ? CORNER : CROSS);

            // 3. 打印节点名称
            sb.AppendLine(node.Name);

            // 4. 计算子节点的前缀
            // 如果当前节点是最后一个，父级连线在这里结束，子级前缀补空格
            // 如果当前节点不是最后一个，父级连线要往下走，子级前缀补竖线
            var childPrefix = prefix + (isLast ? SPACE : VERTICAL);

            // 5. 递归处理所有子节点
            for (int i = 0; i < node.Children.Count; i++)
            {
                bool isLastChild = i == node.Children.Count - 1;
                RenderNode(sb, node.Children[i], childPrefix, isLastChild);
            }
        }

        public List<Node> BuildRoot(string[] _lines)
        {
            var roots = new List<Node>();
            var stack = new Stack<Node>();

            foreach (var line in _lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // 修正点：兼容 \t 或者 4个空格 作为一级缩进
                // 这里保留你的 \t 逻辑，如果你的输入是空格，需要改写这里
                int layer = 0;
                // 计算开头的 \t 数量
                layer = line.TakeWhile(c => c == '\t').Count();

                // 如果使用空格缩进，可以用下面这行逻辑替换上面：
                // int spaces = line.TakeWhile(c => c == ' ').Count();
                // layer = spaces / 4; // 假设4个空格为一级

                var node = new Node { Name = line.Trim(), IndentLevel = layer };

                if (stack.Count == 0)
                {
                    roots.Add(node);
                    stack.Push(node);
                }
                else
                {
                    // 找到当前节点的父级（缩进必须比当前小）
                    while (stack.Count > 0 && stack.Peek().IndentLevel >= layer)
                    {
                        stack.Pop();
                    }

                    if (stack.Count > 0)
                    {
                        stack.Peek().Children.Add(node);
                    }
                    else
                    {
                        roots.Add(node);
                    }
                    stack.Push(node);
                }
            }
            return roots;
        }
    }
}