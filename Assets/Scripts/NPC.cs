using UnityEngine;

public class NPC : MonoBehaviour
{
    public Color myTrueColor;
    private SpriteRenderer sr;
    private Color hiddenColor = new Color(0.2f, 0.2f, 0.2f); // Dark gray color
    public float revealSpeed = 3f;

    private Vector2 moveDir;
    private float nextDirChange;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (Time.time > nextDirChange)
        {
            moveDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            nextDirChange = Time.time + Random.Range(2f, 5f);
        }

        //Move slightly, but only ig they are ont screen
        transform.Translate(moveDir * 1.5f * Time.deltaTime);
    }

    private void Start()
    {
        sr.color = hiddenColor;
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
        GameTimer timer = Object.FindFirstObjectByType<GameTimer>();

        //Check if my secret color matches the target
        if (ColorUtility.ToHtmlStringRGB(myTrueColor) == ColorUtility.ToHtmlStringRGB(targetColor))
        {
            Debug.Log("Target NEutralized!");

            timer.AddTime(10f); //Reward 10 Seconds

            //Tell the CrowdManager to pick a new target Color
            Object.FindFirstObjectByType<CrowdManager>().PickNewTarget();

            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Civilian Hit ~ Penalty!");

            timer.SubtractTime(15f); //Penalty 15 Seconds
        }
    }
}
