using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehavior : MonoBehaviour
{
    public GameObject diamond;
    public GameObject van;
    public GameObject backdoor;
    public GameObject frontdoor;

    NavMeshAgent agent;
    
    public enum ActionState { IDLE, WORKING };
    ActionState state = ActionState.IDLE;

    BehaviorTree tree = new BehaviorTree();
    Node.Status treeStatus = Node.Status.RUNNING;

    [Range(0, 1000)]
    public int money = 800;

    // Start is called before the first frame update
    void Start()
    {
        // Declare agent which is a NavMesh
        agent = GetComponent<NavMeshAgent>();

        // Start with 3 actions in a behavior tree
        Sequence steal = new Sequence("Steal Something");
        Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        Leaf goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor);
        Leaf goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor);
        Selector openDoor = new Selector("Open Door");
        
        openDoor.AddChild(goToFrontDoor);
        openDoor.AddChild(goToBackDoor);
        
        Inverter invertMoney = new Inverter("Invert Money");
        invertMoney.AddChild(hasGotMoney); // hasn't got money

        steal.AddChild(invertMoney);
        steal.AddChild(openDoor);
        steal.AddChild(goToDiamond);
        steal.AddChild(goToVan);
        tree.AddChild(steal);

        tree.PrinTree();
    }
    public Node.Status HasMoney()
    {
        // Steal if money < 500 or even if the agent doesn't have any money, steal anyway
        if (money < 500)
        {
            // The status of this node will be inverted in invertMoney.AddChild(hasGotMoney);
            // Essentially, it will not return FAILURE so that tree can kick off its process.
            return Node.Status.FAILURE;
        }
        return Node.Status.SUCCESS;    
    }
    public Node.Status GoToDiamond()
    {
        Node.Status diamondStatus = GoToLocation(diamond.transform.position);
        if (diamondStatus == Node.Status.SUCCESS)
        {   // diamond.transform.parent is Robber Unity Engine Transform
            diamond.transform.parent = this.gameObject.transform;
            return diamondStatus;
        }
        else    
            return diamondStatus;       
    } 
    public Node.Status GoToVan()
    {
        Node.Status vanStatus = GoToLocation(van.transform.position);
        if (vanStatus == Node.Status.SUCCESS)
        {             
            money += 300;
            diamond.SetActive(false);
        }
        return vanStatus;
    }
    public Node.Status GoToBackDoor()
    {
        return GotoToDoor(backdoor);
    }
    public Node.Status GoToFrontDoor()
    {
        return GotoToDoor(frontdoor);
    }
    public Node.Status GotoToDoor(GameObject door)
    {
        Node.Status doorStatus = GoToLocation(door.transform.position);
        if (doorStatus == Node.Status.SUCCESS)
        {
            if (!door.GetComponent<Lock>().isLocked)
            {
                door.SetActive(false);
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else
            return doorStatus;
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
