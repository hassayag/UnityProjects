using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (GunController))]

public class Photon : LivingEntity
{
    private Utils utils;

    public Vector3 currentChunk;
    public int chunkSize = 1000;
    public float speed = 1;
    public float trailWidth = 5.0f;
    public float angSpeed = 100.0f;
    public float speedLimit = 100.0f;
    public float mouseThresh = 10.0f;
    public float turnThresh = 1.0f;
    public float maxCamDist, minCamDist;

    private Rigidbody2D myRigidBody;
    private GameObject currentAim, camera;
    private TrailRenderer trail;
    private GunController gunController;
    private Transform gun;

    //Coroutine Vars
    private Vector3 v_start, v_end;
    private float theta;


    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = gameObject.GetComponent<Rigidbody2D>();
        trail = gameObject.GetComponent<TrailRenderer>();
        utils = new Utils();

        // Get Gun
        gunController = GetComponent<GunController>();
        gun = transform.Find("Gun");
        // Get Camera
        camera = gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        currentChunk = new Vector3 (Mathf.Floor(transform.position.x/chunkSize), Mathf.Floor(transform.position.y/chunkSize));
        StartCoroutine(SetCameraDistance());
        Action();
    }

    void Action()
    {
        // Change speed
        float verticalMove = Input.GetAxisRaw("Vertical");
        if (verticalMove != 0)
        {
            Accelerate(verticalMove/Mathf.Abs(verticalMove));
        }

        // Turn with mouse if mouse has moved past threshold
        Vector3 currentAim = RenderAim();
        Turn(currentAim);

        // Shoot gun
        if (Input.GetMouseButton(0))
        {
            gunController.Shoot(myRigidBody.velocity);
        }

        // Set trail proportional to the speed
        trail.widthMultiplier = utils.Remap(myRigidBody.velocity.magnitude, 0, 1000, 0, trailWidth);
    }

    IEnumerator SetCameraDistance()
    {
        // Scale camera z to speed
        float currentSpeed = myRigidBody.velocity.magnitude;
        float camDistance = utils.Remap(currentSpeed, 0, speedLimit, minCamDist, maxCamDist);

        float lerpDuration = 0.2f;
        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)
        {   
            float dist_t = Mathf.Lerp(camera.transform.position.z, camDistance, timeElapsed/lerpDuration);

            camera.transform.Translate( new Vector3 (0, 0, dist_t - camera.transform.position.z), Space.World);

            timeElapsed += Time.deltaTime;        
            yield return null;
        }
    }

    Vector3 RenderAim()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 p1 = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -1*Camera.main.transform.position.z));
        Vector3 p0 = transform.position;

        // Force into same plane
        p1.z = p0.z;
        
        Vector3 line = p1 - p0; 

        return line;
    }

    void Turn(Vector3 direction)
    {
        // Smooth out the change in direction
        v_start = myRigidBody.velocity;

        // Turning with no speed is pointless
        if (v_start.magnitude == 0)
        {
            return;
        }

        v_end = direction.normalized * v_start.magnitude;
        
        // angle between start and end vector
        theta = angleBetweenVectors(v_start, v_end) * Mathf.Rad2Deg;

        // validate angle
        if (utils.isNan(theta) || Mathf.Round(theta) == 0)
        {
            return;
        }

        // don't bother turning very small amounts
        if (Mathf.Abs(theta) < turnThresh)
        {
            return;
        }

        StartCoroutine(SmoothTurn());
    }

    IEnumerator SmoothTurn()
    {   
        int numIncrements = (int) Mathf.Abs(Mathf.Ceil(theta / (angSpeed/100)));
        float delay = Time.deltaTime;
        float lerpDuration = delay * numIncrements;
        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)
        {   
            float deltaTheta = Mathf.Lerp(0, theta, timeElapsed/lerpDuration);
            if (utils.isNan(deltaTheta))
            {
                break;
            }
            // Rotate velocity vector about z-axis
            Vector3 v_next = Quaternion.AngleAxis(deltaTheta, -1 * Vector3.forward) * v_start;
            myRigidBody.velocity = v_start.magnitude * v_next.normalized;

            timeElapsed += delay;        
            yield return null;
        }
    }

    void DeleteAim()
    {
        if (currentAim != null)
        {
            Object.Destroy(currentAim);
        }
    }

    void Accelerate(float direction)
    {
        Vector3 vel = myRigidBody.velocity;
        if (vel.magnitude > speedLimit && direction == 1)
        {
            return;
        }
        if (vel.magnitude == 0)
        {   
            // Can't go negative velocity
            if (direction == -1)
            {
                return;
            }
            // If no current velocity, just go up
            vel = Vector3.up;
        }

        myRigidBody.AddForce(speed * direction * vel);
    }

    public float angleBetweenVectors(Vector3 v1, Vector3 v2)
    {
        float dotproduct = Vector3.Dot(v1,v2);

        // dotproduct doesn't tell us to rotate clockwise or anticlockwise
        Vector3 crossproduct = Vector3.Cross(v1,v2);
        float polarity = crossproduct.z/Mathf.Abs(crossproduct.z);

        return Mathf.Acos(dotproduct/(v1.magnitude * v2.magnitude)) * polarity * -1;
    }

}
