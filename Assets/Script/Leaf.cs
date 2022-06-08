using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
    // A Tick is like a frame for the BahaviorTree like Update for Unity
    public delegate Status Tick();
    public Tick ProcessMethod;

    public delegate Status TickMulti(int val);
    public TickMulti ProcessMethodMulti;
    public int index;

    public Leaf() { }
    public Leaf(string n, Tick pm)
    {
        name = n;
        // SUCCESS, RUNNING, FAILURE
        ProcessMethod = pm;
    }
    public Leaf(string n, int i, TickMulti pm)
    {
        name = n;
        index = i;
        // SUCCESS, RUNNING, FAILURE
        ProcessMethodMulti = pm;
    }
    public Leaf(string n, Tick pm, int _sortOrder)
    {
        name = n;
        // SUCCESS, RUNNING, FAILURE
        ProcessMethod = pm;
        sortOrder = _sortOrder;
    }
    public override Status Process()
    {
        if(ProcessMethod != null)
        {
            return ProcessMethod();
        }
        else if(ProcessMethodMulti != null)
        {
            return ProcessMethodMulti(index);
        }
        return Status.FAILURE;
    }

    
}
