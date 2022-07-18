using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doggy : MonoBehaviour
{
    public Vector3 currentChunk;
    public int chunkSize = 1000;
    public bool grounded;
    public float rotateTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Store current chunk for use in BuildStars
        currentChunk = new Vector3 (Mathf.Floor(transform.position.x/chunkSize), Mathf.Floor(transform.position.y/chunkSize));

        ButtDown();
    }

    void ButtDown()
    {
        // Search for nearest planet
        GameObject planet = FindClosestPlanet();   
        if (planet == null)
        {
            return;
        } 

        Vector3 toCentre = planet.transform.position - transform.position; 
        float distance = toCentre.magnitude - planet.transform.localScale.x/2;

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
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");

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
}
