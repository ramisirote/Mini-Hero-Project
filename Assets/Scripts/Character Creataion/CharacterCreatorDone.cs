using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCreatorDone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Done(){
        powerCustimizer.instance.SetPowerManagerPowers();
        PlayGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
