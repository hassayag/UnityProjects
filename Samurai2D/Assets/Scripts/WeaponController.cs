using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform playerCom; // player's centre of mass
    public GameObject equippedItem;
    public float maxRotateTime, minAngle;
    public float maxHandRadius;

    SpriteRenderer itemSprite;

    // Start is called before the first frame update
    void Start()
    {
        itemSprite = equippedItem.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Map mouse position to world and put it in the z=0 plane
        Vector3 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mousePos.z = 0;

        Vector3 _transformToMouse = _mousePos - playerCom.position;

        Vector3 _handPos = _mousePos;
        if ((_handPos - playerCom.position).magnitude > maxHandRadius)
        {
            _handPos = playerCom.position + maxHandRadius * _transformToMouse.normalized;
        }
        
        Vector3 _lookPos = _mousePos + 2f * _transformToMouse.normalized;
            
        Vector3 _lookRotation = Quaternion.LookRotation(_lookPos - transform.position).eulerAngles;

        Debug.DrawLine(transform.position, _mousePos, Color.red);

        // Use x rotation for z rotation instead. Look rotation doesn't seem to cover 360 degrees, so let's do it ourself
        if (_transformToMouse.x < 0)
        {
            _lookRotation.z = _lookRotation.x + 90f;
            itemSprite.flipX = true;
        }
        else
        {
            _lookRotation.z = -(_lookRotation.x + 90f);
            itemSprite.flipX = false;
        }

        // Move hand to mousePos
        transform.position = _handPos;

        // Rotate sword to point away from player CoM
        SetZRotation(_lookRotation.z);

        //if (Mathf.Abs(_lookRotation.z - transform.eulerAngles.z) > minAngle)
        //{ 
        //    //StartCoroutine(SmoothRotate(_lookRotation.z));
        //}
    }

    IEnumerator SmoothRotate(float angle)
    {
        float timeElapsed = 0;
        float lerpDuration = maxRotateTime * Mathf.Abs(angle - transform.eulerAngles.z) / 360;
        Debug.Log(angle);
        
        while (timeElapsed < lerpDuration)
        {
            float angle_t = Mathf.Lerp(transform.eulerAngles.z, angle, timeElapsed/lerpDuration);
            SetZRotation(angle_t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        SetZRotation(angle);
    }

    void SetZRotation(float angle)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle);
    }
}
