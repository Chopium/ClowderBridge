using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globe : MonoBehaviour
{
    float sphereRadius = 5;
    public struct MapPoint {
        public float lat;
        public float lon;
        public MapPoint( float lat, float lon) {
            this.lat = lat;
            this.lon = lon;
        }
    }
    
    public MapPoint testPoint = new MapPoint( 0, 0 );

    [Range(-90, 90)]
    public float testlon = 0;

    [Range(-180, 180)]
    public float testlat = 0;

    GameObject marker;
    List<MapPoint> points;

    // Start is called before the first frame update
    void Start()
    {
        points = new List<MapPoint>();

        points.Add( new MapPoint( 0, 0 ) );
        marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.localScale = Vector3.one * 0.1f;

        
        // foreach( MapPoint p in points ) {
        //     marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        //     // float y = Mathf.Cos( (90- p.lat) * 180 /Mathf.PI  ) * sphereRadius;

        //     float r = sphereRadius;
        //     float x = r*Mathf.Cos(p.lat)*Mathf.Cos(p.lon);
        //     float y = r*Mathf.Cos(p.lat)*Mathf.Sin(p.lon);
        //     float z = r*Mathf.Sin(p.lat);
            
        //     marker.transform.position = new Vector3( x, y, z );
        // }
    }

    // Update is called once per frame
    void Update()
    {
        MapPoint p = new MapPoint( testlon * Mathf.PI / 180, testlat * Mathf.PI / 180);
        float r = sphereRadius;
        float x = r*Mathf.Cos(p.lat)*Mathf.Cos(p.lon);
        float z = r*Mathf.Cos(p.lat)*Mathf.Sin(p.lon);
        float y = r*Mathf.Sin(p.lat);
        
        marker.transform.position = new Vector3( x, y, z );
    }
}
