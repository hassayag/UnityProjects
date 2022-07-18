using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;

	public Projectile projectile;
	public float msBetweenShots = 100;
	public float muzzleVelocity = 35;
	public float baseDamage = 1f;
	
	float range = 100f;

	public bool isJet = false;
    public float kickbackForce;

	private Rigidbody2D playerRigidbody;
	private SpriteRenderer fireSprite;
	float nextShotTime;

	Utils utils;

	void Start()
	{
		utils = new Utils();

        playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

		if (isJet)
		{
			fireSprite = transform.GetChild(2).GetComponent<SpriteRenderer>();
		}
		else
		{
			range = transform.parent.transform.parent.GetComponent<Turret>().range;
		}
	}

	public void Shoot(Vector3 dir) {		
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
			newProjectile.GetComponent<Projectile>().Init(range, muzzleVelocity*dir.normalized + dir, baseDamage);
		}
	}

	void FireJet(Vector3 dir)
	{
		playerRigidbody.AddForce(-dir * kickbackForce * Time.deltaTime);
		fireSprite.enabled = true;
	}

	void StopJet()
	{
		fireSprite.enabled = false;
	}
}
