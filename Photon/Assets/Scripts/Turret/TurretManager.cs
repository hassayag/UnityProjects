using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    public GameObject selectedComponent;
    private Transform homePlanetCentre;

    void Start()
    {
        // Get orbit centre of home planet
        selectedComponent = null;
        homePlanetCentre = GameObject.Find("HomePlanet").GetComponent<Planet>().transform.GetChild(0).transform;
    }

    void Update()
    {   
        if (Input.GetMouseButtonDown(0) && selectedComponent != null)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -1*Camera.main.transform.position.z));

            Collider2D hitCollider = Physics2D.OverlapBox(mouseWorldPos, selectedComponent.transform.localScale / 2, 0);
            if (hitCollider != null)
            {
                return;
            }

            GameObject newTurret = Instantiate(selectedComponent, mouseWorldPos, Quaternion.identity);

            // Set orbit dist and speed
            Orbit orbit = newTurret.GetComponent<Orbit>();
            orbit.orbitCentre = homePlanetCentre.transform;
        }
    }

    public void clickFromUI(GameObject turret = null)
    {
        selectedComponent = turret; 
        if (turret.name == "None")
        {
            selectedComponent = null;
        }
    }
}
