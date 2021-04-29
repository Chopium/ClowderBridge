using UnityEngine;
using System.Collections;

public class ScrambleUVs : MonoBehaviour
{
    Vector3[] vertices;
    Mesh mesh;

    public Vector2 uvDistancementAmount = new Vector2(1, 1);

    void Start()
    {
            mesh = GetComponent<MeshFilter>().mesh;
            vertices = mesh.vertices;

        var target = this.GetComponent<Renderer>();

        //Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = mesh.uv;
            for (int i = 0; i < uvs.Length; i++)
            {
                //uvs[q] = new vector2(vertices[q].x, vertices[q].z);

                //uvs[i] = mesh.uv[i];// + new vector2(random.value/1000, random.value / 1000)
                uvs[i] += new Vector2(Random.value * uvDistancementAmount.x, Random.value * uvDistancementAmount.y);
            }
            mesh.uv = uvs;


        target.material.SetTextureScale("_ReflectionTexture", target.material.GetTextureScale("_MainTex"));
        //foreach (var target in mesh.uv)
        //{
        //    target += new Vector2(Random.value, Random.value);
        //}


    }

    
}