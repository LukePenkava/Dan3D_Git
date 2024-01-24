using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{

    public Transform player; // Assign your player character    
    public Transform cam;
    public Transform yawParent;
    public Transform pitchParent;

    public Vector3 targetOffset; // Distance from the player
    public Vector3 camOffset; // Distance from the player   

    public float targetSmoothing = 10f; // Speed at which the camera follows
    public float camSmoothing = 10f; // Speed at which the camera follows
    public float rotationSmoothing = 10f; // Speed at which the camera follows

    public float rotationSpeedYaw = 5.0f;
    public float rotationSpeedPitch = -5.0f;
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {

    }

    private void LateUpdate()
    {
        Vector3 targetPosition = player.position + targetOffset;
        Vector3 smoothedTargetPosition = Vector3.Lerp(transform.position, targetPosition, targetSmoothing * Time.deltaTime);
        transform.position = smoothedTargetPosition;

        cam.localPosition = camOffset;

        // yaw = rotationSpeedYaw * Input.GetAxis("Mouse X");
        // pitch = rotationSpeedPitch * Input.GetAxis("Mouse Y");

        // yawParent.Rotate(Vector3.up, yaw * Time.deltaTime);
        // pitchParent.Rotate(Vector3.right, pitch * Time.deltaTime);

        // cam.LookAt(smoothedTargetPosition);



        yaw = rotationSpeedYaw * Input.GetAxisRaw("Mouse X");
        pitch = rotationSpeedPitch * Input.GetAxisRaw("Mouse Y");

        yawParent.Rotate(Vector3.up, yaw);
        pitchParent.Rotate(Vector3.right, pitch);

        cam.LookAt(smoothedTargetPosition);

        // yaw += ( Input.GetAxis("Mouse X") * rotationSpeedYaw);
        // yawParent.rotation = Quaternion.Euler(0, yaw, 0);        
    }
}

//   Vector3 targetPosition = player.position + targetOffset;
//         Vector3 smoothedTargetPosition = Vector3.Lerp(transform.position, targetPosition, targetSmoothing * Time.deltaTime);
//         transform.position = smoothedTargetPosition;

//         cam.localPosition = camOffset;

//         float newYaw = rotationSpeedYaw * Input.GetAxisRaw("Mouse X");
//         float smoothedYaw = Mathf.Lerp(yaw, newYaw, yawSmoothing * Time.deltaTime);
//         float newPitch = rotationSpeedPitch * Input.GetAxisRaw("Mouse Y");
//         float smoothedPitch = Mathf.Lerp(pitch, newPitch, pitchSmoothing * Time.deltaTime);

//         yawParent.Rotate(Vector3.up, smoothedYaw);
//         pitchParent.Rotate(Vector3.right, smoothedPitch);

//         cam.LookAt(smoothedTargetPosition);
