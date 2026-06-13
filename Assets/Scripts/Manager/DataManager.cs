using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    private void OnEnable()
    {
        GameManager.OnGameWon += HandleGameWon;
        GameManager.OnProgressChanged += HandleShelfRestocked;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        GameManager.OnGameWon -= HandleGameWon;
        GameManager.OnProgressChanged -= HandleShelfRestocked;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        StartLoad();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartLoad();
    }

    public void SaveGame(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        File.WriteAllText(path, json);
    }

    public GameSaveData LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (!File.Exists(path)) return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public void StartLoad()
    {
        GameSaveData data = LoadGame();
        if (data == null || data.restockedShelf == 0)
            return;

        Shelf[] allShelves = FindObjectsOfType<Shelf>();
        foreach (Shelf shelf in allShelves)
        {
            bool wasRestocked = data.restockedShelfIds.Contains(shelf.ShelfIndex);
            if (wasRestocked)
            {
                shelf.RestoreState(wasRestocked);
            }
        }
        GameManager.Instance.RestoreCount(data.restockedShelf, data.restockedShelfIds);
    }
    public void SaveHighScore()
    {
        if (PlayerPrefs.HasKey("TotalWin"))
        {
            int currentHighScore = PlayerPrefs.GetInt("TotalWin");
            PlayerPrefs.SetInt("TotalWin", currentHighScore + 1);
        }
        else
        {
            PlayerPrefs.SetInt("TotalWin", 1);
        }
        PlayerPrefs.Save();
    }

    private void HandleShelfRestocked(int restocked, int required)
    {
        GameSaveData data = new GameSaveData();
        data.restockedShelf = restocked;
        data.restockedShelfIds = GameManager.Instance.RestockedShelfIds;
        SaveGame(data);
    }

    private void ResetSaveData()
    {
        GameSaveData data = new GameSaveData();
        data.restockedShelf = 0;
        data.restockedShelfIds = new List<int>();
        SaveGame(data);
    }
    private void HandleGameWon()
    {
        ResetSaveData();
        SaveHighScore();
    }

}