using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TextureLoader : MonoBehaviour
{
    private string apiKey;
    public Texture2D cityTexture;
    public Texture2D nowhereTexture;
    private string envFilePath;
    public static TextureLoader Instance;

    public event Action<Texture2D> textureIsLoaded;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        envFilePath = Path.Combine(Application.dataPath, "../.env");

        if (!File.Exists(envFilePath))
        {
            Debug.LogError("Fichier .env introuvable : " + envFilePath);
            return;
        }

        apiKey = GetEnvValue("GOOGLE_API_KEY");

        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("GOOGLE_API_KEY introuvable dans .env !");
        }
        else
        {
            Debug.Log("Clé API chargée depuis .env ");
        }
    }

    void OnEnable()
    {
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.StartLoadingTextures += LoadTextures;
        }
    }

    void OnDisable()
    {
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.StartLoadingTextures -= LoadTextures;
        }
    }

    private string GetEnvValue(string key)
    {
        foreach (var line in File.ReadAllLines(envFilePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            var parts = line.Split('=', 2);
            if (parts.Length == 2 && parts[0].Trim() == key)
                return parts[1].Trim();
        }
        return null;
    }

    public void LoadTextures(string cityName)
    {
        StartCoroutine(LoadCityTextureCoroutine(cityName));
    }

    private IEnumerator LoadCityTextureCoroutine(string city)
    {
        string cityUrl = $"https://storage.googleapis.com/bucket_belle_promenade/{city}.jpg";
        string nowhereUrl = $"https://storage.googleapis.com/bucket_belle_promenade/{city}_n.jpg";

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(cityUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                cityTexture = DownloadHandlerTexture.GetContent(request);
                textureIsLoaded?.Invoke(cityTexture);
            }
            else Debug.Log($"Requête de la texture pour {city} a échoué");
        }

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(nowhereUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                nowhereTexture = DownloadHandlerTexture.GetContent(request);
            }
            else Debug.Log($"Requête de la texture pour {city}_n a échoué");
        }
    }
}