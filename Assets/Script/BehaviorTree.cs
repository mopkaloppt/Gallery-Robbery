using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : Node
{
    public BehaviorTree()
    {
        name = "BehaviorTree";
    }

    public BehaviorTree(string n)
    {
        name = n;
    } 
    struct NodeLevel
    {
        public int level;
        public Node node;
    }

    public void PrinTree()
    {
        string treePrintOut = "";
        Stack<NodeLevel> nodeStack = new Stack<NodeLevel>();
        Node currentNode = this;
        nodeStack.Push(new NodeLevel { level=0, node=currentNode });

        while(nodeStack.Count != 0)
        {
            NodeLevel nextNodeLevel = nodeStack.Pop();
            treePrintOut += new string('-', nextNodeLevel.level) + nextNodeLevel.node.name + "\n";
            for(int i = nextNodeLevel.node.children.Count - 1; i>=0; i--)
            {
                nodeStack.Push(new NodeLevel{ level=nextNodeLevel.level+1, node=nextNodeLevel.node.children[i] });
            }
        }
        Debug.Log(treePrintOut);
    }
}
