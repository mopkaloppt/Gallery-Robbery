using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehavior : BTAgent
{
    public GameObject cop;
    public GameObject[] art;
    public GameObject diamond;
    public GameObject stolenDiamond;
    public GameObject painting;
    public GameObject stolen;
    public GameObject van;
    public GameObject backdoor;
    public GameObject frontdoor;

    [Range(0, 1000)]
    public int money = 800;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        // Start with 3 actions in a behavior tree
        Sequence steal = new Sequence("Steal Something");
        Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);

        RSelector objectToSteal = new RSelector("Steal More!");
        for (int i = 0; i < art.Length; i++)
        {
            Leaf gotoart = new Leaf("Go to " + art[i].name, i, GoToArt);
            objectToSteal.AddChild(gotoart);
        }
        
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond, 1);
        Leaf goToPainting = new Leaf("Go To Painting", GoToPainting, 2);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        Leaf goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor, 2);
        Leaf goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor, 1);
        PSelector openDoor = new PSelector("Open Door");
        
        openDoor.AddChild(goToFrontDoor);
        openDoor.AddChild(goToBackDoor);
        
        Inverter invertMoney = new Inverter("Invert Money");
        invertMoney.AddChild(hasGotMoney); // hasn't got money

        steal.AddChild(invertMoney);
        steal.AddChild(openDoor);
        steal.AddChild(objectToSteal);
        steal.AddChild(goToVan);

        Sequence runAway = new Sequence("Run Away");
        Leaf canSeeCop = new Leaf("Saw Cop!", CanSeeCop);
        Leaf fleeFromCop = new Leaf("Flee!", FleeFromCop);
        runAway.AddChild(canSeeCop);
        runAway.AddChild(fleeFromCop);

        tree.AddChild(runAway);
        tree.PrinTree();
    }
    public Node.Status CanSeeCop()
    {
        return CanSee(cop.transform.position, "Cop", 20, 90);
    }

    public Node.Status FleeFromCop()
    {
        return Flee(cop.transform.position, 20);
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
        if (!diamond.activeSelf) return Node.Status.FAILURE;
        Node.Status diamondStatus = GoToLocation(diamond.transform.position);
        if (diamondStatus == Node.Status.SUCCESS)
        {     
            // child the diamond to that empty game object
            diamond.transform.parent = stolenDiamond.transform;
            // set its position to the same position as the empty game object
            diamond.transform.position = stolenDiamond.transform.position;
            stolen = diamond;
            return diamondStatus;
        }
        else    
            return diamondStatus;       
    }
    public Node.Status GoToPainting()
    {
        if (!painting.activeSelf) return Node.Status.FAILURE;
        Node.Status paintingStatus = GoToLocation(painting.transform.position);
        if (paintingStatus == Node.Status.SUCCESS)
        {   // diamond.transform.parent is Robber Unity Engine Transform
            painting.transform.parent = this.gameObject.transform;
            stolen = painting;
            return paintingStatus;
        }
        else    
            return paintingStatus;       
    }
    public Node.Status GoToArt(int i)
    {
        if (!art[i].activeSelf) return Node.Status.FAILURE;
        Node.Status s = GoToLocation(art[i].transform.position);
        if (s == Node.Status.SUCCESS)
        {   // diamond.transform.parent is Robber Unity Engine Transform
            art[i].transform.parent = this.gameObject.transform;
            stolen = art[i];
            return s;
        }
        else    
            return s;       
    }
    
    public Node.Status GoToVan()
    {
        Node.Status vanStatus = GoToLocation(van.transform.position);
        if (vanStatus == Node.Status.SUCCESS)
        {             
            money += 300;
            stolen.SetActive(false);
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
                door.GetComponent<NavMeshObstacle>().enabled = false;
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else
            return doorStatus;
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
