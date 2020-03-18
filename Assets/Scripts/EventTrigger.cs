using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public enum TriggerState
    {
        Patrol,
        Hide,
        Attack
    }
    [Header("Settings")]
    public TriggerState enter;
    public TriggerState exit;
    [Header("Exposed Variables (Do not edit)")]
    public Enemy[] enemies;

    private void Start()
    {
        enemies = FindObjectsOfType<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeEnemyStates((int)enter);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeEnemyStates((int)exit);
        }
    }

    void ChangeEnemyStates(int state)
    {
        foreach(Enemy enemy in enemies)
        {
            enemy.newState = state;
        }
    }
}
