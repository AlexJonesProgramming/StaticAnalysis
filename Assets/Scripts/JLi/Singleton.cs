using UnityEngine;

namespace JLi_Utility
{
    /// <summary>
    /// My custom base singleton class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// Determines whether this singleton should persist between scene loads.
        /// </summary>
        protected virtual bool isPersistent => false;
        /*//Currently cannot do this until 'abstract static' interfaces can be used.
        [Tooltip("Determines whether this singleton should be generated when it does not exist already.")]
        protected virtual bool shouldGenerate => false;
        //*/

        private static bool isQuitting = false;

        private bool isInitialized = false;
        public bool IsInitialized { get { return isInitialized; } protected set { isInitialized = value; } }

        private static T instance;
        public static T Instance
        {
            get
            {
                //Prevents errors on OnApplicationQuit
                if (isQuitting)
                    return null;

                if (instance == null)
                {
                    //If instance has not been set, try to find existing instances
                    T[] instances = FindObjectsOfType<T>(true);
                    int count = instances.Length;
                    if (count > 0)
                    {
                        //If exactly one instance is found, set instance and return it
                        if (count == 1)
                            instance = instances[0];
                        else
                            Debugger.LogError($"Somehow there are '{count}' instances of this type ({typeof(T).Name})!");
                    }
                    //else if(!shouldGenerate)
                    else
                    {
                        Debugger.LogError($"Cannot find any singleton instance of '{typeof(T).Name}'. You will need to create this singleton first.");
                        return null;
                    }

                    /*//Currently cannot do this until 'abstract static' interfaces can be used.
                    //If no instance is found, create one
                    else if(shouldGenerate)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                    //*/
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (isPersistent && instance == this)
                DontDestroyOnLoad(Instance);

            OnAwakeCallback();
        }
        protected virtual void OnAwakeCallback() { }

#if UNITY_EDITOR
        private void OnValidate()
        {
            //Debugger.Log($"[{GetInstanceID()}]: {Instance == this}");
            if (Instance != null && Instance != this)
            {
                Debugger.LogError($"Attempted to create singleton of type '{typeof(T).Name}' on gameobject '{name}'. Destroying the newly created one...", gameObject);
                if (!Application.isPlaying)
                    UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(this);
                else
                    UnityEditor.EditorApplication.delayCall += () => Destroy(this);
            }

            OnValidateCallback();
        }
        protected virtual void OnValidateCallback() { }
#endif

        private void OnApplicationQuit()
        {
            isQuitting = true;
            OnApplicationQuitCallback();
        }
        protected virtual void OnApplicationQuitCallback() { }
    }
}