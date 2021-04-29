using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine
{
    public class cineOverride : MonoBehaviour
    {
        public void setBodyOffset()
        {
            this.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, 2, 2f);
        }
    }
}