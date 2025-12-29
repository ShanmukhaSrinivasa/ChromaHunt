using UnityEngine;

public class NPC : MonoBehaviour
{
    public Color myTrueColor;
    private SpriteRenderer sr;
    private Color hiddenColor = new Color(0.2f, 0.2f, 0.2f); // Dark gray color
    public float revealSpeed = 3f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
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
}
