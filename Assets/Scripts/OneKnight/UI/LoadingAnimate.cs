using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OneKnight.UI {

    public class LoadingAnimate : MonoBehaviour {

        public string scenePath;
        public Image loadBar;


        void Start() {
            // Press the space key to start coroutine
            // Use a coroutine to load the Scene in the background
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenePath);
            StartCoroutine(LoadYourAsyncScene(asyncLoad));
        }

        IEnumerator LoadYourAsyncScene(AsyncOperation loader) {

            // Wait until the asynchronous scene fully loads
            while(!loader.isDone) {
                loadBar.fillAmount = (loader.progress);
                yield return null;
            }
        }
    }
}
