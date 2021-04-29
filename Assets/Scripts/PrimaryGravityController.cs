using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DoctorWolfy121.GravitySystem
{
    public class PrimaryGravityController : MonoBehaviour
    {
        public GravityItem playerCharacter;
        DirectionalGravity mainGravity;
        PlayerController mainControl;
        // Start is called before the first frame update
        void Start()
        {
            mainGravity = GetComponent<DirectionalGravity>();
            mainControl = playerCharacter.GetComponent<PlayerController>();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (playerCharacter.ActiveFieldCount > 1 && mainGravity.enableGravity)
            {
                mainGravity.enableGravity = false;
                if (mainControl.hitHVertical)
                {
                    playerCharacter.GetComponent<PlayerController>().playGravitySound();
                }
                
            }
            else if(playerCharacter.ActiveFieldCount == 1 && !mainGravity.enableGravity)
            {
                mainGravity.enableGravity = true;
                if (mainControl.hitHVertical)
                {
                    playerCharacter.GetComponent<PlayerController>().playGravitySound();
                }
            }
        }
    }
}