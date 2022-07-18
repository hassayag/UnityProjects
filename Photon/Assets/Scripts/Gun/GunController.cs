using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
	public Transform weaponHold;
	public Gun startingGun;
	public Gun equippedGun;

	void Awake() {
		if (startingGun != null) {
			EquipGun(startingGun);
		}
	}

	public void EquipGun(Gun gunToEquip) {
		if (equippedGun != null) {
			Destroy(equippedGun.gameObject);
		}
		equippedGun = Instantiate (gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
		equippedGun.transform.SetParent(weaponHold);
	}

	public void Shoot(Vector3 dir) {
		if (equippedGun != null) {
			equippedGun.Shoot(dir);
		}
	}

	public void StopShoot() {
		if (equippedGun != null) {
			equippedGun.StopShoot();
		}
	}
}