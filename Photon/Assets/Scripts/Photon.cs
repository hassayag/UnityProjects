using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (GunController))]

public class Photon : LivingEntity
{
    private Utils utils;

    public Vector3 currentChunk;
    public int chunkSize = 1000;
    public float flySpeed, walkSpeed;
    public float trailWidth = 5.0f;
    public float angSpeed = 100.0f;
    public float speedLimit = 100.0f;
    public float mouseThresh = 10.0f;
    public float turnThresh = 1.0f;
    public float maxCamDist, minCamDist;

    private Rigidbody2D myRigidBody;
    private GameObject currentAim, camera, sprite;
    private TrailRenderer trail;
    private GunController gunController;
    private Transform gun;
    private SpriteRenderer spriteRenderer;


    bool isFlying = true;

    //Coroutine Vars
    private Vector3 v_start, v_end;
    private float theta, theta0;

    private Quaternion eulerAng, eulerAng0;

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

        // Get Sprite
        sprite = gameObject.transform.GetChild(1).gameObject;
        spriteRenderer = sprite.GetComponent<SpriteRenderer>();

        // Init Velocity
        myRigidBody.velocity = new Vector3 (0.3f,0,0);
        myRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        // Store current chunk for use in BuildStars
        currentChunk = new Vector3 (Mathf.Floor(transform.position.x/chunkSize), Mathf.Floor(transform.position.y/chunkSize));

        // Check flying inputs
        if (isFlying)
        {
            FlyAction();
            
            // Flip sprite if negative angle
            FlipSprite();
        }
        else
        {
            ButtDown();
            GroundAction();
        }
    }

    void LateUpdate()
    {
        // Set camera distance based on velocity
        StartCoroutine(SetCameraDistance());
    }

    void FlyAction()
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

        // Switch to walk
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isFlying = false;
            // myRigidBody.constraints = RigidbodyConstraints2D.None;
        }

        // Set trail proportional to the speed
        // trail.widthMultiplier = utils.Remap(myRigidBody.velocity.magnitude, 0, 1000, 0, trailWidth);
    }

    void GroundAction()
    {
        float leftRight = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(leftRight) != 0)
        {
            Walk(leftRight/Mathf.Abs(leftRight));
        }

        // Switch to fly
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isFlying = true;
            // myRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    // dir = +/- 1
    void Walk(float dir)
    {
        transform.Translate(new Vector3(dir*walkSpeed*Time.deltaTime, 0, 0));
    }

    void ButtDown()
    {
        // Search for nearest planet
        GameObject planet = FindClosestPlanet();    
        Vector3 toCentre = planet.transform.position - transform.position;    
        
        // transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(toCentre.y, toCentre.x) * Mathf.Rad2Deg + 90);
        Debug.Log(Mathf.Atan2(toCentre.y, toCentre.x) * Mathf.Rad2Deg);
        // StartCoroutine(SmoothRotate());
    }

    GameObject FindClosestPlanet()
    {
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");

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
    
    IEnumerator SmoothRotate()
    {   
        // Set min increments to 10
        int numIncrements = (int) Mathf.Max(Mathf.Abs(Mathf.Ceil(Mathf.Pow(theta,2f) / (angSpeed/100))), 10);
        float delay = Time.deltaTime;
        float lerpDuration = delay * numIncrements;
        float timeElapsed = 0;
        float deltaTheta = 0;

        while (timeElapsed < lerpDuration)
        {   
            transform.rotation = Quaternion.Lerp(eulerAng0, eulerAng0, timeElapsed/lerpDuration);

            timeElapsed += delay;        
            yield return null;
        }
    }

    void FlipSprite()
    {
        float dogAngle = sprite.transform.localEulerAngles.z - 90;

        if (dogAngle < 180 && dogAngle > 0)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }
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
        theta0 = angleBetweenVectors(Vector3.up, v_start) * Mathf.Rad2Deg;

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
        StopCoroutine(SmoothTurn());
    }

    IEnumerator SmoothTurn()
    {   
        // Set min increments to 100
        int numIncrements = (int) Mathf.Max(Mathf.Abs(Mathf.Ceil(Mathf.Pow(theta,2f) / (angSpeed/100))), 100);
        float delay = Time.deltaTime;
        float lerpDuration = delay * numIncrements;
        float timeElapsed = 0;
        float deltaTheta = 0;

        while (timeElapsed < lerpDuration && Mathf.Abs(theta - deltaTheta) > 0.5f)
        {   
            deltaTheta = Mathf.Lerp(0, theta, timeElapsed/lerpDuration);
            if (utils.isNan(deltaTheta))
            {
                break;
            }
            // Rotate velocity vector about z-axis
            Vector3 v_next = Quaternion.AngleAxis(deltaTheta, -1 * Vector3.forward) * v_start;
            myRigidBody.velocity = v_start.magnitude * v_next.normalized;
            
            // Angle sprites head towards mouse
            sprite.transform.rotation = Quaternion.Euler(0, 0, -(theta0+deltaTheta-90));

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

    // direction = +/- 1 to indicate thrust direction
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

        myRigidBody.AddForce(flySpeed * direction * vel.normalized);
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
