using UnityEngine;

public class GyroCamera : MonoBehaviour
{
    void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
        else Debug.LogWarning("Gyroscope not supported");
    }

    void Update()
    {
        if (Input.gyro.enabled)
        {
            transform.rotation = GyroToUnity(Input.gyro.attitude);
        }

    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        Quaternion unityQ = new Quaternion(q.x, q.y, -q.z, -q.w);

        return Quaternion.Euler(90, 0, 0) * unityQ;
    }
}
