using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionHandling : MonoBehaviour {

    void Start() {
        Application.targetFrameRate = 60;
    }

    public void changeScene(string scene) {
        SceneManager.LoadScene(scene);
    }
}
