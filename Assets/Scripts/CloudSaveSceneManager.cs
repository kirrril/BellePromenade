using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class CloudSaveSceneManager : MonoBehaviour
{
    [SerializeField]
    private UIcloudSaveScene uiCloudSaveScene;

    // public event Action tryConnectCloud;
    public event Action connectionOK;
    public event Action connectionFailed;
    public event Action<string, string> trySignIn;
    public event Action<string, string> trySignUp;
    public event Action<string> displayAuthError;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        uiCloudSaveScene.signInButtonClick += HandleSignIn;
        uiCloudSaveScene.signUpButtonClick += HandleSignUp;
        uiCloudSaveScene.exitButtonClick += HandleExit;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        uiCloudSaveScene.signInButtonClick -= HandleSignIn;
        uiCloudSaveScene.signUpButtonClick -= HandleSignUp;
        uiCloudSaveScene.exitButtonClick -= HandleExit;

        if (CloudSave.Instance != null)
        {
            CloudSave.Instance.cloudConnected -= HandleSuccessfulConnection;
            CloudSave.Instance.connectionFailed -= HandleFailedConnection;
            CloudSave.Instance.authentificationErrorWarning -= HandleAuthentificationError;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CloudSaveScene")
        {
            // tryConnectCloud?.Invoke();

            CloudSave.Instance.cloudConnected += HandleSuccessfulConnection;
            CloudSave.Instance.connectionFailed += HandleFailedConnection;
            CloudSave.Instance.authentificationErrorWarning += HandleAuthentificationError;
        }
    }

    private void HandleSuccessfulConnection()
    {
        connectionOK?.Invoke();
    }

    private void HandleFailedConnection()
    {
        connectionFailed?.Invoke();
    }

    private void HandleSignIn(string email, string password)
    {
        trySignIn?.Invoke(email, password);
    }

    private void HandleSignUp(string email, string password)
    {
        trySignUp?.Invoke(email, password);
    }

    private void HandleAuthentificationError(string errorMessage)
    {
        displayAuthError?.Invoke(errorMessage);
    }

    private void HandleExit()
    {
        SceneManager.LoadScene(CloudSave.previousScene);
    }
}
