using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;

	public Projectile projectile;
	public float msBetweenShots = 100;
	public float muzzleVelocity = 35;

	float nextShotTime;

	Utils utils;

	public void Shoot(Vector3 dir) {
		utils = new Utils();
		// dir.z = 0;

		if (Time.time > nextShotTime) {
			nextShotTime = Time.time + msBetweenShots / 1000;
			Projectile newProjectile = Instantiate (projectile, muzzle.position, Quaternion.FromToRotation(transform.up, dir.normalized)) as Projectile;
			newProjectile.SetVelocity(muzzleVelocity*dir.normalized + dir);
		}
	}
}
