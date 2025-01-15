using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Enemy currentEnemy;

    [SerializeField] protected float attackCooldown = 1;
    protected float lastTimeAttacked;

    [Header("Tower Setup")]
    [SerializeField] protected EnemyType enemyPriorityType = EnemyType.None;
    [SerializeField] protected Transform towerHead;
    [SerializeField] protected float rotationSpeed = 10;
    private bool canRotate;

    [SerializeField] protected float attackRange = 2.5f;
    [SerializeField] protected LayerMask whatIsEnemy;

    [Space]
    [Tooltip("Enabling this allows tower to change target beetwen attacks")]
    [SerializeField] private bool dynamicTargetChange;
    private float targetCheckInterval = .1f;
    private float lastTimeCheckedTarget;

    protected virtual void Awake()
    {
        EnableRotation(true);
    }

    protected virtual void Update()
    {
        UpdateTargetIfNeeded();

        if (currentEnemy == null)
        {
            currentEnemy = FindEnemyWithinRange();
            return;
        }

        if (CanAttack())
            Attack();

        if (Vector3.Distance(currentEnemy.CenterPoint(), transform.position) > attackRange)
            currentEnemy = null;

        RotateTowardsEnemy();
    }

    private void UpdateTargetIfNeeded()
    {
        if (!dynamicTargetChange)
            return;

        if (Time.time > lastTimeCheckedTarget + targetCheckInterval)
        {
            lastTimeCheckedTarget = Time.time;
            currentEnemy = FindEnemyWithinRange();
        }
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

    protected Enemy FindEnemyWithinRange()
    {
        List<Enemy> priorityTargets = new List<Enemy>();
        List<Enemy> possibleTargets = new List<Enemy>();
        Collider[] enemiesAround = Physics.OverlapSphere(transform.position, attackRange, whatIsEnemy); // Get all the enemies around the tower

        foreach (Collider enemy in enemiesAround)
        {
            Enemy newEnemy = enemy.GetComponent<Enemy>();
            EnemyType newEnemyType = newEnemy.GetEnemyType();

            if (newEnemyType == enemyPriorityType)
                priorityTargets.Add(newEnemy);
            else
                possibleTargets.Add(newEnemy);            
        }        

        if (priorityTargets.Count > 0)
            return GetMostAdvancedEnemy(priorityTargets);

        if (possibleTargets.Count > 0)
            return GetMostAdvancedEnemy(possibleTargets);        

        return null;
    }

    public Enemy GetMostAdvancedEnemy(List<Enemy> targets)
    {
        Enemy mostAdvanceEnemy = null;
        float minRemainingDistance = float.MaxValue; // Set the minimum distance to the maximum value possible
        foreach (Enemy enemy in targets)
        {
            float remainingDistance = enemy.DistanceToFinishLine(); // Get the remaining distance to the finish line of the enemy
            if (remainingDistance < minRemainingDistance) // Check if the remaining distance is less than the minimum distance found so far
            {
                minRemainingDistance = remainingDistance;
                mostAdvanceEnemy = enemy;
            }
        }
        return mostAdvanceEnemy;
    }

    public void EnableRotation(bool enable)
    {
        canRotate = enable;
    }

    protected virtual void RotateTowardsEnemy()
    {
        if (!canRotate)
            return;

        if (currentEnemy == null)
            return;

        Vector3 directionEnemy = DirectionEnemyFrom(towerHead); // Get the direction to the enemy

        Quaternion lookRotation = Quaternion.LookRotation(directionEnemy); // Get the rotation to look at the enemy

        Vector3 rotation = Quaternion.Lerp(towerHead.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles; // Rotate the tower head to look at the enemy smoothly

        towerHead.rotation = Quaternion.Euler(rotation); // Set the rotation of the tower head
    }

    protected Vector3 DirectionEnemyFrom(Transform startPoint)
    {
        return (currentEnemy.CenterPoint() - startPoint.position).normalized;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
