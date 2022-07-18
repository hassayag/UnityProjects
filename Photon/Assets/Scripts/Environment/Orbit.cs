using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    Utils utils;

    float orbitDist;
    Transform centreObj;
    float orbitSpeed;

    public Transform orbitCentre;
    public float minOrbitSpeed, maxOrbitSpeed;

    // Start is called before the first frame update
    void Start()
    {
        utils = new Utils();

        // Create object to act as centre
        centreObj = Instantiate(new GameObject(), orbitCentre.position, Quaternion.identity).transform;
        transform.SetParent(centreObj);

        // Get radius of orbit
        Vector3 radius = transform.position - centreObj.transform.position;
        orbitDist = radius.magnitude;
        orbitSpeed = utils.Remap(orbitDist, 0f, 1000f, maxOrbitSpeed, minOrbitSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        centreObj.Rotate(new Vector3(0, 0, orbitSpeed * Time.deltaTime / orbitDist));
    }
}
