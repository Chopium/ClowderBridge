using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetStartedMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int hasPlayed = PlayerPrefs.GetInt( "HasPlayed");
 
        if( hasPlayed == 0 )
        {
        PlayerPrefs.SetInt( "HasPlayed", 1 );
        }
        else
        {
        // Not First Time
            gameObject.SetActive(false);
        }
    }

    public void GetStartedButton() {
        gameObject.SetActive(false);
    }
}
