using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Utils utils;

    public float maxSpeed;
    public float rampUpTime;
    public float maxJumpForce, minJumpForce, chargeRate;
    public float maxCentripetalForce;
    public float cameraZoomSpeed, maxCamZoom, minCamZoom;
    public Transform hold;

    private float jumpCharge=0;
    private float lastLeftRight;
    private float speed = 0;
    private float moveTimeElapsed = 0, stopTimeElapsed = 0;
    private float distToGround;      
    private Quaternion lastFloorRot;       

    private Rigidbody2D myRigidBody;
    private SpriteRenderer spriteRenderer;
    private GameObject sprite;
    private LayerMask mask;
    private Collider2D floor;
    private GunController gunController;

    // Start is called before the first frame update
    void Start()
    {
        // Imported classes
        utils = new Utils();

        // Get rigidbody
        myRigidBody = gameObject.GetComponent<Rigidbody2D>();

        // Get Sprite
        sprite = gameObject.transform.GetChild(0).gameObject;
        spriteRenderer = sprite.GetComponent<SpriteRenderer>();

        // Get distance between player and ground
        Collider2D Collider = GetComponent<Collider2D>();
        distToGround = Collider.bounds.extents.y;

        // Get Gun
        gunController = GetComponent<GunController>();
 
        // For raycast layer mask
        mask = LayerMask.GetMask("Platform", "Planet");

    }

    // Update is called once per frame
    void Update()
    {       
        // Get current object below Player
        floor = GetFloor();

        Action();
    }

    void LateUpdate()
    {
        // Camera Actions
        CameraAction();
    }

    void Action()
    {
        Walk();

        // Jump charge system
        if (Input.GetKeyUp(KeyCode.Space))
        {            
            Jump(jumpCharge);       
            jumpCharge = 0;     
        }
        else if (Input.GetKeyDown(KeyCode.Space) || jumpCharge > 0)
        {
            if (jumpCharge == 0)
            {
                jumpCharge = minJumpForce;
            }
            else if (jumpCharge < maxJumpForce)
            {
                jumpCharge += chargeRate * Time.deltaTime;
            }
            else
            {
                jumpCharge = maxJumpForce;
            }
        }

        // Turn with mouse if mouse has moved past threshold
        Vector3 currentAim = RenderAim();
        RotateGun(currentAim);

        // Shoot gun
        if (Input.GetMouseButton(1))
        {
            gunController.Shoot(currentAim);
            FlipSprite(-currentAim.x);
        }
        else
        {
            gunController.StopShoot();
        }
    }

    void Walk()
    {
        float leftRight = Input.GetAxisRaw("Horizontal");

        if (leftRight != 0)
        {            
            // Accelerate
            if (moveTimeElapsed < rampUpTime)
            {
                speed = Mathf.Lerp(0, maxSpeed, moveTimeElapsed/rampUpTime);
                moveTimeElapsed += Time.deltaTime;
            }

            // Set pos
            transform.position = transform.position + leftRight * transform.right * speed * Time.deltaTime; 

            // Flip sprite to point to movement dir
            FlipSprite(leftRight);
        }
        else
        {
            speed = 0f;
            moveTimeElapsed = 0;
        }
    }

    void Jump(float force)
    {
        if (floor != null)
        {
            myRigidBody.AddForce(force * transform.up);
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

    Collider2D GetFloor()
    {
        RaycastHit2D hit = (Physics2D.Raycast(transform.position, -transform.up, 1f + distToGround, mask));
        
        return hit.collider;
    }

    void FlipSprite(float dir)
    {
        if (dir < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    void RotateGun(Vector3 dir)
    {
         float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
         hold.rotation = Quaternion.Euler(0,0,angle);
    }

    void ApplyCentripetal()
    {
        float force = utils.Remap(speed, 0, maxSpeed, 0, maxCentripetalForce);
        myRigidBody.AddForce(force * -transform.up * Time.deltaTime);
    }

    void CameraAction()
    {
        // Mouse scroll => Zoom in/out
        if (Input.mouseScrollDelta.y != 0)
        {
            float zoomAccel = utils.Remap(Camera.main.transform.position.z, -minCamZoom, -maxCamZoom, 0.1f, 5f);

            StartCoroutine(SmoothZoom(Input.mouseScrollDelta.y * cameraZoomSpeed * zoomAccel, 0.5f));
        }
    }

    IEnumerator SmoothZoom(float dist, float lerpDuration)
    {
        float timeElapsed = 0;

        Vector3 startPos = Camera.main.transform.position;
        
        // Ensure we don't exceed min/max zoom distance
        Vector3 finalPos = new Vector3 (startPos.x, startPos.y, startPos.z + dist);
        if (finalPos.z > minCamZoom)
        {
            finalPos = new Vector3 (startPos.x, startPos.y, minCamZoom);
        }
        else if (finalPos.z < maxCamZoom)
        {
            finalPos = new Vector3 (startPos.x, startPos.y, maxCamZoom);
        }

        while (timeElapsed < lerpDuration)
        {   
            Camera.main.transform.position = Vector3.Lerp(startPos, finalPos, timeElapsed/lerpDuration);

            timeElapsed += Time.deltaTime;        
            yield return null;
        }

        Camera.main.transform.position = finalPos;
    }
}
