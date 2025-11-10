using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Threading.Tasks;

public class CloudSave : MonoBehaviour
{
    [SerializeField]
    private CloudSaveSceneManager cloudSaveSceneManager;

    public static CloudSave Instance;
    public event Action<string> authentificationErrorWarning;
    public event Action cloudConnected;
    public event Action connectionFailed;
    public bool isLoading;
    public bool isConnected;
    public static string previousScene;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitializeCloud();
    }

    void OnEnable()
    {
        // cloudSaveSceneManager.tryConnectCloud += InitializeCloud;
        cloudSaveSceneManager.trySignIn += HandleSignIn;
        cloudSaveSceneManager.trySignUp += HandleSignUp;
        LocalSave.Instance.saveInCloud += SaveData;
    }

    void OnDisable()
    {
        // cloudSaveSceneManager.tryConnectCloud -= InitializeCloud;
        cloudSaveSceneManager.trySignIn -= HandleSignIn;
        cloudSaveSceneManager.trySignUp -= HandleSignUp;
        LocalSave.Instance.saveInCloud -= SaveData;
    }

    private async void InitializeCloud()
    {
        await InitializeCloudAsync();
    }

    public async Task InitializeCloudAsync()
    {
        isLoading = true;
        Debug.Log("Initializing Cloud");
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            await Awaitable.WaitForSecondsAsync(1);
            Debug.Log("Cloud connected");
            AuthenticationService.Instance.SignOut();
            AuthenticationService.Instance.ClearSessionToken();
            isLoading = false;
            cloudConnected?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            isLoading = false;
            connectionFailed?.Invoke();
        }
        finally
        {
            AuthenticationService.Instance.SignOut();
            AuthenticationService.Instance.ClearSessionToken();
        }
    }

    private async void HandleSignIn(string username, string password)
    {
        await SignInWithUsernamePasswordAsync(username, password);
    }

    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        isLoading = true;
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("SignIn is successful.");
            isLoading = false;
            isConnected = true;
            await LoadPlayerStartGame(username);
        }
        catch (AuthenticationException ex)
        {
            string fullMessage = ex.ToString();
            Debug.LogError(fullMessage);
            isLoading = false;
            isConnected = false;
            authentificationErrorWarning?.Invoke("Unknown username\nor password.\nPlease correct your input\nor create a new account.");
        }
        catch (RequestFailedException ex)
        {
            isLoading = false;
            isConnected = false;
            authentificationErrorWarning?.Invoke("Unknown username\nor password.\nPlease correct your input\nor create a new account.");
            Debug.LogException(ex);
        }
    }

    private async Task LoadPlayerStartGame(string username)
    {
        isLoading = true;

        var keys = new HashSet<string> { "visitedCities" };
        var cloudData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);
        if (cloudData != null)
        {
            cloudData.TryGetValue("visitedCities", out var citiesObj);
            List<string> citiesInCloud = citiesObj.Value.GetAs<List<string>>();

            if (citiesInCloud.Count > Player.Instance.currentPlayer.visitedCities.Count)
            {
                Player.Instance.currentPlayer.visitedCities = citiesInCloud;
                Player.Instance.currentPlayer.experience = citiesInCloud.Count;
            }
            else
            {
                SaveData();
            }
        }

        isLoading = false;

        SceneManager.LoadScene(previousScene);
    }

    private async void HandleSignUp(string username, string password)
    {
        await SignUpWithUsernamePasswordAsync(username, password);
    }

    async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            isLoading = true;
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("SignUp is successful.");
            isLoading = false;
            isConnected = true;
            SaveData();
            SceneManager.LoadScene(previousScene);
        }
        catch (AuthenticationException ex)
        {
            isLoading = false;
            isConnected = false;
            authentificationErrorWarning?.Invoke("This username is not available for creating a new account.");
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            isLoading = false;
            isConnected = false;
            authentificationErrorWarning?.Invoke("This username is not available for creating a new account.");
            Debug.LogException(ex);
        }
    }

    private async void SaveData()
    {
        if (isConnected)
        {
            await SavingData();
        }
    }

    public async Task SavingData()
    {
        var playerProgress = new Dictionary<string, object>
        {
            {"visitedCities", Player.Instance.currentPlayer.visitedCities},
        };

        await CloudSaveService.Instance.Data.Player.SaveAsync(playerProgress);
    }
}
