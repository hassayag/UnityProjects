using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float range = 100f;
    public float rotationSpeed = 1f;

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

        if (target == null)
        {
            target = findClosestEnemy();
        }
        else
        {
            // check if target is dead OR out of range OR out of sight
            if (targetIsDead() || targetOutOfRange() || !hasLineOfSight(target.transform.position))
            {
                target = null;
            }
            else
            {
                // Lead shot based on target's velocity
                Vector3 futureTargetPos = getFuturePos(target);

                Vector3 targetDir = futureTargetPos - transform.position;

                rotateGun(targetDir);

                // Shoot gun
                gunController.Shoot(gun.transform.right);
            }
        }
    }

    bool targetIsDead()
    {
        if (target.GetComponent<LivingEntity>().getHealth() < 1)
        {
            return true;
        }

        return false;
    }

    bool targetOutOfRange()
    {
        float dist = (target.transform.position - transform.position).magnitude;

        if (dist > range)
        {
            return true;
        }

        return false;
    }

    GameObject findClosestEnemy()
    {
        GameObject closestEnemy = null;
        float closestDistance = -1;

        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPos = enemy.transform.position;
            if (!hasLineOfSight(enemyPos))
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

    bool hasLineOfSight(Vector3 targetPos)
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
        // gun.transform.rotation *= Quaternion.FromToRotation(gun.transform.right, dir);;

        StartCoroutine(smoothRotate(dir));
    }

    IEnumerator smoothRotate(Vector3 dir)
    {
        dir.z = 0;

        float timeElapsed = 0;
        var startRotation = gun.transform.rotation;
        var finalRotation = startRotation * Quaternion.FromToRotation(gun.transform.right, dir);

        float lerpDuration = Mathf.Abs((finalRotation.eulerAngles.z - startRotation.eulerAngles.z)/rotationSpeed);

        while (timeElapsed < lerpDuration)
        {   
            gun.transform.rotation = Quaternion.Lerp(startRotation, finalRotation, timeElapsed/lerpDuration);

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

        float bulletTravelTime = (target.transform.position - muzzlePos).magnitude / (gun.muzzleVelocity + 70f);
        
        Vector3 futurePos = target.transform.position + targetVelocity * bulletTravelTime;

        Debug.DrawLine(target.transform.position, muzzlePos, Color.red);

        return futurePos;
    }
}
