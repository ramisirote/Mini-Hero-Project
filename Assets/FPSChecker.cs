using UnityEngine;
using UnityEngine.UI;

public class FPSChecker : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private float refreshRate = 0.2f;
    private float deltaTime = 0.0f;
    private float timer;

    // Update is called once per frame
    void Update() {
        if (Time.unscaledTime > timer) {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            text.text = fps + " FPS";
            timer = Time.unscaledTime + refreshRate;
        }
    }
}
