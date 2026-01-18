using UnityEngine;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour
{
    public float normalSize = 5f; //standard view
    public float zoomSize = 2f; //Magnified view
    public float zoomSpeed = 8f; //Speed of zooming

    public float mapWidth = 18f;
    public float mapHeight = 10f;

    private Camera cam;

    public GameObject pingIndicator;
    public Image screenFlashImage; 

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

        if (isZooming)
        {
            Time.timeScale = 0.7f; // Slow motion
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // Smooth Physics
        }
        else
        {
            Time.timeScale = 1f; // Normal Speed
            Time.fixedDeltaTime = 0.02f;
        }
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

    public void ShowPingRipple(Vector2 direction)
    {
        StartCoroutine(ProcessPing(direction));
    }

    private System.Collections.IEnumerator ProcessPing(Vector2 direction)
    {
        pingIndicator.SetActive(true);
        CanvasGroup cg = pingIndicator.GetComponent<CanvasGroup>();

        cg.alpha = 1;


        // Roatate the ping indicator to point toawrds the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        pingIndicator.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        // Set a very small starting scale for the ripple effect
        pingIndicator.transform.localScale = new Vector3(0.3f, 0.3f, 1f);

        // Fade out after 1.5 seconds
        float duration = 0.8f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            // 1. Make the ripple expand outward quickly
            float curve = 1f - Mathf.Pow(1f - t, 3); // Ease out cubic
            pingIndicator.transform.localScale = Vector3.Lerp(new Vector3(0.5f, 0.5f, 1f), new Vector3(3f, 3f, 1f), curve);

            cg.alpha = 1f - curve;
            yield return null;
        }

        pingIndicator.SetActive(false);
    }

    public void FlashRed()
    {
        StartCoroutine(RedFlashRoutine());
    }

    private System.Collections.IEnumerator RedFlashRoutine()
    {
        float elapsed = 0f;
        float duration = 0.25f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0.4f, 0f, elapsed / duration);
            screenFlashImage.color = new Color(1, 0, 0, alpha);
            yield return null;
        }
    }
}
