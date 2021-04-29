using UnityEngine;

public class scrollTexture : MonoBehaviour
{
    // Scroll main texture based on time

    public float scrollSpeed = 0.5f;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        rend.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(-offset, 0));
    }
}