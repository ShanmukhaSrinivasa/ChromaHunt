using Unity.VisualScripting;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Color myTrueColor;
    private SpriteRenderer sr;
    private Color hiddenColor = new Color(0.2f, 0.2f, 0.2f); // Dark gray color
    public float revealSpeed = 3f;

    private Vector2 moveDir;
    private float nextDirChange;

    private Vector2 targetWanderPoint;
    private bool isWaiting = false;
    public float moveSpeed = 2.0f;

    private bool isDead = false;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (!isWaiting)
        {
            MoveTowardTarget();
        }
    }
  
    private void Start()
    {
        sr.color = hiddenColor;

        //Pick first target immeadiately so they start moving
        CrowdManager manager = Object.FindAnyObjectByType<CrowdManager>();
        targetWanderPoint = new Vector2(
            Random.Range(-manager.worldWidth, manager.worldWidth),
            Random.Range(-manager.worldHeight, manager.worldHeight)
        );
    }

    private void Update()
    {
        //Only reveal if the player is holding Right-Click (Mouse1)
        if (Input.GetMouseButton(1))
        {
            float dist = Vector2.Distance(transform.position, Camera.main.transform.position);

            //If NPC is near the center of the zoom lens (distance < 1.5)
            if (dist< 1.5f)
            {
                sr.color = Color.Lerp(sr.color, myTrueColor, Time.deltaTime * revealSpeed);
            }
            else
            {
                sr.color = Color.Lerp(sr.color, hiddenColor, Time.deltaTime * revealSpeed);
            }
        }
        else
        {
            //Fade back to gray when not zooming
            sr.color = Color.Lerp(sr.color, hiddenColor, Time.deltaTime * revealSpeed);
        }

    }

    public void RevealColor()
    {
        sr.color = myTrueColor;
    }

    public void OnShot(Color targetColor)
    {
        if(isDead) return; //Prevent multiple hits

        GameTimer timer = Object.FindFirstObjectByType<GameTimer>();
        CrowdManager manager = Object.FindFirstObjectByType<CrowdManager>();
        PlayerCamera cam = Object.FindFirstObjectByType<PlayerCamera>();

        //Check if my secret color matches the target
        if (UnityEngine.ColorUtility.ToHtmlStringRGB(myTrueColor) == UnityEngine.ColorUtility.ToHtmlStringRGB(targetColor))
        {
            isDead = true;

            Debug.Log("Target Neutralized!");

            cam.Shake(0.1f, 0.2f); //Small satisfying shake for a hit

            StartCoroutine(DeResEffect());

            timer.AddTime(20f); //Reward 20 Seconds

            manager.OnTargetHit();

            timer.AddScore(1);
        }
        else
        {
            Debug.Log("Civilian Hit ~ Penalty!");

            cam.Shake(0.3f, 0.4f); //Larger shake for a miss

            cam.FlashRed();

            timer.SubtractTime(15f); //Penalty 15 Seconds
        }
    }

    System.Collections.IEnumerator DeResEffect()
    {
        float duration = 0.5f;
        float currentTime = 0;
        Vector3 startScale = transform.localScale;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            //Shrink the NPC while they fade out
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, currentTime / duration);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void MoveTowardTarget()
    {
        //Move the NPC toward the current target point
        transform.position = Vector2.MoveTowards(transform.position, targetWanderPoint, moveSpeed * Time.deltaTime);

        //If we reached the point, wait a bit then pick a new one
        if (Vector2.Distance(transform.position, targetWanderPoint) < 0.2f)
        {
            StartCoroutine(WaitAndPickNewPoint());
        }
    }

    System.Collections.IEnumerator WaitAndPickNewPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(Random.Range(1f, 3f));

        //Get limits from your CrowdManager variables
        CrowdManager manager = Object.FindAnyObjectByType<CrowdManager>();

        //Pick a new point within the map boundaries
        targetWanderPoint = new Vector2(
            Random.Range(-manager.worldWidth + 1f, manager.worldWidth - 1f),
            Random.Range(-manager.worldHeight + 1f, manager.worldHeight - 1f)
        );

        isWaiting = false;
    }
}
