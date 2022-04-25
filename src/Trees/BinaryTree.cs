using System.Collections;
using System.Text;

namespace Trees;

public class BinaryTree<T> : ICollection<T> where T : IComparable<T>
{
    protected class Node<TValue>
    {
        public TValue Value { get; set; }
        public Node<TValue>? Left { get; set; }
        public Node<TValue>? Right { get; set; }

        public Node(TValue value) => Value = value;
    }

    protected Node<T>? Root;

    public BinaryTree() { }
    public BinaryTree(T value) => Root = new Node<T>(value);
    public BinaryTree(IEnumerable<T> collection)  => AddRange(collection);

    public T MinValue
    {
        get
        {
            if (Root == null)
                throw new InvalidOperationException("Tree is empty");
            var current = Root;
            while (current.Left != null)
                current = current.Left;
            return current.Value;
        }
    }
    public T MaxValue
    {
        get
        {
            if (Root == null)
                throw new InvalidOperationException("Tree is empty");
            var current = Root;
            while (current.Right != null)
                current = current.Right;
            return current.Value;
        }
    }

    public void AddRange(IEnumerable<T> collection)
    {
        foreach (var value in collection) Add(value);
    }

    public IEnumerable<T> Inorder()
    {
        if (Root == null)
            yield break;

        var stack = new Stack<Node<T>>();
        var node = Root;

        while (stack.Count > 0 || node != null)
        {
            if (node == null)
            {
                node = stack.Pop();
                yield return node.Value;
                node = node.Right;
            }
            else
            {
                stack.Push(node);
                node = node.Left;
            }
        }
    }
    public IEnumerable<T> Preorder()
    {
        if (Root == null)
            yield break;

        var stack = new Stack<Node<T>>();
        stack.Push(Root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            yield return node.Value;
            if (node.Right != null)
                stack.Push(node.Right);
            if (node.Left != null)
                stack.Push(node.Left);
        }
    }

    public IEnumerable<T> Postorder()
    {
        if (Root == null)
            yield break;

        var stack = new Stack<Node<T>>();
        var node = Root;

        while (stack.Count > 0 || node != null)
        {
            if (node == null)
            {
                node = stack.Pop();
                if (stack.Count > 0 && node.Right == stack.Peek())
                {
                    stack.Pop();
                    stack.Push(node);
                    node = node.Right;
                }
                else
                {
                    yield return node.Value;
                    node = null;
                }
            }
            else
            {
                if (node.Right != null)
                    stack.Push(node.Right);
                stack.Push(node);
                node = node.Left;
            }
        }
    }
    public IEnumerable<T> Levelorder()
    {
        if (Root == null)
            yield break;

        var queue = new Queue<Node<T>>();
        queue.Enqueue(Root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            yield return node.Value;
            if (node.Left != null)
                queue.Enqueue(node.Left);
            if (node.Right != null)
                queue.Enqueue(node.Right);
        }
    }

    public override string? ToString()
    {
        if (Root == null)
            return base.ToString();

        var result = new StringBuilder();

        void Append(Node<T> node) =>
            result!.Append(node == Root ? $"[{node.Value}] " : $"{node.Value} ");

        var current = Root;
        while (current != null)
        {
            if (current.Left == null)
            {
                Append(current);
                current = current.Right;
            }
            else
            {
                var pre = current.Left;
                while (pre.Right != null && pre.Right != current)
                    pre = pre.Right;

                if (pre.Right == null)
                {
                    pre.Right = current;
                    current = current.Left;
                }
                else
                {
                    pre.Right = null;
                    Append(current);
                    current = current.Right;
                }
            } 

        }

        return result.ToString();
    }

    #region ICollection<T> Members
    public int Count { get; protected set; }
    public virtual void Add(T item)
    {
        var node = new Node<T>(item);

        if (Root == null)
            Root = node;
        else
        {
            Node<T>? current = Root;
            Node<T>? parent = null;

            while (current != null)
            {
                parent = current;
                current = item.CompareTo(current.Value) < 0 ? current.Left : current.Right;
            }

            switch (item.CompareTo(parent!.Value))
            {
                case < 0:
                    parent.Left = node;
                    break;
                default:
                    parent.Right = node;
                    break;
            }
        }
        ++Count;
    }
    public virtual bool Remove(T item)
    {
        if (Root == null)
            return false;

        Node<T>? current = Root, parent = null;

        int result;
        do
        {
            result = item.CompareTo(current.Value);
            switch (result)
            {
                case < 0:
                    parent = current;
                    current = current.Left;
                    break;
                case > 0:
                    parent = current;
                    current = current.Right;
                    break;
            }
            if (current == null)
                return false;
        }
        while (result != 0);

        if (current.Right == null)
        {
            if (current == Root)
                Root = current.Left;
            else
            {
                result = current.Value.CompareTo(parent!.Value);
                if (result < 0)
                    parent.Left = current.Left;
                else
                    parent.Right = current.Left;
            }
        }
        else if (current.Right.Left == null)
        {
            current.Right.Left = current.Left;
            if (current == Root)
                Root = current.Right;
            else
            {
                result = current.Value.CompareTo(parent!.Value);
                if (result < 0)
                    parent.Left = current.Right;
                else
                    parent.Right = current.Right;
            }
        }
        else
        {
            Node<T> min = current.Right.Left, prev = current.Right;
            while (min.Left != null)
            {
                prev = min;
                min = min.Left;
            }
            prev.Left = min.Right;
            min.Left = current.Left;
            min.Right = current.Right;

            if (current == Root)
                Root = min;
            else
            {
                result = current.Value.CompareTo(parent!.Value);
                if (result < 0)
                    parent.Left = min;
                else
                    parent.Right = min;
            }
        }
        --Count;
        return true;
    }
    public void Clear()
    {
        Root = null;
        Count = 0;
    }
    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (var value in this)
            array[arrayIndex++] = value;
    }

    public virtual bool IsReadOnly => false;
    public bool Contains(T item)
    {
        var current = Root;
        while (current != null)
        {
            var result = item.CompareTo(current.Value);
            switch (result)
            {
                case 0:
                    return true;
                case < 0:
                    current = current.Left;
                    break;
                default:
                    current = current.Right;
                    break;
            }
        }
        return false;
    }
    #endregion

    #region IEnumerable<T> Members
    public IEnumerator<T> GetEnumerator() => Inorder().GetEnumerator();
    #endregion

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion
}