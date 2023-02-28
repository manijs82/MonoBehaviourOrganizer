
using System.Collections.Generic;

public class Tree<T>
{
    public Node<T> Root;

    public Tree(Node<T> root)
    {
        Root = root;
    }
}

public class Node<T>
{
    public T Value;
    public List<Node<T>> Children;

    public Node(T value)
    {
        Value = value;
    }
}
