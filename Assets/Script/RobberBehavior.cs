using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobberBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Start with 3 actions in a behavior tree
        BehaviorTree tree = new BehaviorTree();
        Node steal = new Node("Steal Something");
        Node goToDiamond = new Node("Go To Diamond");
        Node goToVan = new Node("Go To Van");

        steal.AddChild(goToDiamond);
        steal.AddChild(goToVan);
        tree.AddChild(steal);
        
        tree.PrinTree();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
