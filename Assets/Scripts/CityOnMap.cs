using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CityOnMap : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    private GyroCamera gyroCamera;

    private GameObject backToMapCanvas;

    [SerializeField]
    private Button cityButton;
    [SerializeField]
    private Material memoryMaterial;
    [SerializeField]
    private Material cityMaterial;
    private Color baseColor;
    public static bool isOnMap;
    float flickeringFrequency;

    [SerializeField]
    private Transform initialCameraPosition;

    void Awake()
    {
        gyroCamera = transform.GetComponent<GyroCamera>();
        backToMapCanvas = GameObject.Find("BackToMapCanvas");
    }

    void OnEnable()
    {
        TextureLoader.Instance.textureIsLoaded += ApplyCityTexture;
    }

    void OnDisable()
    {
        TextureLoader.Instance.textureIsLoaded -= ApplyCityTexture;
    }

    void Start()
    {

        backToMapCanvas.SetActive(false);
        cityMaterial = GetComponent<Renderer>().material;
        flickeringFrequency = Random.Range(0.005f, 0.5f);
        baseColor = cityMaterial.color;
        isOnMap = true;
    }

    void LateUpdate()
    {
        if (isOnMap)
        {
            Quaternion camRot = mainCamera.transform.rotation;

            Vector3 euler = camRot.eulerAngles;
            euler.z = 0f;
            Quaternion noRoll = Quaternion.Euler(euler);

            Vector3 lookPos = transform.position + noRoll * Vector3.forward;
            Vector3 up = noRoll * Vector3.up;
            transform.LookAt(lookPos, up);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }

        float t = Mathf.PingPong(Time.time * flickeringFrequency, 1f);
        float intensity = Mathf.Lerp(0.5f, 1.1f, t);
        cityMaterial.color = baseColor * intensity;
    }

    public void MoveToCity(string cityName)
    {
        StartCoroutine(MovingToCity());
        TextureLoader.Instance.LoadTextures(cityName);
    }

    private IEnumerator MovingToCity()
    {
        Vector3 startPos = mainCamera.transform.position;
        Vector3 targetPos = gameObject.transform.position;

        float elapsed = 0f;
        float duration = 1.5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }
        mainCamera.transform.position = targetPos;
        isOnMap = false;
        backToMapCanvas.SetActive(true);
    }

    // public void MoveToMap()
    // {
    //     StartCoroutine(MovingToMap());
    //     backToMapCanvas.SetActive(false);
    // }

    // private IEnumerator MovingToMap()
    // {
    //     Quaternion currentRotation = mainCamera.transform.rotation;
    //     float elapsed = 0f;
    //     float duration = 1.5f;

    //     while (elapsed < duration)
    //     {
    //         elapsed += Time.deltaTime;
    //         float t = elapsed / duration;

    //         mainCamera.transform.position = Vector3.Lerp(gameObject.transform.position, initialCameraPosition.position, t);
    //         mainCamera.transform.rotation = Quaternion.Slerp(currentRotation, initialCameraPosition.rotation, t);

    //         yield return null;
    //     }
    //     mainCamera.transform.position = initialCameraPosition.position;
    //     mainCamera.transform.rotation = initialCameraPosition.rotation;
    //     isOnMap = true;
    // }

    private void ApplyCityTexture(Texture2D texture)
    {
        memoryMaterial.mainTexture = texture;
    }
}
