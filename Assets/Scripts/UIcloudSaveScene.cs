using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class UIcloudSaveScene : MonoBehaviour
{
    [SerializeField]
    private CloudSaveSceneManager cloudSaveSceneManager;

    [SerializeField]
    private UIDocument cloudSaveUIdocument;

    [SerializeField]
    private VisualTreeAsset phoneUxml;

    [SerializeField]
    private VisualTreeAsset tabletUxml;

    private VisualElement root;
    private Label connectionFailedWarning;
    private VisualElement authentificationForm;
    private VisualElement authentificationError;
    private VisualElement loader;
    private VisualElement wiper;
    private float wiperRotation = 0f;
    private int wiperDirection = 1;
    private Label authErrorText;
    private Label signInUsernameWarning;
    private Label signUpUsernameWarning;
    private Label signInPasswordWarning;
    private Label signUpPasswordWarning;
    private TextField signInUsernameInput;
    private TextField signUpUsernameInput;
    private TextField signInPasswordInput;
    private TextField signUpPasswordInput;
    private TextField confirmPasswordInput;
    private Button signInButton;
    private Button signUpButton;
    private Button exitButton;
    private Button okButton;
    public event Action<string, string> signInButtonClick;
    public event Action<string, string> signUpButtonClick;
    public event Action exitButtonClick;
    private EventCallback<ChangeEvent<string>> signInUsernameChangedCallback;
    private EventCallback<ChangeEvent<string>> signInPasswordChangedCallback;
    private EventCallback<ChangeEvent<string>> signUpUsernameChangedCallback;
    private EventCallback<ChangeEvent<string>> signUpPasswordChangedCallback;
    private EventCallback<ChangeEvent<string>> confirmPasswordChangedCallback;


    void Awake()
    {
        cloudSaveUIdocument.visualTreeAsset = UIentryScene.isTablet ? tabletUxml : phoneUxml;

        root = cloudSaveUIdocument.rootVisualElement;
        connectionFailedWarning = root.Q<Label>("connectionFailedWarning");
        authentificationForm = root.Q<VisualElement>("authentificationForm");
        authentificationError = root.Q<VisualElement>("authentificationError");
        loader = root.Q<VisualElement>("loader");
        wiper = root.Q<VisualElement>("wiper");
        authErrorText = root.Q<Label>("authErrorText");
        signInUsernameWarning = root.Q<Label>("signInUsernameWarning");
        signUpUsernameWarning = root.Q<Label>("signUpUsernameWarning");
        signInPasswordWarning = root.Q<Label>("signInPasswordWarning");
        signUpPasswordWarning = root.Q<Label>("signUpPasswordWarning");
        signInUsernameInput = root.Q<TextField>("signInUsernameInput");
        signUpUsernameInput = root.Q<TextField>("signUpUsernameInput");
        signInPasswordInput = root.Q<TextField>("signInPasswordInput");
        signUpPasswordInput = root.Q<TextField>("signUpPasswordInput");
        confirmPasswordInput = root.Q<TextField>("confirmPasswordInput");
        signInButton = root.Q<Button>("signInButton");
        signUpButton = root.Q<Button>("signUpButton");
        exitButton = root.Q<Button>("exitButton");
        okButton = root.Q<Button>("okButton");

        InitAuthorisationPanel();
    }

    void OnEnable()
    {
        cloudSaveSceneManager.connectionOK += ConnectionTestOk;
        cloudSaveSceneManager.connectionFailed += ConnectionTestFailed;
        cloudSaveSceneManager.displayAuthError += DisplayAuthentificationError;
        exitButton.RegisterCallback<ClickEvent>(HandleExitButtonClick);
        okButton.RegisterCallback<ClickEvent>(HandleOkButtonClick);
        signInUsernameChangedCallback = OnSignInUsernameValueChange;
        signInUsernameInput.RegisterValueChangedCallback(signInUsernameChangedCallback);
        signInPasswordChangedCallback = OnSignInPasswordValueChange;
        signInPasswordInput.RegisterValueChangedCallback(signInPasswordChangedCallback);
        signInButton.RegisterCallback<ClickEvent>(HandleSignInButtonClick);
        signUpUsernameChangedCallback = OnSignUpUsernameValueChange;
        signUpUsernameInput.RegisterValueChangedCallback(signUpUsernameChangedCallback);
        signUpPasswordChangedCallback = OnSignUpPasswordValueChange;
        signUpPasswordInput.RegisterValueChangedCallback(signUpPasswordChangedCallback);
        confirmPasswordChangedCallback = OnConfirmPasswordValueChange;
        confirmPasswordInput.RegisterValueChangedCallback(confirmPasswordChangedCallback);
        signUpButton.RegisterCallback<ClickEvent>(HandleSignUpButtonClick);
    }

    private void ConnectionTestOk()
    {
        authentificationForm.style.display = DisplayStyle.Flex;
    }

    private void ConnectionTestFailed()
    {
        Debug.Log("Connection Failed");
        connectionFailedWarning.style.display = DisplayStyle.Flex;
    }

    private void HandleExitButtonClick(ClickEvent evt)
    {
        OnExitButtonClick();
    }

    private void HandleOkButtonClick(ClickEvent evt)
    {
        OnOkButtonClick();
    }

    private void HandleSignInButtonClick(ClickEvent evt)
    {
        OnSignInButtonClick();
    }

    private void HandleSignUpButtonClick(ClickEvent evt)
    {
        OnSignUpButtonClick();
    }

    void OnDisable()
    {
        cloudSaveSceneManager.connectionOK -= ConnectionTestOk;
        cloudSaveSceneManager.connectionFailed -= ConnectionTestFailed;
        cloudSaveSceneManager.displayAuthError -= DisplayAuthentificationError;
        exitButton.UnregisterCallback<ClickEvent>(HandleExitButtonClick);
        okButton.UnregisterCallback<ClickEvent>(HandleOkButtonClick);
        signInUsernameInput.UnregisterValueChangedCallback(signInUsernameChangedCallback);
        signInPasswordInput.UnregisterValueChangedCallback(signInPasswordChangedCallback);
        signInButton.UnregisterCallback<ClickEvent>(HandleSignInButtonClick);
        signUpUsernameInput.UnregisterValueChangedCallback(signUpUsernameChangedCallback);
        signUpPasswordInput.UnregisterValueChangedCallback(signUpPasswordChangedCallback);
        confirmPasswordInput.UnregisterValueChangedCallback(confirmPasswordChangedCallback);
        signUpButton.UnregisterCallback<ClickEvent>(HandleSignUpButtonClick);
    }

    void Update()
    {
        if (CloudSave.Instance.isLoading)
        {
            loader.style.display = DisplayStyle.Flex;
            wiperRotation += wiperDirection * 180f * Time.deltaTime;

            if (wiperRotation >= 180f)
            {
                wiperRotation = 180f;
                wiperDirection = -1;
            }
            else if (wiperRotation <= 0f)
            {
                wiperRotation = 0f;
                wiperDirection = 1;
            }

            wiper.style.rotate = new Rotate(wiperRotation);
        }
        else if (!CloudSave.Instance.isLoading)
        {
            loader.style.display = DisplayStyle.None;
            wiperRotation = 0;
            wiperDirection = 1;
        }
    }

    private void InitAuthorisationPanel()
    {
        loader.style.display = DisplayStyle.None;
        connectionFailedWarning.style.display = DisplayStyle.None;
        authentificationForm.style.display = DisplayStyle.None;
        authentificationError.style.display = DisplayStyle.None;
        signInUsernameWarning.text = "Min 3 max 10 chars, only letters or numbers or underscore.";
        signInPasswordWarning.text = "Min 8 chars, with uppercase, lowercase, number, and underscore.";
        signInButton.SetEnabled(false);
        signUpUsernameWarning.text = "Min 3 max 10 chars, only letters or numbers or underscore.";
        signUpPasswordWarning.text = "Min 8 chars, with uppercase, lowercase, number, and underscore.";
        signUpButton.SetEnabled(false);
    }

    private void OnSignInUsernameValueChange(ChangeEvent<string> evt)
    {
        if (CheckUsername(evt.newValue))
        {
            signInUsernameWarning.text = "Username OK";
        }
        else
        {
            signInUsernameWarning.text = "Min 3 max 10 chars, only letters or numbers or underscore.";
        }
    }

    private void OnSignInPasswordValueChange(ChangeEvent<string> evt)
    {
        if (CheckPassword(evt.newValue))
        {
            signInPasswordWarning.text = "Password OK";
            signInButton.SetEnabled(true);
        }
        else
        {
            signInPasswordWarning.text = "Min 8 chars, with uppercase, lowercase, number, and underscore.";
            signInButton.SetEnabled(false);
        }
    }


    private void OnSignUpUsernameValueChange(ChangeEvent<string> evt)
    {
        if (CheckUsername(evt.newValue))
        {
            signUpUsernameWarning.text = "Username OK";
        }
        else
        {
            signUpUsernameWarning.text = "Min 3 max 10 chars, only letters or numbers or underscore.";
        }
    }

    private void OnSignUpPasswordValueChange(ChangeEvent<string> evt)
    {
        if (CheckPassword(evt.newValue))
        {
            signUpPasswordWarning.text = "Password OK";
        }
        else
        {
            signUpPasswordWarning.text = "Min 8 chars, with uppercase, lowercase, number, and underscore.";
        }
        confirmPasswordInput.value = "";
    }

    private void OnConfirmPasswordValueChange(ChangeEvent<string> evt)
    {
        if (evt.newValue != "" && evt.newValue != signUpPasswordInput.value)
        {
            confirmPasswordInput.style.color = new Color(1, 0, 0);
            signUpButton.SetEnabled(false);
        }
        else
        {
            confirmPasswordInput.style.color = new Color(0, 0, 0);
            signUpButton.SetEnabled(true);
        }
    }

    private bool CheckUsername(string usernameFieldValue)
    {
        string pattern = @"^[A-Za-z0-9_]{3,20}$";
        return Regex.IsMatch(usernameFieldValue, pattern);
    }

    private bool CheckPassword(string passwordFieldValue)
    {
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*_)[A-Za-z\d_]{8,30}$";
        return Regex.IsMatch(passwordFieldValue, pattern);
    }

    private void DisplayAuthentificationError(string errorMessage)
    {
        authentificationError.style.display = DisplayStyle.Flex;
        authErrorText.text = errorMessage;
        signInButton.SetEnabled(false);
        signUpButton.SetEnabled(false);
        signInUsernameInput.value = "";
        signInPasswordInput.value = "";
        signUpUsernameInput.value = "";
        signUpPasswordInput.value = "";
    }

    private void OnSignInButtonClick()
    {
        signInButtonClick?.Invoke(signInUsernameInput.value, signInPasswordInput.value);
    }

    private void OnSignUpButtonClick()
    {
        signUpButtonClick?.Invoke(signUpUsernameInput.value, signUpPasswordInput.value);
    }

    private void OnExitButtonClick()
    {
        exitButtonClick?.Invoke();
    }

    private void OnOkButtonClick()
    {
        authentificationError.style.display = DisplayStyle.None;
    }
}