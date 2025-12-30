using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float normalSize = 5f; //standard view
    public float zoomSize = 2f; //Magnified view
    public float zoomSpeed = 8f; //Speed of zooming

    public float mapWidth = 18f;
    public float mapHeight = 10f;

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

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = -10f; //Keep camera back from 2D plane

        float followSpeed = isZooming ? zoomSpeed : 2f;
        Vector3 targetPos = Vector3.Lerp(transform.position, mouseWorldPos, Time.deltaTime * followSpeed);

        targetPos.x = Mathf.Clamp(targetPos.x, -mapWidth, mapWidth);
        targetPos.y = Mathf.Clamp(targetPos.y, -mapHeight, mapHeight);

        transform.position = targetPos;
    } 

    public void Shake(float intensity, float duration)
    {
        StartCoroutine(ProcessShake(intensity, duration));
    }

    private System.Collections.IEnumerator ProcessShake(float intensity, float duration)
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            transform.position = new Vector3(originalPos.x + x, originalPos.y + y, -10f);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
