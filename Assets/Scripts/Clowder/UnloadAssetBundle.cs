using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnloadAssetBundle : MonoBehaviour
{
    public AssetBundle target;
    private void OnDestroy()
    {
        target.Unload(true);
    }
}
