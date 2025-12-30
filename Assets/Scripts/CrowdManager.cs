using UnityEngine;
using UnityEngine.UI;

public class CrowdManager : MonoBehaviour
{
    public GameObject npcPrefab;
    public int crowdSize = 50;
    public Image wantedPosterUI;
    public Color currentTargetColor;

    public int currentWave = 1;
    public int baseCrowdSize = 20;
    public int targetsPerWave = 1;
    private int targetsRemaining;

    public float worldWidth = 18f;
    public float worldHeight = 10f;


    private void Start()
    {
        SpawnWave();
    }

    public void SpawnWave()
    {
        //Clear old NPCs if any
        NPC[] oldNPCs = Object.FindObjectsByType<NPC>(FindObjectsSortMode.None);

        foreach(NPC n in oldNPCs)
        {
            Destroy(n.gameObject);
        }

        //Calculate difficulty
        int spawnCount = baseCrowdSize + (currentWave * 10); //10 more people every wave
        targetsRemaining = 1 + (currentWave / 3); //Extra target every 3 waves

        for(int i=0; i< spawnCount; i++)
        {
            Vector2 randomPos = new Vector2(Random.Range(-worldWidth, worldWidth), Random.Range(-worldHeight, worldHeight));
            GameObject newNPC = Instantiate(npcPrefab, randomPos, Quaternion.identity);
            NPC npcScript = newNPC.GetComponent<NPC>();
            npcScript.myTrueColor = new Color(Random.value, Random.value, Random.value);

            if (i == 0) // For now just pick the first one as Target
            {
                currentTargetColor = npcScript.myTrueColor;
                wantedPosterUI.color = currentTargetColor;
            }
        }
    }

    public void OnTargetHit()
    {
        targetsRemaining--;
        if (targetsRemaining <= 0)
        {
            currentWave++;
            //Check if it's Shop Time (every 3 waves)
            if (currentWave % 3 == 0)
            {
                //OpenShop()
            }
            else
            {
                SpawnWave();
            }
        }
        else
        {
            PickNewTarget();
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
            SpawnWave();
        }
    }
}
