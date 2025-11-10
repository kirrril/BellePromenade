using UnityEngine;
using UnityEngine.SceneManagement;

public class StartAgain : MonoBehaviour
{
    public void DeleteLocalSaveAndStartAgain()
    {
        LocalSave.Instance.InitLocalSave();
        SceneManager.LoadScene("EntryScene");
    }
}
