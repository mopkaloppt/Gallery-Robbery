using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : Node
{
   public Inverter(string n)
   {
       name =  n;
   }
    public override Status Process()
    {
        // An Inverter will only ever have one child.
        Debug.Log("Processing " + children[0].name);
        Status childStatus = children[0].Process();
        if (childStatus == Status.RUNNING) return Status.RUNNING;
        if (childStatus == Status.FAILURE) 
            return Status.SUCCESS;
        else 
            return Status.FAILURE;
    }
}