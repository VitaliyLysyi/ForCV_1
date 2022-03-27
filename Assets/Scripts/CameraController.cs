using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{ 
    public bool EnaleTargetMovement;
    public Transform Target;
    public float smoothing;
    public float ZoomMin = 10f;
    public float ZoomMax = 2f;

    private float MouseWheel;
    private Camera Cam;

    void Start()
    {
        Cam = GetComponent<Camera>();
        if (Target == null)
            Debug.Log("Чорте! камера нідочго не привязана не забувай!");
    }

    void Update()
    {
        CameraFollow();
        CameraZoom();
    }

    void CameraZoom()
    {
        MouseWheel = Input.GetAxis("MouseScrollWheel");
        if (MouseWheel != 0)                    
        {
            Cam.orthographicSize = Cam.orthographicSize + MouseWheel;
            if (Cam.orthographicSize < ZoomMax)
            {
                Cam.orthographicSize = ZoomMax;
            }
            if (Cam.orthographicSize > ZoomMin)
            {
                Cam.orthographicSize = ZoomMin;
            }
        }
    }

    void CameraFollow()
    {
        if ((transform.position != Target.position) & (EnaleTargetMovement == true))
        {
            Vector3 TargetPosition = new Vector3(Target.position.x, Target.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, TargetPosition, smoothing);
        }
    }
}