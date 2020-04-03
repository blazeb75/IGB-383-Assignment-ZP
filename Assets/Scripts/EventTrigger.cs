using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [Header("Settings")]
    public enemyState enter;
    public enemyState exit;
    [Tooltip("Populate with the enemies this trigger affects. Leave blank to affect all enemies.")]
    public Enemy[] enemies;

    private void Start()
    {
        if (enemies.Length == 0)
            enemies = FindObjectsOfType<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeEnemyStates(enter);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeEnemyStates(exit);
        }
    }

    void ChangeEnemyStates(enemyState state)
    {
        foreach(Enemy enemy in enemies)
        {
            enemy.newState = state;
        }
    }
}
