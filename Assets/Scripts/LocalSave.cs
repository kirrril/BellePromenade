using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class LocalSave : MonoBehaviour
{
    public static LocalSave Instance;
    string filePath;
    public event Action<List<string>, int> loadPlayer;
    public event Action saveInCloud;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        filePath = Application.persistentDataPath + "/PlayerProfile.json";
        if (!File.Exists(filePath)) LocalSavePlayerProfile();
        
    }

    void Start()
    {
        LoadPlayerProfile();
        Player.Instance.localSavePlayer += LocalSavePlayerProfile;
    }

    private void LocalSavePlayerProfile()
    {
        filePath = Application.persistentDataPath + "/PlayerProfile.json";
        string profileUpdate = JsonUtility.ToJson(Player.Instance.currentPlayer);
        System.IO.File.WriteAllText(filePath, profileUpdate);
        saveInCloud?.Invoke();
    }

    private void LoadPlayerProfile()
    {
        filePath = Application.persistentDataPath + "/PlayerProfile.json";
        string playerProfile = System.IO.File.ReadAllText(filePath);
        var currentPlayerLocalSave = JsonUtility.FromJson<Player.PlayerProfile>(playerProfile);
        Debug.Log(filePath);
        Debug.Log(currentPlayerLocalSave.visitedCities.Count);
        loadPlayer?.Invoke(currentPlayerLocalSave.visitedCities, currentPlayerLocalSave.visitedCities.Count);
    }

    private void DeleteLocalSave()
    {
        Debug.Log("DeleteLocalSave called");
        string filePath = Application.persistentDataPath + "/PlayerProfile.json";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public void InitLocalSave()
    {
        DeleteLocalSave();
        Player.Instance.currentPlayer = new Player.PlayerProfile();
        LocalSavePlayerProfile();
        LoadPlayerProfile();
    }
}
