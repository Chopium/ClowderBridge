using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using System;
//using TriLib;

public class ConsoleInitializer : MonoBehaviour
{
    //string[] LevelNames;
    //string MapListString;

    // private string ClowderAPIUrl = "https://cyprus.ncsa.illinois.edu/clowder/api/";

    void Start()
    {
        var repo = ConsoleCommandsRepository.Instance;
        repo.RegisterCommand("clearPerfs", clearPerfs);
        //#region Basic Commands
        ////repo.RegisterCommand("help", Help);
        //repo.RegisterCommand("map", Map);
        ////repo.RegisterCommand("maplist", Maplist);

        //repo.RegisterCommand("startrecord", StartRecording);
        //repo.RegisterCommand("stoprecord", StopRecording);

        //repo.RegisterCommand("startdemo", StartDemo);
        //repo.RegisterCommand("stopdemo", StopDemo);

        //repo.RegisterCommand("reload", ReloadLevel);
        //repo.RegisterCommand("clear", Clear);
        //repo.RegisterCommand("exit", Exit);

        //repo.RegisterCommand("reset", ResetPosition);
        //repo.RegisterCommand("quality", SetQuality);

        //repo.RegisterCommand("trace", trace);
        //repo.RegisterCommand("collapse", collapse);
        /*
        repo.RegisterCommand("reset2", ResetVive);
        repo.RegisterCommand("sun_push", PushSun);
        repo.RegisterCommand("sun_speed", setSunSpeed);
        repo.RegisterCommand("sun_angle", setSunAngle);*/
        //#endregion
        //#region Graphics commands
        //repo.RegisterCommand("cl_drawtrees", cl_drawtrees);
        //repo.RegisterCommand("cl_lod", cl_lod);
        //repo.RegisterCommand("cl_farclipplane", cl_camerafarplane);
        //repo.RegisterCommand("cl_drawshadows", cl_drawshadows);
        //#endregion
        //#region R3D Commands
        //repo.RegisterCommand("plr_scale", ScalePlayer);
        //repo.RegisterCommand("plr_pos", MovePlayer);
        //repo.RegisterCommand("plr_rot", SetPlayerRotation);
        //repo.RegisterCommand("plr_turn", RotatePlayer);

        //repo.RegisterCommand("obj_return", ReturnObject);

        //repo.RegisterCommand("setscenerio", setScenerio);
        //repo.RegisterCommand("disableScenerio", disableScenerio);
        //repo.RegisterCommand("enablescenerio", enableScenerio);
        //#endregion
        //repo.RegisterCommand("assetDelay", assetDelay);
        //repo.RegisterCommand("search", ClowderSearch);
        //repo.RegisterCommand("searchmetadata", ClowderSearchMetadata);
        //repo.RegisterCommand("createdataset", ClowderCreateDataset);
        //repo.RegisterCommand("serialize", SerializeFields);
        //repo.RegisterCommand("url", parseURL);
        //repo.RegisterCommand("drawgui", DrawGUI);

        //repo.RegisterCommand("postmeta", postMetadata);

        PlayerPrefs.SetString("date_time", System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
        //Console.Instance.LogText("RIVEEL3D   -   " + PlayerPrefs.GetString("date_time") + "   -   Type help to get a list of commands.", LogType.Log);
    }
    #region Older Commands
    public string ScalePlayer(params string[] args)
    {
        float GoalScale = 1f;
        float.TryParse(args[0], out GoalScale);
        //Global.PlayerScale = GoalScale;
        return "Scaled Player to " + GoalScale;
    }
    public string EnvPointer_restart(params string[] args)
    {
        //EnvironmentPointerManager.instance.initializeParenting();
        return "attempting..";
    }
    public string MovePlayer(params string[] args)
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;
        float.TryParse(args[0], out x);
        float.TryParse(args[1], out y);
        float.TryParse(args[2], out z);

        //PlayerInterface.instance.teleportPlayer(new Vector3(x, y, z), 0, 0);
        return "Moved player to (" + x + ", " + y + ", " + z + ")";
    }
    public string SetPlayerRotation(params string[] args)
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;
        float.TryParse(args[0], out x);
        float.TryParse(args[1], out y);
        float.TryParse(args[2], out z);

