using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform currentEnemy;

    [SerializeField] protected float attackCooldown = 1;
    protected float lastTimeAttacked;

    [Header("Tower Setup")]
    [SerializeField] protected Transform towerHead;
    [SerializeField] protected float rotationSpeed = 10;

    [SerializeField] protected float attackRange = 2.5f;
    [SerializeField] protected LayerMask whatIsEnemy;

    protected virtual void Update()
    {
        if (currentEnemy == null)
        {
            currentEnemy = FindRandomEnemyWithinRange();
            return;
        }

        if (CanAttack())
            Attack();

        if (Vector3.Distance(currentEnemy.position, transform.position) > attackRange)       
            currentEnemy = null;        

        RotateTowardsEnemy();
    }

    protected virtual void Attack()
    {
        //Debug.Log("Attacking the enemy" + Time.time);
    }

    protected bool CanAttack()
    {
        if (Time.time > lastTimeAttacked + attackCooldown)
        {
            lastTimeAttacked = Time.time;
            return true;
        }
        return false;
    }

    protected Transform FindRandomEnemyWithinRange()
    {
        List<Transform> possibleTargets = new List<Transform>(); 
        Collider[] enemiesAround = Physics.OverlapSphere(transform.position, attackRange, whatIsEnemy); // Get all the enemies around the tower

        foreach (Collider enemy in enemiesAround)
        {
            possibleTargets.Add(enemy.transform);
        }

        int randomIndex = Random.Range(0, possibleTargets.Count); // Get a random index from the list of enemies

        if (possibleTargets.Count <= 0)
            return null;

        return possibleTargets[randomIndex];
    }

    protected virtual void RotateTowardsEnemy()
    {
        if (currentEnemy == null)
            return;

        Vector3 directionEnemy = currentEnemy.position - towerHead.position; // Get the direction to the enemy

        Quaternion lookRotation = Quaternion.LookRotation(directionEnemy); // Get the rotation to look at the enemy

        Vector3 rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles; // Rotate the tower head to look at the enemy smoothly

        towerHead.rotation = Quaternion.Euler(rotation); // Set the rotation of the tower head
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
