using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;

	public Projectile projectile;
	public float msBetweenShots = 100;
	public float muzzleVelocity = 35;

	public bool isJet = false;
    public float kickbackForce;

	private Rigidbody2D playerRigidbody;
	private SpriteRenderer fireSprite;
	float nextShotTime;

	Utils utils;

	void Start()
	{
        playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
		fireSprite = transform.Find("Fire").GetComponent<SpriteRenderer>();
	}

	public void Shoot(Vector3 dir) {
		utils = new Utils();
		
		if (isJet)
		{
			FireJet(dir);
		}
		else
		{
			ShootGun(dir);
		}
	}

	public void StopShoot() 
	{
		if (isJet)
		{
			StopJet();
		}
	}

	void ShootGun(Vector3 dir)
	{
		if (Time.time > nextShotTime) {
			nextShotTime = Time.time + msBetweenShots / 1000;
			Projectile newProjectile = Instantiate (projectile, muzzle.position, Quaternion.FromToRotation(transform.up, dir.normalized)) as Projectile;
			newProjectile.SetVelocity(muzzleVelocity*dir.normalized + dir);
		}
	}

	void FireJet(Vector3 dir)
	{
		playerRigidbody.AddForce(-dir * kickbackForce);
		fireSprite.enabled = true;
	}

	void StopJet()
	{
		fireSprite.enabled = false;
	}
}
