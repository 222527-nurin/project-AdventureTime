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

        //position (behind player)
        Vector3 position = target.position - (rotation * Vector3.forward * distance);

        transform.position = position;
        transform.LookAt(target);
    }
}
