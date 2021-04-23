using System;

public class RoadBSPTree
{
    //private RoadBSPTree leftTree;
    //private RoadBSPTree rightTree;
    //private RoadBSPTree parentTree;

    //RoadBSPTree getLeftTree() { return leftTree; }
    //RoadBSPTree getRightTree() { return rightTree; }
    //RoadBSPTree getparentTree() { return parentTree; }
}

public class BinaryTreeNode<T>
{
    public T Data { get; set; }
    public BinaryTreeNode<T> Left { get; set; }
    public BinaryTreeNode<T> Right { get; set; }

    public BinaryTreeNode(T data)
    {
        this.Data = data;
    }
}

public class BinaryTree<T>
{
    public BinaryTreeNode<T> Root { get; set; }

    // 데이터 출력 예
    public void PreOrderTraversal(BinaryTreeNode<T> node)
    {
        if (node == null) return;

        Console.WriteLine(node.Data);
        PreOrderTraversal(node.Left);
        PreOrderTraversal(node.Right);
    }
}

// 예제
class Program
{
    static void Main(String[] args)
    {
        BinaryTree<int> btree = new BinaryTree<int>();
        btree.Root = new BinaryTreeNode<int>(1);
        btree.Root.Left = new BinaryTreeNode<int>(2);
        btree.Root.Right = new BinaryTreeNode<int>(3);
        btree.Root.Left.Left = new BinaryTreeNode<int>(4);
    }
}