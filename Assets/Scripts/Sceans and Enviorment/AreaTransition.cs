using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AreaTransition : MonoBehaviour
{
    // [SerializeField] private SceneAsset sceneToLoad;
    [SerializeField] private bool backwards;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            if (backwards && SceneManager.sceneCountInBuildSettings > SceneManager.GetActiveScene().buildIndex - 1 ) {
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
            }
            else if(SceneManager.sceneCountInBuildSettings > SceneManager.GetActiveScene().buildIndex + 1 ){
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
