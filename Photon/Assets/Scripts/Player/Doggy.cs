using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doggy : MonoBehaviour
{
    public Vector3 currentChunk;
    public int chunkSize = 1000;
    public bool grounded;
    public float rotateTime;

    private GameObject[] planets;
    private GameObject planet;

    void Start()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");
    }

    void Update()
    {
        // Store current chunk for use in BuildStars
        currentChunk = new Vector3 (Mathf.Floor(transform.position.x/chunkSize), Mathf.Floor(transform.position.y/chunkSize));

        ButtDown();

        // Attach/Detach player from orbitting body
        if (grounded && planet.transform.parent.tag == "OrbitCentre")
        {
            AttachToPlanet();
        }
        else
        {   
            transform.parent = null;
        }
    }

    void ButtDown()
    {
        // Search for nearest planet
        planet = FindClosestPlanet();   
        if (planet == null)
        {
            return;
        } 
        Debug.DrawLine(transform.position, planet.transform.position);
        Vector3 toCentre = planet.transform.position - transform.position; 
        float distance = toCentre.magnitude - planet.transform.lossyScale.x/2;

        // Allow longer distance when player is grounded
        if (( grounded && distance < 100f ) || 
            (!grounded && distance < 10f  ))   
        {
            Vector3 rotatedCentre = Vector3.Cross(toCentre, transform.forward);
            float angle = Mathf.Atan2(rotatedCentre.y, rotatedCentre.x) * Mathf.Rad2Deg + 180;
            Reorient(Quaternion.Euler(0, 0, angle), rotateTime);

            grounded = true;
        }
        else
        {
            Reorient(Quaternion.identity, 0.5f);

            grounded = false;
        }
    }

    GameObject FindClosestPlanet()
    {
        if (planets.Length == 0) 
        { 
            return null;
        }

        float distMin = -1;
        int distMin_ind = -1;
        
        for (int planet_ind = 0; planet_ind < planets.Length; planet_ind ++)
        {
            Vector3 planetVect = planets[planet_ind].transform.position - transform.position;
            float dist = planetVect.magnitude;
            
            // Store closest planet
            if (dist < distMin || distMin == -1)
            {
                distMin = dist;
                distMin_ind = planet_ind;
            }
        }
        return planets[distMin_ind];
    }
    
    void Reorient(Quaternion rot, float rotTime)
    {
        StartCoroutine(SmoothRotate(rot,rotTime));
    }

    IEnumerator SmoothRotate(Quaternion rot, float rotTime)
    {
        float timeElapsed = 0;

        Quaternion startRot = transform.rotation;

        while (timeElapsed < rotTime)
        {   
            transform.rotation = Quaternion.Lerp(startRot, rot, timeElapsed/rotTime);

            timeElapsed += Time.deltaTime;        
            yield return null;
        }

        transform.rotation = rot;
    }

    void AttachToPlanet()
    {
        // Maintain radius from planet 
        if (planet == null)
        {
            return;
        }

        if (planet.transform.parent.tag == "OrbitCentre")
        {
            transform.SetParent(planet.transform.parent);
        }
    }
}
