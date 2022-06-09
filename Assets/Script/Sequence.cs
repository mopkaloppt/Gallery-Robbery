using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
   public Sequence(string n)
   {
       name =  n;
   }
    public override Status Process()
    {
        Debug.Log("Processing " + children[currentChild].name);
        // We are looping through each child for an action node (e.g. steal)
        Status childStatus = children[currentChild].Process();
        if (childStatus == Status.RUNNING) return Status.RUNNING;
        if (childStatus == Status.FAILURE)
        {
            currentChild = 0;
            foreach (Node n in children)
            {
                n.Reset();
                return Status.FAILURE;
            }     
        } 
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