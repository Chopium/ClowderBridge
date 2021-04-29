using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulatePlayerStats : MonoBehaviour
{
    public Button Exit;
    public GameObject Time;
    public GameObject Cheats;
    public GameObject Total;
    public GameObject Rank;

    // Start is called before the first frame update
    void OnEnable()
    {
        //Time.name = "RAW TIME: "+ TimeSpan.FromSeconds(round(GameManager.Instance.runtime)).ToString(@"hh\:mm\:ss");//.ToString("mm:ss:ms")
        //Cheats.name = "CHEATS: " + GameManager.Instance.cheatLevel+" x "+TimeSpan.FromSeconds(round(GameManager.Instance.cheatPenalty)).ToString(@"hh\:mm\:ss");
        //Total.name = "TOTAL: "+TimeSpan.FromSeconds(round(GameManager.Instance.getTotal())).ToString(@"hh\:mm\:ss");
        //Rank.name = GameManager.Instance.getRank();

    }

    float round(float var)
    {
        // 37.66666 * 100 =3766.66 
        // 3766.66 + .5 =3767.16    for rounding off value 
        // then type cast to int so value is 3767 
        // then divided by 100 so the value converted into 37.67 
        float value = (int)(var * 100 + .5);
        return (float)value / 100;
    }
}
