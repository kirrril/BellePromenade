using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    [SerializeField]
    private EntrySceneManager entrySceneManager;
    public static Player Instance;
    public PlayerProfile currentPlayer;

    public event Action localSavePlayer;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentPlayer = new PlayerProfile();
    }

    void OnEnable()
    {
        LocalSave.Instance.loadPlayer += LoadPlayerProfile;
    }

    void OnDisable()
    {
        LocalSave.Instance.loadPlayer -= LoadPlayerProfile;
    }

    private void LoadPlayerProfile(List<string> visitedCities, int experience)
    {
        currentPlayer.visitedCities = visitedCities;
        currentPlayer.experience = experience;
    }

    public void AddVisitedCity(string city)
    {
        currentPlayer.visitedCities.Add(city);
        currentPlayer.lastVisitedCity = city;
        currentPlayer.experience = IncreaseExperience();

        localSavePlayer?.Invoke();
    }

    private int IncreaseExperience()
    {
        return currentPlayer.visitedCities.Count;
    }



    [System.Serializable]
    public class PlayerProfile
    {
        public List<string> visitedCities;
        public int experience;
        public string lastVisitedCity;

        public PlayerProfile()
        {
            visitedCities = new List<string>();
            experience = 0;
            lastVisitedCity = string.Empty;
        }
    }
}
