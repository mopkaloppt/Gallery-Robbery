using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehavior : MonoBehaviour
{
    public GameObject diamond;
    public GameObject van;
     public GameObject door;
    NavMeshAgent agent;
    BehaviorTree tree = new BehaviorTree();
    public enum ActionState { IDLE, WORKING };
    ActionState state = ActionState.IDLE;
    Node.Status treeStatus = Node.Status.RUNNING;
    // Start is called before the first frame update
    void Start()
    {
        // Declare agent which is a NavMesh
        agent = GetComponent<NavMeshAgent>();

        // Start with 3 actions in a behavior tree
        Sequence steal = new Sequence("Steal Something");
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        Leaf goToDoor = new Leaf("Go To Door", GoToDoor);
        
        steal.AddChild(goToDoor);
        steal.AddChild(goToDiamond);
        steal.AddChild(goToDoor);
        steal.AddChild(goToVan);
        tree.AddChild(steal);

        tree.PrinTree();
    }
    public Node.Status GoToDiamond()
    {
        return GoToLocation(diamond.transform.position);
    } 
    public Node.Status GoToVan()
    {
        return GoToLocation(van.transform.position);
    }
    public Node.Status GoToDoor()
    {
        return GoToLocation(door.transform.position);
    }
    public Node.Status GoToLocation(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
            Debug.Log(state);
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
        if (treeStatus == Node.Status.RUNNING)
        {
            treeStatus = tree.Process();
            Debug.Log(treeStatus);
        }
    }
}
