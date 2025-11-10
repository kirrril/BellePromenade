using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class YouLoseSceneManager : MonoBehaviour
{
    [SerializeField]
    private UIyouLoseScene uiYouLoseScene;

    [SerializeField]
    private Material sphereMaterial;

    void OnEnable()
    {
        uiYouLoseScene.startButtonClicked += PlayAgain;
        uiYouLoseScene.signInButtonClicked += Connect;
    }

    void OnDisable()
    {
        uiYouLoseScene.startButtonClicked -= PlayAgain;
        uiYouLoseScene.signInButtonClicked -= Connect;
    }

    void Start()
    {
        sphereMaterial.mainTexture = TextureLoader.Instance.nowhereTexture;
    }

    public void PlayAgain()
    {
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("GameScene");
    }

    public void Connect()
    {
        StartCoroutine(LoadConnectScene());
    }

    private IEnumerator LoadConnectScene()
    {
        CloudSave.previousScene = "YouLoseScene";
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("CloudSaveScene");
    }
}