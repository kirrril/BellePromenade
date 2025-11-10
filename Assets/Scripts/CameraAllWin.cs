using System.Collections;
using UnityEngine;

public class CameraAllWin : MonoBehaviour
{
    private GyroCamera gyroCamera;

    [SerializeField]
    private GameObject backToMapCanvas;

    [SerializeField]
    private Transform initialCameraPosition;

    [SerializeField]
    private Transform zoomInCameraPosition;

    void Awake()
    {
        gyroCamera = transform.GetComponent<GyroCamera>();
    }

    void Start()
    {
        gyroCamera.enabled = false;
        transform.position = zoomInCameraPosition.position;
        // transform.rotation = zoomInCameraPosition.rotation;

        StartCoroutine(CameraZoomOut());
    }

    private IEnumerator CameraZoomOut()
    {
        float elapsed = 0f;
        float duration = 1.5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(zoomInCameraPosition.position, initialCameraPosition.position, t);
            // transform.rotation = Quaternion.Slerp(zoomInCameraPosition.rotation, initialCameraPosition.rotation, t);

            yield return null;
        }
        transform.position = initialCameraPosition.position;
        // transform.rotation = initialCameraPosition.rotation;
        gyroCamera.enabled = true;
    }

    public void MoveToMap()
    {
        StartCoroutine(MovingToMap());
        backToMapCanvas.SetActive(false);
    }

    private IEnumerator MovingToMap()
    {
        // Quaternion currentRotation = transform.rotation;
        float elapsed = 0f;
        float duration = 1.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(gameObject.transform.position, initialCameraPosition.position, t);
            // transform.rotation = Quaternion.Slerp(currentRotation, initialCameraPosition.rotation, t);

            yield return null;
        }
        transform.position = initialCameraPosition.position;
        // transform.rotation = initialCameraPosition.rotation;
        CityOnMap.isOnMap = true;
    }
}