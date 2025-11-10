using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIyouLoseScene : MonoBehaviour
{
    [SerializeField]
    private YouLoseSceneManager youLoseSceneManager;
    private UIDocument youLoseUIdocument;
    private VisualElement root;
    private Button startButton;
    private Button signInButton;
    private VisualElement signedIn;
    private Label signInMessage;
    private VisualElement startGameGlow;

    private float baseOpacity = 1f;
    float flickeringFrequency = 1f;

    public event Action startButtonClicked;
    public event Action signInButtonClicked;

    void Awake()
    {
        youLoseUIdocument = GetComponent<UIDocument>();
        root = youLoseUIdocument.rootVisualElement;
        startButton = root.Q<Button>("startButton");
        signInButton = root.Q<Button>("signInButton");
        signedIn = root.Q<VisualElement>("signedIn");
        signInMessage = root.Q<Label>("signInMessage");
        startGameGlow = root.Q<VisualElement>("startGameGlow");
    }

    void Start()
    {
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
}
