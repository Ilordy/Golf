#region Using
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
        private static T _instance;
        [SerializeField] private bool _deactivateOnLoad;
        [SerializeField] private bool _dontDestroyOnLoad;

        private bool _isInitialized;

        public static T I {
            get {
                // Instance required for the first time, we look for it
                if (_instance == null) {
                    var instances = Resources.FindObjectsOfTypeAll<T>();
                    if (instances == null || instances.Length == 0) {
                        return null;
                    }

                    _instance = instances.FirstOrDefault(i => i.gameObject.scene.buildIndex != -1);
                    _instance?.Init();
                }
                return _instance;
            }
        }

        // If no other monobehaviour request the instance in an awake function
        // executing before this one, no need to search the object.
        protected virtual void Awake() {
            if (_instance == null || !_instance || !_instance.gameObject) {
                _instance = (T)this;
            } else if (_instance != this) {
                Debug.LogError($"Another instance of {GetType()} already exist! Destroying self...");
                Destroy(this);
                return;
            }
            _instance.Init();
        }

        /// <summary>
        ///     This function is called when the instance is used the first time
        ///     Put all the initializations you need here, as you would do in Awake
        /// </summary>
        public void Init() {
            if (_isInitialized) {
                return;
            }

            if (_dontDestroyOnLoad) {
                DontDestroyOnLoad(gameObject);
            }

            if (_deactivateOnLoad) {
                gameObject.SetActive(false);
            }

            if (!gameObject) {
                return;
            }

            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            _isInitialized = true;
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene) {
            // Sanity
            if (!I || !gameObject || gameObject == null) {
                SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
                _instance = null;
                return;
            }

            // On scene change, do not nullify instance on the following conditions:
            // 1) Singleton is marked "don't destroy on load"
            if (_dontDestroyOnLoad) {
                return;
            }

            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            _instance = null;
        }

        /// Make sure the instance isn't referenced anymore when the user quit, just in case.
        private void OnApplicationQuit() {
            _instance = null;
        }

        protected virtual void OnDestroy() {
            // Clear static listener OnDestroy
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;

            StopAllCoroutines();
            if (_instance != this) {
                return;
            }
            _instance = null;
            _isInitialized = false;
        }
    }
