using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIentryScene : MonoBehaviour
{
    [SerializeField]
    private EntrySceneManager entrySceneManager;

    [SerializeField]
    private UIDocument entryUIdocument;

    [SerializeField]
    private VisualTreeAsset phoneUxml;

    [SerializeField]
    private VisualTreeAsset tabletUxml;

    private VisualElement root;
    private Button startButton;
    private Button signInButton;
    private VisualElement signedIn;
    private VisualElement startGameGlow;
    private VisualElement roadSign;
    private VisualElement cockpit;
    public event Action startButtonClick;
    public event Action signInButtonClick;

    private float baseOpacity = 1f;
    float flickeringFrequency = 1f;
    public static bool isTablet;


    void Awake()
    {
        float screenAspectRatio = (float)Screen.width / Screen.height;
        // float screenAspectRatio = 2340 / 1080;
        isTablet = screenAspectRatio < 1.7f;

        entryUIdocument.visualTreeAsset = isTablet ? tabletUxml : phoneUxml;

        root = entryUIdocument.rootVisualElement;
        startButton = root.Q<Button>("startButton");
        signInButton = root.Q<Button>("signInButton");
        signedIn = root.Q<VisualElement>("signedIn");
        startGameGlow = root.Q<VisualElement>("startGameGlow");


    }

    void Start()
    {
        if (CloudSave.Instance != null && CloudSave.Instance.isConnected)
        {
            signInButton.style.display = DisplayStyle.None;
            signedIn.style.display = DisplayStyle.Flex;
        }
        else if (CloudSave.Instance != null && !CloudSave.Instance.isConnected || CloudSave.Instance == null)
        {
            signInButton.style.display = DisplayStyle.Flex;
            signedIn.style.display = DisplayStyle.None;
        }
    }

    void OnEnable()
    {
        startButton.RegisterCallback<ClickEvent>(HandleStartButtonClick);
        signInButton.RegisterCallback<ClickEvent>(HandleSignInButtonClick);
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * flickeringFrequency, 1f);
        float opacity = Mathf.Lerp(0.1f, 1.0f, t);
        startGameGlow.style.opacity = baseOpacity * opacity;
    }

    private void HandleStartButtonClick(ClickEvent evt)
    {
        startButtonClick?.Invoke();
    }

    private void HandleSignInButtonClick(ClickEvent evt)
    {
        signInButtonClick?.Invoke();
    }

    void OnDisable()
    {
        startButton.UnregisterCallback<ClickEvent>(HandleStartButtonClick);
        signInButton.UnregisterCallback<ClickEvent>(HandleSignInButtonClick);
    }
}