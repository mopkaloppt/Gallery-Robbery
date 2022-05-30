using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    public Selector(string n)
   {
       name =  n;
   }
    public override Status Process()
    {
        Debug.Log("Processing " + children[currentChild].name);
        // We are looping through each child for an action node (e.g. steal)
        Status childStatus = children[currentChild].Process();
        if (childStatus == Status.RUNNING) return Status.RUNNING;
        if (childStatus == Status.SUCCESS)
        {
            currentChild = 0;
            return Status.SUCCESS;
        }
        // If the currentChild failed, move on to the next child anyway.
        currentChild++;                
        if (currentChild >= children.Count)
        {
            currentChild = 0;
            return Status.FAILURE;
        }
        return Status.RUNNING;
    }
}