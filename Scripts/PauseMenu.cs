
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
   
    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

  
    public void StartPause()
    {
        Time.timeScale = 0;
    }

    
    public void EndPause()
    {
        Time.timeScale = 1f;
    }

    
    public void Enable_Object(GameObject target)
    {
        target.SetActive(true);
    }

    
    public void Disable_Object(GameObject target)
    {
        target.SetActive(false);
    }
    
   
    public void Toggle_Object(GameObject target)   
    {
        target.SetActive(!target.activeSelf);
    }

    bool doubleSpeed = false;
    public void Double_Speed_Mode()
    {
        doubleSpeed = !doubleSpeed;
        if (doubleSpeed)
            Time.timeScale = 2f;
        else
            Time.timeScale = 1f;
    }
}
