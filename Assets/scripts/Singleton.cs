using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    static private T _instance;
    static public T Instance {
        get {
            if (_instance == null) {
                // Check if there is more then one instance of the singleton.
                var instances = FindObjectsOfType<T>();
                if (instances.Length > 1) {
                    Debug.LogWarningFormat("[{0}]: More then one instance of " + 
                        "singleton.", typeof(T).Name);
                }

                // Get the first instance of the singleton in the scene.
                _instance = FindObjectOfType<T>();

                // If not instance exists create one.
                //if (_instance == null) {
                //    GameObject singletonGO = new GameObject();
                //    singletonGO.name = "(Singleton)" + typeof(T).ToString();
                //    _instance = singletonGO.AddComponent<T>();
                //    Debug.LogWarningFormat("[{0}]: No instance of singleton found "
                //        + "in scene. Creating instance", typeof(T).Name);
                //}

                if (_instance == null) {
                    Debug.LogWarningFormat("[{0}] No instance of the singleton found in scene.", typeof(T).Name);
                }
            }
            return _instance;
        }
    }
}
