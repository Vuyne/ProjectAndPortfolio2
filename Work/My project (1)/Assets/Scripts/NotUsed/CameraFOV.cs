using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    public Camera cam;            
    public float kickAmount = 5f;  
    public float kickSpeed = 10f;  

    float normalFOV;

    void Start()
    {
        if (cam == null) cam = Camera.main;
        normalFOV = cam.fieldOfView;
    }

    void Update()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normalFOV, Time.deltaTime * kickSpeed);
    }

    public void FireKick()
    {
        cam.fieldOfView += kickAmount;
    }
}
