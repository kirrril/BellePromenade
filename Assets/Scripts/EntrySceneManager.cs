using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EntrySceneManager : MonoBehaviour
{
    [SerializeField]
    private UIentryScene uiEntryScene;

    void OnEnable()
    {
        uiEntryScene.startButtonClick += Play;
        uiEntryScene.signInButtonClick += Connect;
    }

    void OnDisable()
    {
        uiEntryScene.startButtonClick -= Play;
        uiEntryScene.signInButtonClick -= Connect;
    }

    public void Play()
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
        CloudSave.previousScene = "EntryScene";
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("CloudSaveScene");
    }
}
