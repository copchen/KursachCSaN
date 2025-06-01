

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour
{
    public Text totalKills, totalScores, totalWavesPassed;

    void Start()
    {
        Load_Statistics();
    }

    public void Load_Statistics()
    {
        totalKills.text = "Total Kills : " + PlayerPrefs.GetInt("Total Kills").ToString();

        totalScores.text = "Total Scores : " + PlayerPrefs.GetInt("Total Scores").ToString();

        totalWavesPassed.text = "Total Waves Passed : " + PlayerPrefs.GetInt("Total Waves Passed").ToString();
    }
}