        //PlayerInterface.PlayerCharacter.transform.localEulerAngles = new Vector3(x, y, z);
        return "Rotated player to (" + x + ", " + y + ", " + z + ")";
    }
    public string RotatePlayer(params string[] args)
    {
        float degrees = 0f;

        float.TryParse(args[0], out degrees);

        //PlayerInterface.PlayerCharacter.transform.Rotate(Vector3.up * degrees);
        return "Rotated player ( " + degrees + " degrees)";
    }
    public string cl_lod(params string[] args)
    {
        float oldLOD = QualitySettings.lodBias;
        float newLOD = 0f;
        float.TryParse(args[0], out newLOD);
        if (newLOD != oldLOD && newLOD > 0f && newLOD < 10f)
        { QualitySettings.lodBias = newLOD; }
        return "Lod setting is now at " + QualitySettings.lodBias;
    }
    public string cl_camerafarplane(params string[] args)
    {
        float oldLOD = Camera.main.farClipPlane;
        float newLOD = 1000f;
        float.TryParse(args[0], out newLOD);
        if (newLOD != oldLOD && newLOD > 0f && newLOD < 100000f)
        {
            Camera.main.farClipPlane = newLOD;
            //atmospheric effects should be adjusted here, or we could take care of it through rendering changes
        }
        return "Camera Far Clip Plane at :  " + Camera.main.farClipPlane;
    }
    public string cl_drawshadows(params string[] args)
    {
        bool enableshadows = false;
        bool.TryParse(args[0], out enableshadows);
        QualitySettings.shadows = enableshadows ? ShadowQuality.All : ShadowQuality.Disable;

        return "shadows :  " + enableshadows;
    }
    public string trace(params string[] args)
    {
        bool input = false;
        bool.TryParse(args[0], out input);
        Console.Instance.doStackTrace = input;

        return "stack trace :  " + input;
    }
    public string collapse(params string[] args)
    {
        bool input = false;
        bool.TryParse(args[0], out input);
        Console.Instance.setCollase(input);

        return "collapse :  " + input;
    }
    public string DrawGUI(params string[] args)
    {
        bool enableGUI = true;
        bool.TryParse(args[0], out enableGUI); //(Light[]) GameObject.FindObjectsOfType (typeof(Light));
        //QualitySettings.shadows = enableGUI ? ShadowQuality.All : ShadowQuality.Disable;
        Canvas[] targets = (Canvas[])Resources.FindObjectsOfTypeAll(typeof(Canvas));
        foreach (Canvas t in targets)
        {
            t.gameObject.SetActive(enableGUI);
        }

        return "GUI Draw :  " + enableGUI;
    }
    public string cl_drawtrees(params string[] args) //0 to false, 1 to true
    {
        bool enabletrees = false;
        bool.TryParse(args[0], out enabletrees);
        GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag("Trees");

        foreach (GameObject go in gameObjectArray)
        {
            foreach (Transform child in go.transform)
            {
                child.gameObject.SetActive(enabletrees);
            }
        }
        return "Trees set to : " + enabletrees;
    }
    public string Help(params string[] args)
    {
        return
            "map [mapname] -- loads the map - either by name or by the build index number\n" + "maplist --  returns a list of available maps\n" +

            "cl_lod 0 -- change the number for quality render and distance cull setting 2 or 3 recommended\n" +
            "cl_drawtrees true -- true or false. disable trees to improve rendering speed if they have been identified in the scene\n" +
            "cl_drawshadows true -- true or false. disable shadows to improve rendering speed\n" +
            "cl_camerafarplane 1000 -- the number sets the distance the camera renders in meters. low numbers improve performance\n" +
            "reload --  reloads the current level\n" + "reset -- resets player to starting position and scale\n"
            + "sun_push [degrees] -- pushes sun forward along path by input amount \n" + "sun_speed -- sets sun's speed in degrees per second\n" + "clear -- clear gui\n"
            + "startrecord name frequency -- enter name for session and recording frequency in seconds (0.5 recommended) e.g. startrecord colter 0.5\n" + "stoprecord -- stop recording output\n";
    }
    public string Clear(params string[] args)
    {
        if (args.Length > 0)
        {
            if (args[0].Contains("scene"))
            {
                ClowderBridge.Instance.ClearScene();
            }
            if (args[0].Contains("resources"))
            {
                Resources.UnloadUnusedAssets();
            }
            if (args[0].Contains("log"))
            {
                Console.Instance.ClearLog();
            }
        }
        else
        {
            Console.Instance.ClearLog();
        }
        return ("");
    }
    public string SetQuality(params string[] args)
    {

        string output = "";
        int input = QualitySettings.GetQualityLevel();
        if (args.Length > 0)
        {
            if (args[0].Equals("list"))
            {
                for (int i = 0; i < QualitySettings.names.Length; i++)
                {
                    output += "\n" + QualitySettings.names[i] + " [" + i + "]";
                }
            }
            else
            {
                int.TryParse(args[0], out input);
                if (input != QualitySettings.GetQualityLevel())
                {
                    QualitySettings.SetQualityLevel(input);
                }
            }
            //Debug.Log(QualitySettings.names[input]);
        }
        output += "\nCurrent Quality Setting: " + QualitySettings.names[input] + " [" + input + "]";
        return (output);
    }
    public string Map(params string[] args)
    {
        //check if map name is valid
        var mapname = args[0];
        int mapNumber;
        if (int.TryParse(mapname, out mapNumber))
        {
            SceneManager.LoadSceneAsync(mapNumber);
            return ("Attempting to Load Map");
            /*
            if (mapNumber > 0 )
            {
                SceneManager.LoadSceneAsync(mapNumber);
                return ("Attempting to Load Map");
            }
            else
            {
                return ("Cannot go to map zero");
            }*/
        }
        //Debug.Log(SceneManager.GetSceneByName(mapname).buildIndex);
        if (SceneManager.GetSceneByName(mapname).buildIndex < 1)
        {
            return ("Cannot got to map zero");
        }
        else
            SceneManager.LoadSceneAsync(mapname);
        return ("Attempting to Load Map...");
    }
    public string ReturnObject(params string[] args)
    {
        //Debug.Log("Returning Object");
        //PlayerInterface.instance.ReturnInteractiveObject();
        return ("Object Returned!");
    }
    public string ResetPosition(params string[] args)
    {/*
        //PlayerInterface.instance.ResetPlayer();
        var target = GameObject.FindObjectsOfType<R3D_SpawnPoint_Test01>() as R3D_SpawnPoint_Test01[];
        foreach(R3D_SpawnPoint_Test01 instance in target)
        {
            if(instance.MainSpawnpoint)
            {
                instance.SpawnHere();
            }
        }*/
        return ("Player Reset!");
    }
    public string ReloadLevel(params string[] args)
    {
        //check if map name is valid
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        //return ("Attempting to Reload Map...");
        /*
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            Destroy(o);
        }*/
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        return ("Attempting to Reload Map...");
    }
    public string postMetadata(params string[] args)
    {
        if (args.Length > 0)
        {
            ClowderBridge.Instance.PostAllMetadata();
            return ("unused argument: " + args[0]);

        }
        else
        {
            ClowderBridge.Instance.PostAllMetadata();
            return ("posting...");
        }
    }
    public string assetDelay(params string[] args)
    {
        if (args[1] != null)
        {
            float.TryParse(args[1], out ClowderBridge.Instance.AssetWaitTime);
        }

        return "Asset Delay Wait Time Set To :  " + args[1];
    }
    //  public string Maplist(params string[] args)
    //{
    //	if(LevelNames == null){
    //		LevelNames = Assets.Scenes.Scripts.Game.Levels;
    //		MapListString = null;
    //		for (int i = 0; i < (LevelNames.Length); i++)
    //		{
    //		MapListString += (LevelNames[i] +" : "+ i + "\n");

    //		}
    //	}
    //return MapListString;
    //}
    public string Exit(params string[] args)
    {
        StartCoroutine("Quit");
        return "Exiting";
    }
    public IEnumerator Quit()
    {
#if UNITY_EDITOR
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif

        //PlayerPrefs.DeleteAll();
        //yield return new WaitForEndOfFrame();
        //System.Diagnostics.Process.GetCurrentProcess().Kill();

        yield return new WaitForEndOfFrame();
        Application.Quit();
        yield break;

    }
    public string StopRecording(params string[] args)
    {
        //GetComponent<OutputManager>().StopRecording();
        return "Recording Stopped";
    }
    public string StartRecording(params string[] args)
    {
        //args[3] = "hello";
        float recordRate = 0.5f;
        if (args[1] != null)
        {
            float.TryParse(args[1], out recordRate);
        }
        string name = "null";
        if (args[0] != null)
        {
            name = args[0];
        }
        //GetComponent<OutputManager>().InitializeRecording(recordRate, name);
        //Global.Instance.StartRecording(recordRate, user);
        return "Recording Started";
    }
    public string StartDemo(params string[] args)
    {
        //Global.Instance.StartDemo(args[0]);
        return "Playback Started";
    }
    public string StopDemo(params string[] args)
    {
        //Global.Instance.StopDemo();
        ReloadLevel();
        return "Playback Halted";
    }
    public string setScenerio(params string[] args)
    {
        if (GameObject.FindGameObjectWithTag("GameObjectToggler") != null)
        {
            var target = GameObject.FindGameObjectWithTag("GameObjectToggler");
            int value;
            int.TryParse(args[0], out value);
            //target.GetComponent<GameObjectsToggle>().setScenerio(value);
        }

        return "setting scenerio...";
    }
    public string disableScenerio(params string[] args)
    {
        if (GameObject.FindGameObjectWithTag("GameObjectToggler") != null)
        {
            var target = GameObject.FindGameObjectWithTag("GameObjectToggler");
            int value;
            int.TryParse(args[0], out value);
            //target.GetComponent<GameObjectsToggle>().disableScenerio(value);
        }
        return "disabling scenerio...";
    }
    public string enableScenerio(params string[] args)
    {
        if (GameObject.FindGameObjectWithTag("GameObjectToggler") != null)
        {
            var target = GameObject.FindGameObjectWithTag("GameObjectToggler");
            int value;
            int.TryParse(args[0], out value);
            //target.GetComponent<GameObjectsToggle>().enableScenerio(value);
        }
        return "enabling scenerio...";
    }
    #endregion
    #region Clowder Commands
    public string parseURL(params string[] args)
    {
        ClowderBridge.Instance.DownloadFromURL(args[0]);
        return ("");
    }
    

   




  

   

   

    public string ClowderSearch(params string[] args)
    {
        print(args[0]);
        // string[] parts = args[0].Split(' ');
        string type = args[0];
        string query = args[1];
        //Debug.Log(ClowderAPIUrl + "search?query=" + query); //https://cyprus.ncsa.illinois.edu/clowder/api/search?query=riveel3d
        ClowderBridge.Instance.Search(type, query);

        //R3D_AssetDownloader.Instance.AssetURI = args[0];
        //R3D_AssetDownloader.Instance.DownloadAsset(args[0], args[1], null, null, null, null, null);
        return ("");
    }


    public string ClowderSearchMetadata(params string[] args)
    {
        print(args[0]);
        // string[] parts = args[0].Split(' ');
        string type = args[0];
        string query = args[1];
        print("HELLO!");
        //Debug.Log(ClowderAPIUrl + "search?query=" + query); //https://cyprus.ncsa.illinois.edu/clowder/api/search?query=riveel3d
        ClowderBridge.Instance.SearchMetadata(type, query);

        //R3D_AssetDownloader.Instance.AssetURI = args[0];
        //R3D_AssetDownloader.Instance.DownloadAsset(args[0], args[1], null, null, null, null, null);
        return ("");
    }

    public string ClowderCreateDataset(params string[] args)
    {
        string name = args[0];
        string description = args[1];

        //Debug.Log(ClowderAPIUrl + "search?query=" + query); //https://cyprus.ncsa.illinois.edu/clowder/api/search?query=riveel3d
        StartCoroutine(ClowderBridge.Instance.CreateDataset(name, description, null, result => {
            print("datasetID new " + result);
        }));


        //R3D_AssetDownloader.Instance.AssetURI = args[0];
        //R3D_AssetDownloader.Instance.DownloadAsset(args[0], args[1], null, null, null, null, null);
        return ("");
    }

    public string SerializeFields(params string[] args)
    {
        string target = args[0];
        if (GameObject.Find(args[0]) != null)
        {
            //GameObject.Find(args[0]).GetComponent<JsonRef>().SerializeFields();
        }


        return ("");
    }
    #endregion
    public string clearPerfs(params string[] args)
    {
        PlayerPrefs.DeleteAll();
        return ("");
    }
}
