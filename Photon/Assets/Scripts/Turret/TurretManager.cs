using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    public GameObject fixedPosSymbol; 
    public float snapThresh, playPauseRampTime;

    private Transform selectedComponent, transSelectedComponent;
    private Transform homePlanetCentre;
    private Vector3 mouseWorldPos;
    private Transform symbolGroup;
    private Utils utils;

    private List<Vector2> fixedPositions;

    void Start()
    {
        utils = new Utils();

        // Get orbit centre of home planet
        selectedComponent = null;
        homePlanetCentre = GameObject.Find("HomePlanet").GetComponent<Planet>().transform.GetChild(0).transform;

        buildFixedPositions();
        
        // Disable symbols
        symbolGroup.gameObject.SetActive(false);
    }

    void buildFixedPositions()
    {
        List<float> radiusList = new List<float>{200f, 300f, 400f};
        int radialSegments = 8;

        symbolGroup = Instantiate(new GameObject(), homePlanetCentre.position, Quaternion.identity).transform;
        fixedPositions = new List<Vector2>();

        foreach (float radius in radiusList)
        {
            for (int i=0; i<radialSegments; i++)
            {
                float theta = i * 2*Mathf.PI/radialSegments;
                Vector2 pos = new Vector2(homePlanetCentre.transform.position.x + radius*Mathf.Cos(theta), 
                    homePlanetCentre.transform.position.y + radius*Mathf.Sin(theta));

                // Display the fixed point
                Transform symbol = Instantiate(fixedPosSymbol, pos, Quaternion.identity).transform;

                symbol.transform.SetParent(symbolGroup);

                // Store position   
                fixedPositions.Add(pos);
            }
        }
    }

    void Update()
    {   
        if (selectedComponent != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -1*Camera.main.transform.position.z));

            (Vector2 spotPos, bool foundSpot) = findClosestSpot(mouseWorldPos);

            // Left click to place current component
            if (Input.GetMouseButtonDown(0) && foundSpot)
            {
                placeComponent(spotPos);
            }
            // Right click to deselect component
            else if (Input.GetMouseButtonDown(1))
            {
                selectedComponent = null;
                hideGhost();
            }

            // Set ghost to follow mouse
            if (transSelectedComponent != null)
            {
                transSelectedComponent.position = spotPos;

                transSelectedComponent.rotation *= Quaternion.FromToRotation(-transSelectedComponent.right, 
                    homePlanetCentre.position - transSelectedComponent.position);
            }
        }
    }

    void placeComponent(Vector3 pos)
    {
        Collider2D hitCollider = Physics2D.OverlapBox(pos, selectedComponent.localScale / 2, 0);
        if (hitCollider != null)
        {
            return;
        }

        GameObject newTurret = Instantiate(selectedComponent.gameObject, pos, Quaternion.identity);
        newTurret.transform.rotation *= Quaternion.FromToRotation(-newTurret.transform.right, homePlanetCentre.position - newTurret.transform.position);

        // Set orbit dist and speed
        Orbit orbit = newTurret.GetComponent<Orbit>();
        orbit.orbitCentre = homePlanetCentre.transform;
    }

    (Vector2, bool) findClosestSpot(Vector2 initPos)
    {
        Vector2 closestSpot = Vector3.zero;
        float closestDistance = -1;

        foreach (Vector2 targetPos in fixedPositions)
        {
            float distanceToSpot = (targetPos - initPos).magnitude;

            if (closestDistance == -1 || distanceToSpot < closestDistance)
            {
                closestSpot = targetPos;
                closestDistance = distanceToSpot;
            }
        }

        if (closestDistance != -1 && closestDistance < snapThresh)
        {
            return (closestSpot, true);
        }

        return (initPos, false);
    }

    public void clickFromUI(GameObject turret)
    {
        // Input turret contains both normal and transparent object
        selectedComponent = turret.transform.GetChild(0); 
        transSelectedComponent = turret.transform.GetChild(1); 

        if (turret.name == "None")
        {
            selectedComponent = null;
        }
        else
        {
            showGhost(transSelectedComponent.gameObject);
        }
    }

    void showGhost(GameObject obj)
    {   
        // Pause game 
        pauseGame(playPauseRampTime);

        transSelectedComponent = Instantiate(obj, mouseWorldPos, Quaternion.identity).transform;
        
        // Enable symbols
        symbolGroup.gameObject.SetActive(true);
    }

    void hideGhost()
    {
        // Resume game 
        resumeGame(playPauseRampTime);

        if (transSelectedComponent != null)
        {
            GameObject.Destroy(transSelectedComponent.gameObject);
            transSelectedComponent = null;
        }

        // Disable symbols
        symbolGroup.gameObject.SetActive(false);
    }

    void pauseGame(float rampTime)
    {
        Time.timeScale = 0;
        // StartCoroutine(slowPause(rampTime));
    }

    void resumeGame(float rampTime)
    {
        Time.timeScale = 1;
        // StartCoroutine(slowResume(rampTime));
    }

    // IEnumerator slowPause(float rampTime)
    // {
    //     float timeElapsed = 0;

    //     while (timeElapsed < rampTime)
    //     {   
    //         Time.timeScale = Mathf.Lerp(1, 0, timeElapsed/rampTime);

    //         timeElapsed += 10*Time.deltaTime;        
    //         yield return null;
    //     }

    //     Time.timeScale = 0;
    // }

    // IEnumerator slowResume(float rampTime)
    // {
    //     float timeElapsed = 0;

    //     while (timeElapsed < rampTime)
    //     {   
    //         Time.timeScale = Mathf.Lerp(0, 1, timeElapsed/rampTime);

    //         timeElapsed += 10*Time.deltaTime;        
    //         yield return null;
    //     }

    //     Time.timeScale = 1;
    // }
}
