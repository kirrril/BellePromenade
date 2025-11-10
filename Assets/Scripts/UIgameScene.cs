using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIgameScene : MonoBehaviour
{
    [SerializeField]
    private GameSceneManager gameSceneManager;

    [SerializeField]
    private UIDocument gameUIdocument;
    
    [SerializeField]
    private VisualTreeAsset phoneUxml;

    [SerializeField]
    private VisualTreeAsset tabletUxml;

    [SerializeField]
    private AudioController audioController;

    private TouchScreenKeyboard keyboard;

    [SerializeField]
    private List<Sprite> fuelLevel;

    [SerializeField]
    private List<Sprite> xpLevel;

    private VisualElement root;
    private VisualElement boards;
    private VisualElement leftBoard;
    private VisualElement centralBoard;
    private VisualElement rightBoard;
    private VisualElement beam;
    private VisualElement centralLine1;
    private VisualElement centralLine2;
    private TextField playerInputField;
    private Button inputButton;
    private Label cityName;
    private Label alreadyPlayed;
    private Label comment;
    private VisualElement remainingFuel;
    private VisualElement xpScale;
    private VisualElement signedIn;
    private VisualElement radioLCD;
    private Button signInButton;
    private Button radioPlayButton;
    private Button radioPauseButton;

    private Button allWinButton;

    public event Action<string> PlayerMadeInput;
    public event Action signInButtonClicked;
    // private bool radioSwitch = true;

    void Awake()
    {
        gameUIdocument.visualTreeAsset = UIentryScene.isTablet ? tabletUxml : phoneUxml;

        root = gameUIdocument.rootVisualElement;
        boards = root.Q<VisualElement>("boards");
        leftBoard = root.Q<VisualElement>("leftBoard");
        centralBoard = root.Q<VisualElement>("centralBoard");
        rightBoard = root.Q<VisualElement>("rightBoard");
        beam = root.Q<VisualElement>("beam");
        centralLine1 = root.Q<VisualElement>("centralLine1");
        centralLine2 = root.Q<VisualElement>("centralLine2");
        playerInputField = root.Q<TextField>("playerInput");
        inputButton = root.Q<Button>("inputButton");
        cityName = root.Q<Label>("cityName");
        alreadyPlayed = root.Q<Label>("alreadyPlayedList");
        comment = root.Q<Label>("commentMessage");
        remainingFuel = root.Q<VisualElement>("remainingFuel");
        xpScale = root.Q<VisualElement>("scaleXP");
        signedIn = root.Q<VisualElement>("signedIn");
        radioLCD = root.Q<VisualElement>("radioLCD");
        signInButton = root.Q<Button>("signInButton");
        radioPlayButton = root.Q<Button>("radioPlay");
        radioPauseButton = root.Q<Button>("radioPause");

        allWinButton = root.Q<Button>("allWinButton");
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

        RadioManager();
    }

    void Update()
    {
        if (keyboard != null)
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Done)
            {
                OnPlayerInputTouch(keyboard.text);
                keyboard = null;
            }
            else if (keyboard.status == TouchScreenKeyboard.Status.Canceled)
            {
                keyboard = null;
            }
        }
    }

    void OnEnable()
    {
        gameSceneManager.DisplayMaskedCityFirstTime += DisplayCityFirstTime;
        gameSceneManager.ShowCityToGuess += MoveAndUpdateCity;
        gameSceneManager.AlreadyPlayedUpdate += UpdateAlreadyPlayed;
        gameSceneManager.FuelUpdate += UpdateFuelLevel;
        gameSceneManager.CommentUpdate += UpdateComment;
        gameSceneManager.UpdateXP += UpdateXP;
        radioPlayButton.clicked += RadioOnOff;
        radioPauseButton.clicked += RadioOnOff;

        inputButton.RegisterCallback<ClickEvent>(HandleInputButtonClick);

//////////////

        // playerInputField.RegisterCallback<KeyUpEvent>(HandleEndEdit);

//////////////

        signInButton.RegisterCallback<ClickEvent>(HandleSignInClick);

        allWinButton.RegisterCallback<ClickEvent>(evt => SceneManager.LoadScene("AllWinScene"));
    }

    void OnDisable()
    {
        gameSceneManager.DisplayMaskedCityFirstTime -= DisplayCityFirstTime;
        gameSceneManager.ShowCityToGuess -= MoveAndUpdateCity;
        gameSceneManager.AlreadyPlayedUpdate -= UpdateAlreadyPlayed;
        gameSceneManager.FuelUpdate -= UpdateFuelLevel;
        gameSceneManager.CommentUpdate -= UpdateComment;
        gameSceneManager.UpdateXP -= UpdateXP;
        radioPlayButton.clicked -= RadioOnOff;
        radioPauseButton.clicked -= RadioOnOff;

        inputButton.UnregisterCallback<ClickEvent>(HandleInputButtonClick);


        //////////////

        // playerInputField.UnregisterCallback<KeyUpEvent>(HandleEndEdit);
        
        //////////////

        signInButton.UnregisterCallback<ClickEvent>(HandleSignInClick);
    }

    private void HandleInputButtonClick(ClickEvent evt)
    {
        // playerInputField.Focus();

//////////////

        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }


    //////////////

    // private void HandleEndEdit(KeyUpEvent evt)
    // {
    //     if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
    //     {
    //         OnPlayerInputPC();
    //         evt.StopPropagation();
    //         evt.StopImmediatePropagation();
    //     }
    // }

    // private void OnPlayerInputPC()
    // {
    //     string playerInput = playerInputField.value;
    //     if (!string.IsNullOrWhiteSpace(playerInput))
    //     {
    //         PlayerMadeInput?.Invoke(playerInput.Trim());
    //         playerInputField.value = "";
    //         playerInputField.Blur();
    //     }
    // }


    //////////////

    public void RadioOnOff()
    {
        AudioController.musicIsOn = !AudioController.musicIsOn;
        RadioManager();
    }

    private void RadioManager()
    {
        if (AudioController.musicIsOn)
        {
            radioPlayButton.style.visibility = Visibility.Hidden;
            radioPauseButton.style.visibility = Visibility.Visible;
            radioLCD.style.visibility = Visibility.Visible;
        }
        else
        {
            radioPlayButton.style.visibility = Visibility.Visible;
            radioPauseButton.style.visibility = Visibility.Hidden;
            radioLCD.style.visibility = Visibility.Hidden;
        }
    }

    private void OnPlayerInputTouch(string input)
    {
        if (!string.IsNullOrWhiteSpace(input))
        {
            PlayerMadeInput?.Invoke(input.Trim());
        }
    }

    private void HandleSignInClick(ClickEvent evt)
    {
        signInButtonClicked?.Invoke();
    }

    private void DisplayCityFirstTime(string cityMasked)
    {
        cityName.text = cityMasked;
    }

    private void MoveAndUpdateCity(string cityUpdated)
    {
        StartCoroutine(MoveForwardUpdateCity(boards, cityUpdated));
        StartCoroutine(RepeatVibration(0.9f));
        StartCoroutine(AnimateCentralLine(0.9f));
    }

    private void UpdateAlreadyPlayed(List<string> alreadyPlayedList)
    {
        string alreadyPlayedUpToDate = string.Empty;

        for (int i = 0; i < alreadyPlayedList.Count; i++)
        {
            if (alreadyPlayedUpToDate == "")
            {
                alreadyPlayedUpToDate += $"{alreadyPlayedList[i].ToUpper()}";
            }
            else
            {
                alreadyPlayedUpToDate += $", {alreadyPlayedList[i].ToUpper()}";
            }
        }

        if (alreadyPlayedUpToDate == "")
        {
            alreadyPlayed.text = "NO LETTERS HAVE BEEN PLAYED YET";
        }
        else
        {
            alreadyPlayed.text = alreadyPlayedUpToDate;
        }
    }

    private void UpdateFuelLevel(int level)
    {
        switch (level)
        {
            case 50:
                remainingFuel.style.backgroundImage = new StyleBackground(fuelLevel[0]);
                break;
            case 45:
                remainingFuel.style.backgroundImage = new StyleBackground(fuelLevel[1]);
                break;
            case 40:
                remainingFuel.style.backgroundImage = new StyleBackground(fuelLevel[2]);
                break;
            case 35:
                remainingFuel.style.backgroundImage = new StyleBackground(fuelLevel[3]);
                break;
            case 30:
                remainingFuel.style.backgroundImage = new StyleBackground(fuelLevel[4]);
                break;
            case 25:
                remainingFuel.style.backgroundImage = new StyleBackground(fuelLevel[5]);
                break;
            case 20:
                remainingFuel.style.backgroundImage = new StyleBackground(fuelLevel[6]);
                break;
            case 15:
                remainingFuel.style.backgroundImage = new StyleBackground(fuelLevel[7]);
                break;
            case 10:
                remainingFuel.style.backgroundImage = new StyleBackground(fuelLevel[8]);
                break;
            case 5:
                remainingFuel.style.backgroundImage = new StyleBackground(fuelLevel[9]);
                break;
            default:
                remainingFuel.style.backgroundImage = new StyleBackground(fuelLevel[10]);
                break;
        }
    }

    private void UpdateComment(string commentUpdate)
    {
        comment.text = commentUpdate;
    }

    private void UpdateXP(int xp)
    {
        xpScale.style.backgroundImage = new StyleBackground(xpLevel[xp]);
    }

    private IEnumerator RepeatVibration(float duration, float interval = 0.05f)
    {
        float elapsed = 0f;
        float lastVibration = -interval;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (elapsed - lastVibration >= interval)
            {
                Handheld.Vibrate();
                lastVibration = elapsed;
            }
            yield return null;
        }
    }

    private IEnumerator AnimateCentralLine(float duration, float interval = 0.1f)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            centralLine1.style.opacity = 100;
            centralLine2.style.opacity = 0;
            yield return new WaitForSeconds(interval);
            centralLine1.style.opacity = 0;
            centralLine2.style.opacity = 100;
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }

    private IEnumerator MoveForwardUpdateCity(VisualElement billboards, string cityUpdate)
    {
        audioController.isAccelerating = true;

        float duration = 0.6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            billboards.style.scale = new Scale(new Vector2(Mathf.Lerp(1f, 10f, t), Mathf.Lerp(1f, 10f, t)));
            billboards.style.translate = new Translate(0, Mathf.Lerp(0, -1600, t), 0);
            leftBoard.style.opacity = Mathf.Lerp(1f, 0f, t);
            centralBoard.style.opacity = Mathf.Lerp(1f, 0f, t);
            rightBoard.style.opacity = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        billboards.style.scale = new Scale(new Vector2(0.7f, 0.7f));
        billboards.style.translate = new Translate(0, -65, 0);
        beam.style.opacity = 0.5f;
        leftBoard.style.opacity = 1f;
        centralBoard.style.opacity = 1f;
        rightBoard.style.opacity = 1f;

        elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            billboards.style.translate = new Translate(0, Mathf.Lerp(-65, 0, t), 0);
            billboards.style.scale = new Scale(new Vector2(Mathf.Lerp(0.7f, 1f, t), Mathf.Lerp(0.7f, 1f, t)));
            beam.style.opacity = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        audioController.isAccelerating = false;
        cityName.text = cityUpdate;
    }
}
