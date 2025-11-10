
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    [SerializeField]
    private List<string> allCitiesList;

    [SerializeField]
    private UIgameScene uiGameScene;

    private string cityToGuess;

    public List<string> alreadyPlayed = new List<string>();

    private List<string> notYetGuessedCities = new List<string>();

    private int remainingFuel = 50;

    private string cityDisplayVersion = string.Empty;

    private string inputFirstLetter;

    public event Action<string> ShowCityToGuess;
    public event Action<string> DisplayMaskedCityFirstTime;
    public event Action<string> StartLoadingTextures;
    public event Action<List<string>> AlreadyPlayedUpdate;
    public event Action<int> FuelUpdate;
    public event Action<string> CommentUpdate;
    public event Action<int> UpdateXP;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        uiGameScene.PlayerMadeInput += DisplayCityToGuess;
        uiGameScene.PlayerMadeInput += UpdateFuelLevel;
        uiGameScene.PlayerMadeInput += UpdateComment;
        uiGameScene.PlayerMadeInput += UpdateAlreadyPlayed;
        uiGameScene.PlayerMadeInput += GameLogic;
        uiGameScene.signInButtonClicked += Connect;
    }

    void OnDisable()
    {
        uiGameScene.PlayerMadeInput -= DisplayCityToGuess;
        uiGameScene.PlayerMadeInput -= UpdateFuelLevel;
        uiGameScene.PlayerMadeInput -= UpdateComment;
        uiGameScene.PlayerMadeInput -= UpdateAlreadyPlayed;
        uiGameScene.PlayerMadeInput -= GameLogic;
        uiGameScene.signInButtonClicked -= Connect;
    }

    void Start()
    {
        FindNotYetGuessed();
        ChooseCityToGuess();
        DisplayCityFirstTime();
        AlreadyPlayedUpdate?.Invoke(alreadyPlayed);
        CommentUpdate?.Invoke("ENTER\nA LETTER\nOR THE FULL\nCITY NAME");
        FuelUpdate?.Invoke(remainingFuel);
        UpdateXP?.Invoke(Player.Instance.currentPlayer.experience);
        Debug.Log(Player.Instance.currentPlayer.experience);
    }

    private void FindNotYetGuessed()
    {
        foreach (string city in allCitiesList)
        {
            if (!Player.Instance.currentPlayer.visitedCities.Contains(city.ToUpper()))
            {
                notYetGuessedCities.Add(city);
            }
        }
    }

    private void ChooseCityToGuess()
    {
        if (notYetGuessedCities.Count == 0)
        {
            SceneManager.LoadScene("AllWinScene");
        }
        else
        {
            int cityIndex = 400;

            while (cityIndex >= notYetGuessedCities.Count)
            {
                cityIndex = UnityEngine.Random.Range(0, 3);
            }

            cityToGuess = notYetGuessedCities[cityIndex];
            StartLoadingTextures?.Invoke(cityToGuess);
        }
    }

    private bool CheckInput(string input)
    {
        string pattern = @"^[\p{L}-']+$";
        return Regex.IsMatch(input, pattern);
    }

    private void DisplayCityToGuess(string input)
    {
        if (CheckInput(input))
        {
            cityDisplayVersion = string.Empty;

            if (input.ToUpper() != cityToGuess.ToUpper())
            {
                string inputFirstLetter = input.Substring(0, 1).ToUpper();

                foreach (char letter in cityToGuess.ToUpper())
                {
                    if (letter.ToString() != input.ToUpper() && !alreadyPlayed.Contains(letter.ToString().ToUpper()))
                    {
                        cityDisplayVersion += "_";
                    }
                    else if (letter.ToString() == input.ToUpper() || alreadyPlayed.Contains(letter.ToString().ToUpper()))
                    {
                        cityDisplayVersion += letter.ToString();
                    }
                }
            }
            else
            {
                cityDisplayVersion = cityToGuess.ToUpper();
            }

            ShowCityToGuess?.Invoke(cityDisplayVersion);
        }
        else
        {
            return;
        }
    }

    private void DisplayCityFirstTime()
    {
        cityDisplayVersion = string.Empty;

        foreach (char letter in cityToGuess.ToUpper())
        {
            cityDisplayVersion += "_";
        }
        DisplayMaskedCityFirstTime?.Invoke(cityDisplayVersion);
    }

    private void UpdateAlreadyPlayed(string input)
    {
        if (CheckInput(input))

        {
            if (input.ToUpper() == cityToGuess.ToUpper())
            {
                foreach (char letter in input)
                {
                    if (!alreadyPlayed.Contains(letter.ToString().ToUpper()))
                    {
                        alreadyPlayed.Add(letter.ToString().ToUpper());
                    }
                }
            }
            else
            {
                string inputFirstLetter = input.Substring(0, 1).ToUpper();

                if (!alreadyPlayed.Contains(inputFirstLetter))
                {
                    alreadyPlayed.Add(inputFirstLetter);
                }
            }

            AlreadyPlayedUpdate?.Invoke(alreadyPlayed);
        }
        else
        {
            return;
        }
    }

    private void UpdateFuelLevel(string input)
    {
        if (CheckInput(input))
        {
            string inputFirstLetter = input.Substring(0, 1).ToUpper();

            if (input.ToUpper() != cityToGuess.ToUpper() && !alreadyPlayed.Contains(inputFirstLetter))
            {
                remainingFuel -= 5;
            }

            FuelUpdate?.Invoke(remainingFuel);
        }
        else
        {
            return;
        }
    }

    private void GameLogic(string input)
    {
        if (CheckInput(input))
        {
            if (cityDisplayVersion == cityToGuess.ToUpper() || input.ToUpper() == cityToGuess.ToUpper())
            {
                Player.Instance.AddVisitedCity(cityToGuess.ToUpper());
                StartCoroutine(YouWin());
            }
            else if (cityDisplayVersion != cityToGuess && remainingFuel < 1)
            {
                StartCoroutine(YouLose());
            }
        }
        else
        {
            return;
        }
    }

    private IEnumerator YouLose()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("YouLoseScene");
    }

    private IEnumerator YouWin()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("YouWinScene");
    }

    private void UpdateComment(string input)
    {
        string commentText = string.Empty;

        if (CheckInput(input))
        {
            if (input.ToUpper() == cityToGuess.ToUpper())
            {
                commentText = $"BRAVO! YOU GUESSED THE CITY!";
            }
            else
            {
                inputFirstLetter = input.Substring(0, 1).ToUpper();

                if (alreadyPlayed.Contains(inputFirstLetter))
                {
                    commentText = $"THE LETTER \"{inputFirstLetter}\" HAS ALREADY BEEN PLAYED";
                }
                else
                {
                    if (cityToGuess.ToUpper().Contains(inputFirstLetter))
                    {
                        commentText = $"BRAVO ! YOU GUESSED THE LETTER \"{inputFirstLetter}\"!";
                    }
                    else
                    {
                        commentText = $"WRONG!\nTHERE IS NO \"{inputFirstLetter.ToUpper()}\" IN THIS CITY’S NAME...";
                    }
                }
            }
        }
        else
        {
            commentText = $"INVALID INPUT.\nTYPE A LETTER,\nA HYPHEN,\nOR AN APOSTROPHE";
        }

        CommentUpdate?.Invoke(commentText);
    }

    public void Connect()
    {
        StartCoroutine(LoadConnectScene());
    }

    private IEnumerator LoadConnectScene()
    {
        CloudSave.previousScene = "GameScene";
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("CloudSaveScene");
    }
}
