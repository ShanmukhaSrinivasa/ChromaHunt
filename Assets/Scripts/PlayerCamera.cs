using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float normalSize = 5f; //standard view
    public float zoomSize = 2f; //Magnified view
    public float zoomSpeed = 8f; //Speed of zooming

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        //Right-Click (Mouse1) triggers zoom
        bool isZooming = Input.GetMouseButton(1);
        float targetSize = isZooming ? zoomSize : normalSize;

        //Smoothly interpolate the camera size
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);

        if (isZooming)
        {
            //Move camera towards mouse
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = -10f; //Keep camera back from 2D plane
            transform.position = Vector3.Lerp(transform.position, mousePos, Time.deltaTime * zoomSpeed);
        }
        else
        {
            //Return to centre when not zooming
            transform.position = Vector3.Lerp(transform.position, new Vector3(0f, 0f, -10f), Time.deltaTime * zoomSpeed);
        }
    }
}
