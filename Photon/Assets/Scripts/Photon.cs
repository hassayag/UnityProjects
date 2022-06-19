using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photon : MonoBehaviour
{
    private Utils utils;

    public GameObject AimLine;
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
    private Vector3 lastAim;
    private TrailRenderer trail;

    // bool isAiming, fire;

    //Coroutine Vars
    private Vector3 v_start, v_end;
    private float theta;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = gameObject.GetComponent<Rigidbody2D>();
        trail = gameObject.GetComponent<TrailRenderer>();
        utils = new Utils();
        myRigidBody.velocity = Vector3.up * 100;

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
        // Flick mechanic
        // ToggleAim();

        // if (isAiming)
        // {
        //     aimDir = RenderAim();
        // }
        // else if (fire) 
        // {
        //     ShootBall(aimDir);
        // }

        // Get keyboard movement
        // Vector3 arrowDir = new Vector3 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        // Change speed
        float verticalMove = Input.GetAxisRaw("Vertical");
        if (verticalMove != 0)
        {
            Accelerate(verticalMove/Mathf.Abs(verticalMove));
        }

        // Turn with mouse if mouse has moved past threshold
        Vector3 currentAim = RenderAim();
        // Vector3 aimDiff = currentAim - lastAim;
        // if (aimDiff.magnitude > mouseThresh )
        // {
        //     ShootBall(currentAim);
        // }
        Turn(currentAim);

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
    // void ToggleAim()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         isAiming = true;
    //     }
    //     else if (Input.GetMouseButtonUp(0) && isAiming)
    //     {
    //         isAiming = false;
    //         fire = true;
    //         DeleteAim();
    //     }
    //     else if (Input.GetMouseButtonUp(0))
    //     {
    //         isAiming = false;
    //         DeleteAim();
    //     }
    // }

    Vector3 RenderAim()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 p1 = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -1*Camera.main.transform.position.z));
        Vector3 p0 = transform.position;

        // Force into same plane
        p1.z = p0.z;
        
        Vector3 line = p1 - p0;

        // This renders the red aim line
        // ------------------------------
        // float length = line.magnitude;
        // float numOfPoints = AimLine.GetComponent<LineRenderer>().positionCount;
        // float ratio = 1/(numOfPoints-1);

        // // Instantiate aim line only if one does not exist
        // if (currentAim == null)
        // {
        //     currentAim = Instantiate(AimLine, transform.position, Quaternion.identity);
        // }

        // // Set each point in the line - linear line from p0 to p1
        // for (int i = 0; i < numOfPoints; i++)
        // {
        //     // Calculate each points along line. -1 will reflect the line
        //     Vector3 p_i = p0 + -1*(line * ratio * i);
        //     p_i.z += 1;
        //     currentAim.GetComponent<LineRenderer>().SetPosition(i, p_i);
        // }    

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
        // fire = false;
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
            Debug.Log("speed limit reached");
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
