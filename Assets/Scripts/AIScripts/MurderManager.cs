using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurderManager : MonoBehaviour
{
    public static MurderManager Instance;
    public string murdererName; // Could be "Dave", "Stacy", or "Mark"

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional if you change scenes
            PickRandomMurderer();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void PickRandomMurderer()
    {
        string[] candidates = { "Dave", "Stacy", "Mark" }; // Matches your tags
        murdererName = candidates[Random.Range(0, candidates.Length)];
    }

}
