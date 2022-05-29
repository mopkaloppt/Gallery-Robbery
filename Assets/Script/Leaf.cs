using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
    // A Tick is like a frame for the BahaviorTree like Update for Unity
    public delegate Status Tick();
    public Tick ProcessMethod;

    public Leaf() { }
    public Leaf(string n, Tick pm)
    {
        name = n;
        // SUCCESS, RUNNING, FAILURE
        ProcessMethod = pm;
    }
    public override Status Process()
    {
        if(ProcessMethod != null)
        {
            return ProcessMethod();
        }
        return Status.FAILURE;
    }

    
}
