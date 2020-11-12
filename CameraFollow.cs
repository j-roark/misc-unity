using UnityEngine;
using System;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public float camSwingSpeed = 0.125f;
    public float camSwingMultiplier;
    public Vector3 offset;
    Vector3 camSwingDesired;

    void FixedUpdate ()
    {
        float camSwingDir = Input.GetAxisRaw("Horizontal");
        if(camSwingDir == 1) {
            Vector3 camSwingDirection = new Vector3(camSwingMultiplier, 0.0f, 0.0f);
            camSwingDesired = offset + camSwingDirection;
        }
        else if(camSwingDir == -1) {
            Vector3 camSwingDirection = new Vector3(-camSwingMultiplier, 0.0f, 0.0f);
            camSwingDesired = offset + camSwingDirection; 
        }
        else {
            camSwingDesired = new Vector3(0.0f, 0.0f, 0.0f); 
        }
        Vector3 newOffset = Vector3.Lerp(offset, camSwingDesired, camSwingSpeed);
        Vector3 desiredPosition = target.position + newOffset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothPosition;
    }
}
