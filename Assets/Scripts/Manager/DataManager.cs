using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    private void OnEnable()
    {
        // GameManager.OnGameWon        += HandleGameWon;
        // GameManager.OnProgressChanged += HandleShelfRestocked;
        // SceneManager.sceneLoaded      += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // GameManager.OnGameWon        -= HandleGameWon;
        // GameManager.OnProgressChanged -= HandleShelfRestocked;
        // SceneManager.sceneLoaded      -= OnSceneLoaded;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        // StartLoad();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // StartLoad();
    }



}