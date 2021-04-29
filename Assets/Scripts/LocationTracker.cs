using UnityEngine;
using System.Collections;

public class LocationTracker : MonoBehaviour
{
    public GameObject start, current;
    float initialLat, initialLong;
    float currentLat, currentLong;
    float zoomLevel = 20;

    IEnumerator Start()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            // initialLat = Input.location.lastData.latitude;
            // initialLong = Input.location.lastData.longitude;

            // currentLat = Input.location.lastData.latitude;
            // currentLong = Input.location.lastData.longitude;

            initialLat = Input.location.lastData.latitude;
            initialLong = Input.location.lastData.longitude;

            currentLat = Input.location.lastData.latitude;
            currentLong = Input.location.lastData.longitude;

            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();        
    }
    void Update() {
        // currentLat = Input.location.lastData.latitude;
        // currentLong = Input.location.lastData.longitude;

        currentLat += 1;

        current.transform.position = new Vector3( currentLat - initialLat, 0, 0 ) / zoomLevel;
        // current.transform.position = new Vector3( currentLat - initialLat, 0, 0 );
    }
}