using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class YouWinSceneManager : MonoBehaviour
{
    [SerializeField]
    private UIyouWinScene uIyouWinScene;

    [SerializeField]
    private Material sphereMaterial;

    void Awake()
    {
        uIyouWinScene.startButtonClicked += PlayAgain;
        uIyouWinScene.signInButtonClicked += Connect;
    }

    void Start()
    {
        sphereMaterial.mainTexture = TextureLoader.Instance.cityTexture;
    }

    public void PlayAgain()
    {
        StartCoroutine(LoadGameScene());
    }

    void OnEnable()
    {
        uIyouWinScene.startButtonClicked += PlayAgain;
        uIyouWinScene.signInButtonClicked += Connect;
    }

    void OnDisable()
    {
        uIyouWinScene.startButtonClicked -= PlayAgain;
        uIyouWinScene.signInButtonClicked -= Connect;
    }

    private IEnumerator LoadGameScene()
    {
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("GameScene");
        // if (Player.Instance.currentPlayer.visitedCities.Count < 26)
        // {
        //     SceneManager.LoadScene("GameScene");
        // }
        // else
        // {
        //     SceneManager.LoadScene("AllWinScene");
        // }
    }

        public void Connect()
    {
        StartCoroutine(LoadConnectScene());
    }

    private IEnumerator LoadConnectScene()
    {
        CloudSave.previousScene = "YouWinScene";
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("CloudSaveScene");
    }
}
