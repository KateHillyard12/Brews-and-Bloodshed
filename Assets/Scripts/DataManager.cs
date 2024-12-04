using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameData
{
    public List<NPCData> npcStates; // List of NPC data
    public bool isResolutionActive; // Resolution state
    public int totalInteractions; // Total number of interactions with NPCs
    public int totalMugDrops; // Total number of mugs dropped
    public float completionTime; // Total time taken to complete (in seconds)
    public string murdererNPC; // Name of the NPC chosen as the murderer
}

[System.Serializable]
public class NPCData
{
    public string npcName;
    public bool hasReceivedMug;
    public int interactionCount; // Number of times interacted with this NPC
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public List<NPCInteractable> npcInteractables;
    public ResolutionManager resolutionManager;

    private string saveFilePath;

    // Data points to track
    private int totalInteractions;
    private int totalMugDrops;
    private float startTime;

    public string murdererNPC; // Set this value based on game logic

    private void Awake()
    {
        Debug.Log($"Save path: {Application.persistentDataPath}");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        saveFilePath = Path.Combine(Application.persistentDataPath, "GameData.json");
        startTime = Time.time; // Record start time for completion tracking
    }

    public void RecordInteraction()
    {
        totalInteractions++;
    }

    public void RecordMugDrop()
    {
        totalMugDrops++;
    }

    public void SaveGame()
    {
        
        GameData data = new GameData
        {
            npcStates = new List<NPCData>(),
            isResolutionActive = resolutionManager.isResolutionActive,
            totalInteractions = totalInteractions,
            totalMugDrops = totalMugDrops,
            completionTime = Time.time - startTime,
            murdererNPC = murdererNPC
        };

        

        foreach (var npc in npcInteractables)
        {
            NPCData npcData = new NPCData
            {
                npcName = npc.name,
                hasReceivedMug = npc.HasReceivedMug,
                interactionCount = npc.InteractionCount // Assuming this is tracked in your NPCInteractable class
            };
            data.npcStates.Add(npcData);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log($"Game data saved to {saveFilePath}");
    }

    public void LogSavedData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            Debug.Log($"Saved Data: {json}");
        }
        else
        {
            Debug.Log("No save file found.");
        }
    }
}
