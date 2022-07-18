using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float range = 100f;
    public float rotationTime = 1f;

    GameObject[] enemies;
    GameObject target;
    LayerMask layerMask;
    GunController gunController;
    Gun gun;

    void Start()
    {
        // Mask for raycast
        layerMask = LayerMask.GetMask("Planet", "Enemy");

        // Get Gun
        gunController = GetComponent<GunController>();
        gun = gunController.equippedGun;
    }

    void Update()
    {
        // Get list of current enemies
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        // TODO: Fix locking onto next enemy
        if (target == null)
        {
            target = findClosestEnemy();
        }
        else
        {
            // Lead shot based on target's velocity
            Vector3 futureTargetPos = getFuturePos(target);

            Vector3 targetDir = futureTargetPos - transform.position;

            // Debug.DrawRay(transform.position, targetDir);

            rotateGun(targetDir);

            // Shoot gun
            gunController.Shoot(gun.transform.right);
        }
    }

    GameObject findClosestEnemy()
    {
        GameObject closestEnemy = null;
        float closestDistance = -1;

        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPos = enemy.transform.position;
            if (!hasLineOfSight(enemyPos, range))
            {
                continue;
            }

            Vector3 vectorToEnemy = enemyPos - transform.position;
            float distanceToEnemy = vectorToEnemy.magnitude;

            if (closestDistance == -1 || distanceToEnemy < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = distanceToEnemy;
            }
        }

        return closestEnemy;
    }

    bool hasLineOfSight(Vector3 targetPos, float range)
    {
        Vector3 rayDirection = targetPos - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection.normalized, range, layerMask);

        // Check if we hit an enemy with raycast
        if (hit.collider != null)
        {
            if (hit.transform.tag == "Enemy") 
            {
                return true;
            }
        }

        return false;
    }

    void rotateGun(Vector3 dir)
    {
        // Rotate gun towards target
        StartCoroutine(smoothRotate(dir));
    }

    IEnumerator smoothRotate(Vector3 dir)
    {
        float timeElapsed = 0;
        var startRotation = gun.transform.rotation;

        dir.z = 0;
        var finalRotation = Quaternion.FromToRotation(gun.transform.right, dir);

        while (timeElapsed < rotationTime)
        {   
            gun.transform.rotation = Quaternion.Lerp(startRotation, finalRotation, timeElapsed/rotationTime);

            timeElapsed += Time.deltaTime;        
            yield return null;
        }

        gun.transform.rotation = finalRotation;
    }

    Vector3 getFuturePos(GameObject target)
    {
        Vector3 targetVelocity = target.GetComponent<Rigidbody2D>().velocity;

        // Adjust position based on length of gun
        Vector3 muzzlePos = transform.position + gun.transform.right;

        float bulletTravelTime = (target.transform.position - muzzlePos).magnitude / (gun.muzzleVelocity + 100f);
        
        Vector3 futurePos = target.transform.position + targetVelocity * bulletTravelTime;

        Debug.DrawLine(target.transform.position, muzzlePos, Color.red);

        return futurePos;
    }
}
