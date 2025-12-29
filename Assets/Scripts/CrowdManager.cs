using UnityEngine;
using UnityEngine.UI;

public class CrowdManager : MonoBehaviour
{
    public GameObject npcPrefab;
    public int crowdSize = 50;
    public Image wantedPosterUI;
    public Color currentTargetColor;

    private void Start()
    {
        SpawnCrowd();
    }

    public void SpawnCrowd()
    {
        Color targetColor = Color.clear;

        for (int i = 0; i < crowdSize; i++)
        {
            //Spawn at random position
            Vector2 randomPos = new Vector2(Random.Range(-8f, 8f), Random.Range(-4f, 4f));
            GameObject newNPC = Instantiate(npcPrefab, randomPos, Quaternion.identity);

            NPC npcScript = newNPC.GetComponent<NPC>();

            //Assign a random bright color
            npcScript.myTrueColor = new Color(Random.value, Random.value, Random.value);

            //Pick the first NPC's color as the target color
            if (i == 0)
            {
                currentTargetColor = npcScript.myTrueColor;
            }
        }

        //Show the target color on the UI
        if(wantedPosterUI != null)
        {
            wantedPosterUI.color = currentTargetColor;
        }
    }

    public void PickNewTarget()
    {
        //Find all remaining NPCs in the scene
        NPC[] remainingNPCs = Object.FindObjectsByType<NPC>(FindObjectsSortMode.None);

        if (remainingNPCs.Length > 0)
        {
            //Pick a random one from the Survivors
            int randomIndex = Random.Range(0, remainingNPCs.Length);
            currentTargetColor = remainingNPCs[randomIndex].myTrueColor;

            //Update the UI
            if(wantedPosterUI != null)
            {
                wantedPosterUI.color = currentTargetColor;
            }
        }
        else
        {
            //All NPCs are dead - Spawn a new Wave!
            SpawnCrowd();
        }
    }
}
