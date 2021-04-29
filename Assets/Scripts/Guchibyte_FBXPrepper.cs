using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Guchibyte_FBXPrepper : MonoBehaviour
{
    string fileExtension;
    //JSONObject fileMetaData;
    public void Awake()
    {
        //Debug.Log("Cleanup");

        //getting file extension

        //fileMetaData = this.GetComponentInParent<JsonRef>().MetaJson;
        //print(fileMetaData.str);
        

        UpdateMaterials();

        resetTransform();
        fileExtension = Path.GetExtension(this.GetComponentInParent<JsonRef>().gameObject.name);
        if (fileExtension.Equals(".stl"))//this may cause issues if we override R3D transform information
        {
            transform.localScale = transform.localScale / 1000;//mm to meters
            transform.localRotation *= Quaternion.Euler(new Vector3(90, 0, 0));//rotate y to z
        }

        foreach (MeshRenderer m in this.GetComponentsInChildren<MeshRenderer>())
        {
            m.gameObject.AddComponent<MeshCollider>();
        }
        //Wrapper.AddComponent<MeshCollider>();

        //disable imported cameras
        object[] importedCameras = this.GetComponentsInChildren<Camera>();
        foreach (Camera o in importedCameras)
        {
            //Debug.Log(o.gameObject);
            o.gameObject.SetActive(false);
        }
        //List<GameObject> children = this.gameObject.get
    }

    //ensures our object loads in with same orientation to "offset" container. probably could be an optional call based on the type of content. we could have different kinds of transform override per object
    void resetTransform()
    {
        
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
        //transform.localScale = Vector3.one;
    }
    void UpdateMaterials()
    {
        Renderer[] children;
        children = GetComponentsInChildren<Renderer>();

        //create a list of unique materials
        List<Material> allMaterials = new List<Material>();
        foreach (Renderer rend in children)
        {
            //Debug.Log(rend.materials.ToString());
            //Material[] mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.sharedMaterials.Length; j++)
            {
                //Debug.Log("loop1 " + rend.sharedMaterials[j].name);

                if (!allMaterials.Contains(rend.sharedMaterials[j]))
                {
                    //Debug.Log("added " + rend.sharedMaterials[j].name);
                    allMaterials.Add(rend.sharedMaterials[j]);
                }

            }
        }

        //read in name of each material, which can include shader and parameter overrides
        foreach (Material m in allMaterials)
        {
            //string[] arguments = m.name.Substring
            List<string> array = new List<string>(m.name.Split(' ')); //split via spaces
            foreach (string s in array)
            {
                if (StringExtension.CustomStartsWith(s, "[") && StringExtension.CustomEndsWith(s, "]"))//material
                {
                    //Debug.Log("got specific material: "+ s.Substring(1, s.Length - 2));
                    m.shader = Shader.Find(s.Substring(1, s.Length - 2));
                }
                if (StringExtension.CustomStartsWith(s, "(") && StringExtension.CustomEndsWith(s, ")"))//property
                {
                    //Debug.Log("got material parameter override: "+ s.Substring(1, s.Length - 2));
                    string[] args = s.Substring(1, s.Length - 2).Split(',');//split into arguments (SetInt,_StencilMask,2)


                    if (args.Length == 3 && args[0].Contains("SetInt"))
                    {
                        int parameter = 0;
                        int.TryParse(args[2], out parameter);
                        m.SetInt(args[1], parameter);
                    }

                }

            }

            // name [stencil/write] (setInt,_stencilmask,2)

            //STEP ONE - do material overrides
            //hidden 
            if (m.name.Contains("hidden_unlit"))
            {
                m.shader = Shader.Find("Stencil/Read-Unlit");
                m.SetInt("_StencilMask", 2);
            }

            //portal texture
            if (m.name.Contains("[Portal]"))
            {
                m.shader = Shader.Find("Stencil/Write");
                m.SetInt("_StencilMask", 2);
            }
            if (m.name.Contains("[unlit]"))
            {
                m.shader = Shader.Find("Unlit/Texture");
                //m.SetInt("_StencilMask", 2);
            }

            //hidden lit
            if (m.name.Contains("[hidden_lit]"))
            {
                m.shader = Shader.Find("Stencil/Read");
                m.SetInt("_StencilMask", 2);
            }

            if (m.name.Contains("hidden_unlit"))
            {
                m.shader = Shader.Find("Stencil/Read-Unlit");
                m.SetInt("_StencilMask", 2);
            }


            //STEP TWO - check layers and check w/material, create instances to match
            //Debug.Log(m.shader.ToString());
            if (m.shader == Shader.Find("Standard"))
            {
                //Debug.Log(m.GetColor("_Color"));
                if(m.GetColor("_Color").a < 1)
                {
                    //Debug.Log(m.GetFloat("_Mode"));
                    //m.SetFloat("_Mode", 3);
                    StandardShaderUtils.ChangeRenderMode(m, StandardShaderUtils.BlendMode.Transparent);
                }
            }
        }
    }
   
}
public static class StringExtension
{
    public static bool CustomEndsWith(this string a, string b)
    {
        int ap = a.Length - 1;
        int bp = b.Length - 1;

        while (ap >= 0 && bp >= 0 && a[ap] == b[bp])
        {
            ap--;
            bp--;
        }

        return (bp < 0);
    }

    public static bool CustomStartsWith(this string a, string b)
    {
        int aLen = a.Length;
        int bLen = b.Length;

        int ap = 0; int bp = 0;

        while (ap < aLen && bp < bLen && a[ap] == b[bp])
        {
            ap++;
            bp++;
        }

        return (bp == bLen);
    }
}

public static class StandardShaderUtils
{
    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 2450;
                break;
            case BlendMode.Fade:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
            case BlendMode.Transparent:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
        }

    }
}