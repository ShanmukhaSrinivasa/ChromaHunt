using System.Runtime.InteropServices.WindowsRuntime;
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

    [Header("Ping Settings")]
    public float pingCooldown = 10f; // Time between pings
    private float coolDownProgress;
    private float lastPingTime;
    public Slider pingSlider;

    private Vector3 originalPosterPos;


    private void Start()
    {
        originalPosterPos = wantedPosterUI.transform.localPosition;
        SpawnWave();
    }

    private void Update()
    {
        // Proximity Pulse
        NPC targetNPC = GetTargetNPCObject();
        if (targetNPC != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            // 
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            bool isHoveringTarget = (hit.collider != null && hit.collider.gameObject == targetNPC.gameObject);

            float dist = Vector2.Distance(mousePos, targetNPC.transform.position);
            float proximity = Mathf.Clamp01(1f - (dist / 8f)); // Max effect within 8 units
            float finalIntensity = isHoveringTarget ? 1.0f : proximity * 0.3f;

            // 1. Scale Pulse (Bigger)
            float pulse = 1f + (Mathf.Sin(Time.time * 20f) * 0.2f * finalIntensity);
            wantedPosterUI.transform.localScale = Vector3.one * pulse;

            // 2. Shake the UI if very close
            if (finalIntensity > 0.5f)
            {
                wantedPosterUI.transform.localPosition = originalPosterPos + (Vector3)Random.insideUnitCircle * 2f;
            }
            else
            {
                wantedPosterUI.transform.localPosition = Vector3.Lerp(wantedPosterUI.transform.localPosition, originalPosterPos, Time.deltaTime * 5f);
            }
        }

        // 2. Cooldown Logic
        // Calculate how much time has passed since last ping
        float timeSinceLastPing = Time.time - lastPingTime;
        coolDownProgress = Mathf.Clamp01(timeSinceLastPing / pingCooldown);

        // Update the UI Bar
        if (pingSlider != null)
        {
            pingSlider.value = coolDownProgress; 
        }

        // 3. Ping Trigger
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastPingTime + pingCooldown)
        {
            TriggerPing();
            lastPingTime = Time.time;
        }
    }

    private void TriggerPing()
    {
        NPC target = GetTargetNPCObject();

        if (target != null)
        {
            // Calculate distance from camera to target
            Vector2 direction = (target.transform.position - Camera.main.transform.position);

            // Tell the camera to show ripple in this direction
            Object.FindAnyObjectByType<PlayerCamera>().ShowPingRipple(direction);
        }
    }

    NPC GetTargetNPCObject()
    {
        NPC[] all = Object.FindObjectsByType<NPC>(FindObjectsSortMode.None);
        string targetHex = UnityEngine.ColorUtility.ToHtmlStringRGB(currentTargetColor);

        foreach (NPC n in all)
        {
            if (UnityEngine.ColorUtility.ToHtmlStringRGB(n.myTrueColor) == targetHex)
            {
                return n;
            }
        }
        return null;
    }
    public void SpawnWave()
    {
        //Clear old NPCs if any
        NPC[] oldNPCs = Object.FindObjectsByType<NPC>(FindObjectsSortMode.None);

        foreach(NPC n in oldNPCs)
        {
            Destroy(n.gameObject);
        }

        System.Collections.Generic.List<NPC> spawnedScripts = new System.Collections.Generic.List<NPC>();

        //Calculate difficulty
        int spawnCount = baseCrowdSize + (currentWave * 10); //10 more people every wave
        targetsRemaining = 1 + (currentWave / 3); //Extra target every 3 waves

        for(int i=0; i< spawnCount; i++)
        {
            Vector2 randomPos = new Vector2(Random.Range(-worldWidth, worldWidth), Random.Range(-worldHeight, worldHeight));
            GameObject newNPC = Instantiate(npcPrefab, randomPos, Quaternion.identity);
            NPC npcScript = newNPC.GetComponent<NPC>();

            npcScript.myTrueColor = new Color(Random.value, Random.value, Random.value);
            spawnedScripts.Add(npcScript);

            if (spawnedScripts.Count > 0) // For now just pick the first one as Target
            {
                int randomIndex = Random.Range(0, spawnedScripts.Count);
                SetCurrentTarget(spawnedScripts[randomIndex].myTrueColor);
            }
        }
    }

    private void SetCurrentTarget(Color newColor)
    {
        currentTargetColor = newColor;

        if (wantedPosterUI != null)
        {
            wantedPosterUI.color = currentTargetColor;
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

            SetCurrentTarget(remainingNPCs[randomIndex].myTrueColor);
        }
        else
        {
            //All NPCs are dead - Spawn a new Wave!
            SpawnWave();
        }
    }
}
