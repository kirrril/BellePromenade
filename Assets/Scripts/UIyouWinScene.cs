using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIyouWinScene : MonoBehaviour
{
    [SerializeField]
    private YouWinSceneManager youWinSceneManager;
    private UIDocument youWinUIdocument;
    private VisualElement root;
    private Button startButton;
    private Button signInButton;
    private VisualElement signedIn;
    private Label signInMessage;
    private Label greeting;
    private VisualElement startGameGlow;

    private float baseOpacity = 1f;
    float flickeringFrequency = 1f;

    public event Action startButtonClicked;
    public event Action signInButtonClicked;

    void Awake()
    {
        youWinUIdocument = GetComponent<UIDocument>();
        root = youWinUIdocument.rootVisualElement;
        startButton = root.Q<Button>("startButton");
        signInButton = root.Q<Button>("signInButton");
        signedIn = root.Q<VisualElement>("signedIn");
        greeting = root.Q<Label>("youWinMessage");
        signInMessage = root.Q<Label>("signInMessage");
        startGameGlow = root.Q<VisualElement>("startGameGlow");
    }

    void Start()
    {
        DisplayViewAndGreeting();

        if (CloudSave.Instance != null && CloudSave.Instance.isConnected)
        {
            signInButton.style.display = DisplayStyle.None;
            signInMessage.style.display = DisplayStyle.None;
            signedIn.style.display = DisplayStyle.Flex;
        }
        else if (CloudSave.Instance != null && !CloudSave.Instance.isConnected || CloudSave.Instance == null)
        {
            signInButton.style.display = DisplayStyle.Flex;
            signInMessage.style.display = DisplayStyle.Flex;
            signedIn.style.display = DisplayStyle.None;
        }
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * flickeringFrequency, 1f);
        float opacity = Mathf.Lerp(0.1f, 1.0f, t);
        startGameGlow.style.opacity = baseOpacity * opacity;
    }

    void OnEnable()
    {
        startButton.RegisterCallback<ClickEvent>(HandleStartButtonClick);
        signInButton.RegisterCallback<ClickEvent>(HandleSignInButtonClick);
    }

    private void HandleStartButtonClick(ClickEvent evt)
    {
        startButtonClicked?.Invoke();
    }

    private void HandleSignInButtonClick(ClickEvent evt)
    {
        signInButtonClicked?.Invoke();
    }

    void OnDisable()
    {
        startButton.UnregisterCallback<ClickEvent>(HandleStartButtonClick);
        signInButton.UnregisterCallback<ClickEvent>(HandleSignInButtonClick);
    }

    private void DisplayViewAndGreeting()
    {
        greeting.text = $"Welcome to {Player.Instance.currentPlayer.lastVisitedCity}! Continue your journey?";
    }
}