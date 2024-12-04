using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // Remove this line
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }
}
