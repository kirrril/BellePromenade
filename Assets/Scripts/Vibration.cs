using UnityEngine;

public class Vibration : MonoBehaviour
{
// #if UNITY_ANDROID && !UNITY_EDITOR
//     private static AndroidJavaObject unityActivity;
//     private static AndroidJavaObject vibrator;
//     private static bool hasVibrator;
//     private static bool isInitialized;

//     // Initialize on awake to ensure early setup
//     private void Awake()
//     {
//         InitializeAndroid();
//     }

//     // Clean up resources when destroyed
//     private void OnDestroy()
//     {
//         if (unityActivity != null)
//         {
//             unityActivity.Dispose();
//             unityActivity = null;
//         }
//         if (vibrator != null)
//         {
//             vibrator.Dispose();
//             vibrator = null;
//         }
//         isInitialized = false;
//     }

//     private static void InitializeAndroid()
//     {
//         if (isInitialized) return;

//         try
//         {
//             using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//             {
//                 unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
//                 if (unityActivity != null)
//                 {
//                     vibrator = unityActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
//                     hasVibrator = vibrator.Call<bool>("hasVibrator");
//                     isInitialized = true;
//                 }
//             }
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError("Failed to initialize Android vibrator: " + e.Message);
//         }
//     }

//     public static void Vibrate(long milliseconds = 1200)
//     {
//         if (!isInitialized)
//         {
//             InitializeAndroid();
//         }

//         if (vibrator == null || !hasVibrator)
//         {
//             Debug.LogWarning("Vibration not available on this device.");
//             return;
//         }

//         try
//         {
//             using (AndroidJavaClass versionClass = new AndroidJavaClass("android.os.Build$VERSION"))
//             {
//                 int sdkInt = versionClass.GetStatic<int>("SDK_INT");

//                 if (sdkInt >= 26) // Android 8.0 (API 26) and above
//                 {
//                     using (AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect"))
//                     {
//                         AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
//                             "createOneShot", milliseconds, vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE"));
//                         vibrator.Call("vibrate", effect);
//                     }
//                 }
//                 else
//                 {
//                     vibrator.Call("vibrate", milliseconds);
//                 }
//             }
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError("Vibration failed: " + e.Message);
//         }
//     }

//     // Check if device has vibrator
//     public static bool HasVibrator()
//     {
//         if (!isInitialized)
//         {
//             InitializeAndroid();
//         }
//         return hasVibrator;
//     }

// #else
//     public static void Vibrate(long milliseconds = 1200)
//     {
//         Debug.Log("Vibration simulated: " + milliseconds + "ms");
//     }

//     public static bool HasVibrator()
//     {
//         Debug.Log("Vibration check simulated: returning false for non-Android platforms.");
//         return false;
//     }
// #endif
}