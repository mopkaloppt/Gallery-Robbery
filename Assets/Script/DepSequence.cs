using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DepSequence : Node
{
    BehaviorTree dependency;
    NavMeshAgent agent;

    public DepSequence(string n, BehaviorTree d, NavMeshAgent a)
    {
       name =  n;
       dependency = d;
       agent = a;
    }
    public override Status Process()
    {
        if (dependency.Process() == Status.FAILURE)
        {
            agent.ResetPath();
            // Reset all children
            foreach (Node n in children)
            {
                n.Reset();
            }
            return Status.FAILURE;
        }
        Debug.Log("Processing " + children[currentChild].name);
        // We are looping through each child for an action node (e.g. steal)
        Status childStatus = children[currentChild].Process();
        if (childStatus == Status.RUNNING) return Status.RUNNING;
        if (childStatus == Status.FAILURE) return Status.FAILURE;
        // Here we know the currentChild Status is SUCCESS, move on to the next child
        currentChild++;
        if (currentChild >= children.Count)
        {
            currentChild = 0;
            return Status.SUCCESS;
        }
        return Status.RUNNING;
    }
}