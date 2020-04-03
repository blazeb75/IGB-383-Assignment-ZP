using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enemyState
{
    Patrol,
    Hide,
    Attack
}

[System.Serializable]
public struct DfaRow
{
    [Tooltip("This is just a label of what state the row represents. Do not edit it - it has to match the index.")]
    public enemyState row;
    public bool active;
    public enemyState[] columns;

    public DfaRow(enemyState rowState)
    {
        row = rowState;
        active = true;
        columns = new enemyState[] { enemyState.Patrol, enemyState.Hide, enemyState.Attack };
    }
}

public class Enemy : NavigationAgent
{
    //Player Reference
    Player player;

    //Movement Variables
    public float moveSpeed = 10.0f;
    public float minDistance = 0.1f;

    //FSM Variables
    public enemyState newState = 0;
    private enemyState currentState = 0;
    public int hideIndex = 25;
    
    public DfaRow[] dfaTable = new DfaRow[] { new DfaRow(enemyState.Patrol), new DfaRow(enemyState.Hide), new DfaRow(enemyState.Attack) };

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
        Move();
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
            //Store current node index
            currentNodeIndex = graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNodes>().index;
        }
    }

    //FSM Behaviour - Roam - Randomly select nodes to travel to using Greedy Search Algorithm
    private void Roam()
    {
        Debug.Log(this.name + " is roaming.", this);
        if (Vector3.Distance(transform.position, graphNodes.graphNodes[currentPath[currentPath.Count - 1]].transform.position) <= minDistance)
        {
            //Randomly select new waypoint
            int randomNode = Random.Range(0, graphNodes.graphNodes.Length);
            //Reset current path and add first node - needs to be done here because of recursive function of greedy
            currentPath.Clear();
            greedyPaintList.Clear();
            currentPathIndex = 0;
            currentPath.Add(currentNodeIndex);
            //Greedy Search - navigate towards randomNode
            currentPath = GreedySearch(currentPath[currentPathIndex], randomNode, currentPath);
            //Reverse path and remove final (i.e. initial) position
            currentPath.Reverse();
            currentPath.RemoveAt(currentPath.Count - 1);
        }

    }

    //FSM Behaviour - Move towards hide location using A* Search Algorithm
    private void Hide()
    {
        Debug.Log(this.name + " is hiding.", this);
        //Calculate path towards the node the player is nearest
        if (Vector3.Distance(transform.position, graphNodes.graphNodes[player.currentNodeIndex].transform.position) > minDistance && currentPath[currentPath.Count - 1] != hideIndex)
        {
            //A* Search - navigate towards player
            currentPath = AStarSearch(currentPath[currentPathIndex], hideIndex);
            currentPathIndex = 0;
        }

    }

    //FSM Behaviour - Move towards node closest to player using A* Search Algorithm
    private void Attack()
    {
        Debug.Log(this.name + " is attacking.", this);
        //Calculate path towards the node the player is nearest
        if (Vector3.Distance(transform.position, graphNodes.graphNodes[player.currentNodeIndex].transform.position) > minDistance && currentPath[currentPath.Count - 1] != player.currentNodeIndex)
        {
            //A* Search - navigate towards player
            currentPath = AStarSearch(currentPath[currentPathIndex], player.currentNodeIndex);
            currentPathIndex = 0;
        }
    }

    private void PerformState()
    {
        if (dfaTable[(int)currentState].active == false)
        {
            Debug.LogError("Enemy \""+ gameObject.name +"\" is in a state that is disabled in its DFA table. This should never happen.", this);
        }
        else
        {
            currentState = dfaTable[(int)currentState].columns[(int)newState];
        }

        switch (currentState)
        {
            //Roam
            case enemyState.Patrol:
                Roam();
                break;
            //Hide
            case enemyState.Hide:
                Hide();
                break;
            //Attack
            case enemyState.Attack:
                Attack();
                break;
        }

    }
}
