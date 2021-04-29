using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class applyVertexColors : MonoBehaviour
{
    Ornament_2020_Interface reference;
    MeshFilter target;
    //Ornament_2020_Interface reference;
    Mesh ornament;
    public void applyTheColors()
    {
        //Debug.Log("applying");
        reference = this.GetComponent<Ornament_2020_Interface>();
        ornament = this.GetComponent<MeshFilter>().mesh;
        //target = this.GetComponent<MeshFilter>();
        //ornament = target.mesh;


        Mesh mesh = ornament;
        Vector3[] vertices = mesh.vertices;

        // create new colors array where the colors will be created.
        Color[] colors = new Color[vertices.Length];


        colors = mesh.colors;

        for (int i = 0; i < vertices.Length; i++)
        {
            if (colors[i].r < 0.25)
            {
                colors[i] = reference.localIdentity.colors[0];
            }
            else if (colors[i].r < 0.75)
            {
                colors[i] = reference.localIdentity.colors[1];
            }
            else
            {
                colors[i] = reference.localIdentity.colors[2];
            }
        }
        //colors[i] = Color.Lerp(Color.red, Color.green, vertices[i].y);

        // assign the array of colors to the Mesh.
        ornament.colors = colors;



        for (int i = 0; i < ornament.vertexCount; i++)
        {
            if (ornament.colors[i].r < 0.25)
            {
                ornament.colors[i] = reference.localIdentity.colors[0];
            }
            else if (ornament.colors[i].r < 0.75)
            {
                ornament.colors[i] = reference.localIdentity.colors[1];
            }
            else
            {
                ornament.colors[i] = reference.localIdentity.colors[2];
            }
            //ornament.vertices[i].colo;
            //ornament.colors[i]
        }
    }
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(5);
        applyTheColors();
    }
    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
