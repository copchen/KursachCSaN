

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Space(7)]
    [Header("Play the game and press -H- key to delete game's save data")]
    
    public int startingCoins = 1000;

    
    public int minimumCoins = 1000;

   
    public Text totalCoinsText;

   
    public Text totalScoresText;

   
    void Start()
    {
       
        if (PlayerPrefs.GetInt("FirstRun") != 1) 
        {

           
            PlayerPrefs.SetInt("Total Coins", startingCoins); 

           
            PlayerPrefs.SetInt("Minimum Coins", minimumCoins); 

           
            PlayerPrefs.SetInt("Level Unlocked0", 1); 

            
            PlayerPrefs.SetInt("FirstRun", 1);

           
            PlayerPrefs.SetInt("Music", 1);
        }

       
        if (PlayerPrefs.GetInt("Total Coins") < PlayerPrefs.GetInt("Minimum Coins"))
            PlayerPrefs.SetInt("Total Coins", PlayerPrefs.GetInt("Minimum Coins"));

       
        totalCoinsText.text = PlayerPrefs.GetInt("Total Coins").ToString();

       
        totalScoresText.text = PlayerPrefs.GetInt("Total Scores").ToString();

    }
   
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Game's saved data deleted successfully");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
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
}
