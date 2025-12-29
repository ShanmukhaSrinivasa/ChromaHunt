using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CrowdManager crowdManager;

    private void Update()
    {
        //Left-Click to shoot
        if (Input.GetMouseButtonDown(0))
        {
            //Shoot a ray from the camera to the mouse position
            Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

            if (hit.collider != null)
            {
                NPC hitNpc = hit.collider.GetComponent<NPC>();
                if (hitNpc != null)
                {
                    //Tell the NPC it was shot and pass the current target color
                    hitNpc.OnShot(crowdManager.currentTargetColor);
                }
            }
        }
    }
}
