using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTAgent : MonoBehaviour
{
    
    public NavMeshAgent agent;
    public BehaviorTree tree;
    
    public enum ActionState { IDLE, WORKING };
    public ActionState state = ActionState.IDLE;

    public Node.Status treeStatus = Node.Status.RUNNING;

    // Start is called before the first frame update
    public void Start()
    {
        // Declare agent which is a NavMesh
        agent = GetComponent<NavMeshAgent>();
        tree = new BehaviorTree();
    }
    
    public Node.Status GoToLocation(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        }
        // Agent didn't reach the destination
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }
    // Update is called once per frame
    void Update()
    {   
        // Not equal SUCCESS means that the money falls below 500, so the agent is triggered to steal
        if (treeStatus != Node.Status.SUCCESS)
        {
            // Triggering Steal sequence
            treeStatus = tree.Process();
        }            
    }
}
