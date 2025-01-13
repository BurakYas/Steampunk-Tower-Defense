using UnityEngine;

public class Tower_Crossbow : Tower
{
    private Crossbow_Visuals visuals;

    [Header("Crossbow Details")]
    [SerializeField] private Transform gunPoint;

    protected override void Awake()
    {
        base.Awake();
        visuals = GetComponent<Crossbow_Visuals>();
    }

    protected override void Attack()
    {
        Vector3 directionToEnemy = DirectionEnemyFrom(gunPoint);

        if (Physics.Raycast(gunPoint.position, directionToEnemy, out RaycastHit hitInfo, Mathf.Infinity))
        {
            towerHead.forward = directionToEnemy;

            Debug.Log(hitInfo.collider.gameObject.name + " was attacked!");
            Debug.DrawLine(gunPoint.position, hitInfo.point);

            visuals.PlayAttackVFX(gunPoint.position, hitInfo.point);
            visuals.PlayReoladFX(attackCooldown);
        }
    }
}
