using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public float rotationSpeed = 1f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Update()
    {
        rotationY += Input.GetAxis("Mouse X") * rotationSpeed;
        rotationX -= Input.GetAxis("Mouse Y") * rotationSpeed;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}