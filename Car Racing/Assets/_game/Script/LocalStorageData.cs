//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using UnityEngine;


//public class LocalStorageData : MonoBehaviour
//{

//    public static void DeleteKey(string key)
//    {
//        Debug.Log(string.Format("LocalStorageData.DeleteKey(key: {0})", key));

//        #if UNITY_WEBGL
//        RemoveFromLocalStorage(key: key);
//        #else
//        UnityEngine.PlayerPrefs.DeleteKey(key: key);
//        #endif
//    }

//    public static bool HasKey(string key)
//    {
//        Debug.Log(string.Format("LocalStorageData.HasKey(key: {0})", key));

//        #if UNITY_WEBGL
//        return (HasKeyInLocalStorage(key) == 1);
//        #else
//        return (UnityEngine.PlayerPrefs.HasKey(key: key));
//        #endif
//    }

//    public static string GetString(string key)
//    {
//        //Debug.Log(string.Format("LocalStorageData.GetString(key: {0})", key));

//        #if UNITY_WEBGL
//        return LoadFromLocalStorage(key: key);
//        #else
//        return (UnityEngine.PlayerPrefs.GetString(key: key));
//        #endif
//    }

//    public static void SetString(string key, string value)
//    {
//        Debug.Log(string.Format("LocalStorageData.SetString(key: {0}, value: {1})", key, value));

//        #if UNITY_WEBGL
//        SaveToLocalStorage(key: key, value: value);
//        #else
//        UnityEngine.PlayerPrefs.SetString(key: key, value: value);
//        #endif

//    }

//    public static void Save()
//    {
//        Debug.Log(string.Format("LocalStorageData.Save()"));

//        #if !UNITY_WEBGL
//        UnityEngine.PlayerPrefs.Save();
//        #endif
//    }




//    public static void DeleteSessionKey(string key)
//    {
//        Debug.Log(string.Format("SessionStorageData.DeleteKey(key: {0})", key));

//#if UNITY_WEBGL
//        RemoveFromSessionStorage(key: key);
//#else
//        UnityEngine.PlayerPrefs.DeleteKey(key: key);
//#endif
//    }

//    public static bool HasSessionKey(string key)
//    {
//        Debug.Log(string.Format("SessionStorageData.HasKey(key: {0})", key));

//#if UNITY_WEBGL
//        return (HasKeyInSessionStorage(key) == 1);
//#else
//        return (UnityEngine.PlayerPrefs.HasKey(key: key));
//#endif
//    }

//    public static string GetSessionString(string key)
//    {
//        Debug.Log(string.Format("SessionStorageData.GetString(key: {0})", key));

//#if UNITY_WEBGL
//        return LoadFromSessionStorage(key: key);
//#else
//        return (UnityEngine.PlayerPrefs.GetString(key: key));
//#endif
//    }

//    public static void SetSessionString(string key, string value)
//    {
//        Debug.Log(string.Format("SessionStorageData.SetString(key: {0}, value: {1})", key, value));

//#if UNITY_WEBGL
//        SaveToSessionStorage(key: key, value: value);
//#else
//        UnityEngine.PlayerPrefs.SetString(key: key, value: value);
//#endif

//    }

//    public static void SaveSession()
//    {
//        Debug.Log(string.Format("SessionStorageData.Save()"));

//#if !UNITY_WEBGL
//        UnityEngine.PlayerPrefs.Save();
//#endif
//    }





//#if UNITY_WEBGL

//    // Local Storage
//    [DllImport("__Internal")]
//    private static extern void SaveToLocalStorage(string key, string value);

//    [DllImport("__Internal")]
//    private static extern string LoadFromLocalStorage(string key);

//    [DllImport("__Internal")]
//    private static extern void RemoveFromLocalStorage(string key);

//    [DllImport("__Internal")]
//    private static extern int HasKeyInLocalStorage(string key);

//    // Session Storage
//    [DllImport("__Internal")]
//    private static extern void SaveToSessionStorage(string key, string value);

//    [DllImport("__Internal")]
//    private static extern string LoadFromSessionStorage(string key);

//    [DllImport("__Internal")]
//    private static extern void RemoveFromSessionStorage(string key);

//    [DllImport("__Internal")]
//    private static extern int HasKeyInSessionStorage(string key);


//#endif


//}
