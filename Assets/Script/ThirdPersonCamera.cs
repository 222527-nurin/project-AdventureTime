using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target; //Player
    public float distance = 5f; // distance from player
    public float mouseSensitivity = 100f;

    public float minY = -20f; //limit looking down
    public float maxY = 60f; //limit looking up

    private float xRotation = 0f;
    private float yRotation = 0f;

    [Header("Collision")]
    public LayerMask collisionLayers;
    public float cameraRadius = 0.3f;
    public float collisionOffset = 0.2f;

    RaycastHit hit;

    void Start()
    {
        //lock cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //mouse  input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        //clamp vertical rotation
        xRotation = Mathf.Clamp(xRotation, minY, maxY);

        //rotation
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);

        Vector3 origin = target.position + Vector3.up * 1.6f;
        Vector3 desiredPosition = origin - (rotation * Vector3.forward * distance);
        Vector3 direction = (desiredPosition - origin).normalized;

        if (Physics.SphereCast(origin,
                               cameraRadius,
                               direction,
                               out hit,
                               distance,
                               collisionLayers))
        {
            transform.position = hit.point - direction * collisionOffset;
        }
        else
        {
            transform.position = desiredPosition;
        }

        transform.LookAt(origin);
    }
}
