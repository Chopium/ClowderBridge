using UnityEngine;
using System.Collections;

public class waterWaves : MonoBehaviour
{
    
    public float deformSpeed = .5f;
    float[] randomTimes;
    Vector3[] vertices;
    Mesh mesh;

    public bool deform = false;
    public Vector2 deformRange = new Vector2(0.1f, 1);//low point and high point along vertical axis

    public bool distortUVs = true;
    public Vector2 uvDistancementAmount = new Vector2(1, 1);

    public bool scrollTexture = true;
    public Vector2 scrollSpeed = Vector2.zero;


    void Start()
    {
        if (deform || distortUVs)
        {
            mesh = GetComponent<MeshFilter>().mesh;
        }

        //assigns each mesh vertex a time offset to animate along
        if (deform)
        {
            randomTimes = new float[mesh.vertices.Length];
            vertices = mesh.vertices;
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                randomTimes[i] = Random.Range(deformRange.x, deformRange.y);
                i++;
            }
        }

        //moves UV points in random directions. used to break up repetitive planar mapping
        if (distortUVs && uvDistancementAmount != Vector2.one)
        {
            Vector2[] uvs = mesh.uv;
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] += new Vector2(Random.value * uvDistancementAmount.x, Random.value * uvDistancementAmount.y);
            }
            mesh.uv = uvs;
        }

        materialTarget = this.GetComponent<Renderer>();
        
        //make sure reflection texture scales with tiling on albedo (for tiling glitch in render pipeline)
        materialTarget.material.SetTextureScale("_ReflectionTexture", materialTarget.material.GetTextureScale("_MainTex"));
    }

    
    Renderer materialTarget;
    public void FixedUpdate()
    {
        if (deform)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].y = 1 * Mathf.PingPong(Time.time * deformSpeed, randomTimes[i]);
                i++;
            }
            mesh.vertices = vertices;
        }

        if (scrollTexture)
        {
            float verticalOffset = Time.time * scrollSpeed.x;
            float horizontalOffset = Time.time * scrollSpeed.y;
            materialTarget.material.SetTextureOffset("_MainTex", new Vector2(horizontalOffset, verticalOffset));
            materialTarget.material.SetTextureOffset("_ReflectionTexture", new Vector2(horizontalOffset *1.1f, verticalOffset *1.1f));
        }
    }

    public void ToggleScroll()
    {
        scrollTexture = !scrollTexture;
    }
}