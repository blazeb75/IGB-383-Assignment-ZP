using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NavigationAgent
{

    //Player Reference
    Player player;

    //Movement Variables
    public float moveSpeed = 10.0f;
    public float minDistance = 0.1f;

    //FSM Variables
    public int newState = 0;
    private int currentState = 0;
    private int hideIndex = 25;


    // Use this for initialization
    void Start()
    {
        //Find waypoint graph
        graphNodes = GameObject.FindGameObjectWithTag("waypoint graph").GetComponent<WaypointGraph>();
        //Initial node index to move to
        currentPath.Add(currentNodeIndex);
        //Establish reference to player game object
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

        PerformState();
        //Move();
    }

    //Move Enemy
    private void Move()
    {

        if (currentPath.Count > 0)
        {

            //Move towards next node in path
            transform.position = Vector3.MoveTowards(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position, moveSpeed * Time.deltaTime);

            //Increase path index
            if (Vector3.Distance(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position) <= minDistance)
            {

                if (currentPathIndex < currentPath.Count - 1)
                    currentPathIndex++;
            }

            currentNodeIndex = graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNodes>().index;   //Store current node index
        }
    }

    //FSM Behaviour - Roam - Randomly select nodes to travel to using Greedy Search Algorithm
    private void Roam()
    {
        Debug.Log(this.name + " is roaming.", this);
    }

    //FSM Behaviour - Move towards hide location using A* Search Algorithm
    private void Hide()
    {
        Debug.Log(this.name + " is hiding.", this);
    }

    //FSM Behaviour - Move towards node closest to player using A* Search Algorithm
    private void Attack()
    {
        Debug.Log(this.name + " is attacking.", this);
    }

    private void PerformState()
    {
        currentState = newState;
        switch (currentState)
        {
            //Roam
            case 0:
                Roam();
                break;
            //Hide
            case 1:
                Hide();
                break;
            //Attack
            case 2:
                Attack();
                break;
        }

    }
}
