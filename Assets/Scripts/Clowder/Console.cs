using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// A console to display Unity's debug logs in-game.
/// </summary>
public class Console : MonoBehaviour
{
    private static Console instance;
    public static Console Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Console();
            }
            return instance;
        }
    }

    protected struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }

    #region Inspector Settings

    /// <summary>
    /// The hotkey to show and hide the console window.
    /// </summary>
    public KeyCode toggleKey = KeyCode.BackQuote;

    /// <summary>
    /// Whether to open the window by shaking the device (mobile-only).
    /// </summary>
    public bool shakeToOpen = true;

    public bool OpenOnStart = true;

    /// <summary>
    /// The (squared) acceleration above which the window should open.
    /// </summary>
    public float shakeAcceleration = 3f;

    /// <summary>
    /// Whether to only keep a certain number of logs.
    ///
    /// Setting this can be helpful if memory usage is a concern.
    /// </summary>
    public bool restrictLogCount = false;

    /// <summary>
    /// Number of logs to keep before removing old ones.
    /// </summary>
    public int maxLogs = 1000;
    public bool doStackTrace = false;
    #endregion

    protected List<Log> logs = new List<Log>();
    Vector2 scrollPosition;
    protected bool visible = false;
    protected bool collapse = true;
    private ConsoleCommandsRepository consoleCommandsRepository;
    // Visual elements:

    [HideInInspector]
    public string input = "";
    public void setInput(string s)
    {
        input = s;
    }
    public void setCollase(bool b)
    {
        collapse = b;
    }

    protected static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
        {
            { LogType.Assert, Color.cyan },
            { LogType.Error, Color.red },
            { LogType.Exception, Color.red},
            { LogType.Log, new Color(0.9f, 0.9f, 0.9f, 1)},
            { LogType.Warning, Color.yellow },
        };

    const string windowTitle = "";
    const int margin = 10;
    static readonly GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    static readonly GUIContent submitLabel = new GUIContent("Submit", "Submit Entry to console.");
    static readonly GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
    static readonly GUIContent strackTraceLabel = new GUIContent("Verbose", "Show Full Logs");

    //private GUISkin consoleSkin;
    readonly Rect titleBarRect = new Rect(0, 0, 10000, 20);
    Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    public float guiLocalScale = 12f;

    void printConsoleHeaderText()
    {
        LogText("Type 'help' to get a list of console commands available. Press '~' key to toggle console. The 'exit' command will close the application.", LogType.Log);
        PlayerPrefs.SetString("date_time", System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
        LogText(("WIKAR   -   " + PlayerPrefs.GetString("date_time")), LogType.Assert);
    }

    protected virtual void Awake()
    {
        instance = this;
        //consoleSkin = Resources.Load("ConsoleSkin") as GUISkin;
        printConsoleHeaderText();
    }

    protected virtual void Start()
    {

        consoleCommandsRepository = ConsoleCommandsRepository.Instance;
        //consoleSkin = Resources.Load("ConsoleSkin.guiskin") as GUISkin;
        if (OpenOnStart)
        {
            visible = true;
            GUI.FocusControl("input");
        }
        //ScaleGUI();
    }

    protected virtual void Update()
    {

        if (Input.GetKeyDown(toggleKey))
        {
            visible = !visible;
            if (visible)
            {
                GUI.FocusControl("input");
            }
        }
        //phone
        if (shakeToOpen && Input.acceleration.sqrMagnitude > shakeAcceleration && !cooldown)
        {
            visible = !visible;
            StartCoroutine(doCooldown());
        }
    }

    bool cooldown = false;

    IEnumerator doCooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(1);
        cooldown = false;
    }

    void OnGUI()
    {

        if (!visible)
        {
            return;
        }
        else
        {
            //GUI.skin = consoleSkin;
            //Vector2 nativeSize = new Vector2(640, 480);
            //consoleSkin.fontSize = (int)(20.0f * ((float)Screen.width / (float)nativeSize.x));
            // Your code here
            windowRect = GUILayout.Window(123456, windowRect, DrawConsoleWindow, windowTitle);
            //ScaleGUI();
        }
    }

    public void ClearLog()
    {
        logs.Clear();
        printConsoleHeaderText();
    }

    private void HandleSubmit()
    {
        Activate();
        input = "";
    }

    public void sendCommand(string s)
    {
        input = s;
        HandleSubmit();
    }

    public void Activate()
    {
        string[] parts = input.Split(' ');
        string command = parts[0];
        string[] args = parts.Skip(1).ToArray();

        LogText(">" + input, LogType.Assert);
        if (consoleCommandsRepository.HasCommand(command))
        {
            input = "";
            LogText(consoleCommandsRepository.ExecuteCommand(command, args), LogType.Log);
        }
        else
        {
            LogText("Command " + command + " not found.", LogType.Error);
        }
    }

    public void LogText(string message, LogType type)
    {

        HandleLog(message, "", type);

    }


    //Regex regularExpression = new Regex("^[a-zA-Z0-9_]*$");
    //char[] newLine = "\n\r".ToCharArray();

    /// <summary>
    /// Displays a window that lists the recorded logs.
    /// </summary>
    /// <param name="windowID">Window ID.</param>
    void DrawConsoleWindow(int windowID)
    {
        //HandleSubmit();
        if (Event.current.type == EventType.KeyDown)
        {
            // no matter where, but if Escape was pushed, close the dialog
            if (Event.current.keyCode == KeyCode.Escape || Event.current.keyCode == toggleKey)
            {
                visible = false;
                return; // no point in continuing if closed
            }

            // we look if the event occured while the focus was in our input element
            if (GUI.GetNameOfFocusedControl() == "input" && Event.current.keyCode == KeyCode.Return && input != "")
            {
                //Debug.Log("HERE");
                HandleSubmit();
                ScrollToBottom();
                return;
            }
        }

        if (GUI.GetNameOfFocusedControl() == string.Empty)
        {
            GUI.FocusControl("input");
        }
        DrawLogsList();
        DrawToolbar();

        // Allow the window to be dragged by its title bar.
        GUI.DragWindow(titleBarRect);

        //GUI.SetNextControlName("input");
        //inputValue = GUILayout.TextField(inputValue);
        // in case nothing else if focused, focus our input
    }


    Rect innerScrollRect;

    Rect outerScrollRect;

    /// <summary>
    /// Displays a scrollable list of logs.
    /// </summary>
    void DrawLogsList()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Used to determine height of accumulated log labels.
        GUILayout.BeginVertical();

        // Iterate through the recorded logs.
        for (var i = 0; i < logs.Count; i++)
        {
            var log = logs[i];

            // Combine identical messages if collapse option is chosen.
            if (collapse && i > 0)
            {
                var previousMessage = logs[i - 1].message;

                if (log.message == previousMessage)
                {
                    continue;
                }
            }

            //consoleSkin.label.normal.textColor = logTypeColors[log.type];
            //GUI.contentColor = logTypeColors[log.type];
            if (log.type != LogType.Log && doStackTrace)
            {
                GUILayout.Label(log.message + " at : \r\n" + log.stackTrace);
            }
            else
            {
                GUILayout.Label(log.message);
            }
        }

        GUILayout.EndVertical();
        innerScrollRect = GUILayoutUtility.GetLastRect();
        GUILayout.EndScrollView();
        outerScrollRect = GUILayoutUtility.GetLastRect();

        // If we're scrolled to bottom now, guarantee that it continues to be in next cycle.
        if (Event.current.type == EventType.Repaint && IsScrolledToBottom(innerScrollRect, outerScrollRect))
        {
            ScrollToBottom();
        }

        // Ensure GUI colour is reset before drawing other components.
        GUI.contentColor = Color.white;
    }

    /// <summary>
    /// Displays options for filtering and changing the logs list.
    /// </summary>
    void DrawToolbar()
    {

        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("input");
        input = GUILayout.TextField(input);

        if (GUILayout.Button(submitLabel, GUILayout.ExpandWidth(false)))
        {
            HandleSubmit();
            ScrollToBottom();
            return;
        }

        if (GUILayout.Button(clearLabel, GUILayout.ExpandWidth(false)))
        {
            logs.Clear();
        }
        collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));
        doStackTrace = GUILayout.Toggle(doStackTrace, strackTraceLabel, GUILayout.ExpandWidth(false));


        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Records a log from the log callback.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="stackTrace">Trace of where the message came from.</param>
    /// <param name="type">Type of message (error, exception, warning, assert).</param>
    protected virtual void HandleLog(string message, string stackTrace, LogType type)
    {
        logs.Add(new Log
        {
            message = message,
            stackTrace = stackTrace,
            type = type,
        });
        TrimExcessLogs();
        //if(scrollPosition == new Vector2(0, Int32.MaxValue))
        //{
        //    ScrollToBottom();
        //}
    }

    /// <summary>
    /// Determines whether the scroll view is scrolled to the bottom.
    /// </summary>
    /// <param name="innerScrollRect">Rect surrounding scroll view content.</param>
    /// <param name="outerScrollRect">Scroll view container.</param>
    /// <returns>Whether scroll view is scrolled to bottom.</returns>
    bool IsScrolledToBottom(Rect innerScrollRect, Rect outerScrollRect)
    {
        var innerScrollHeight = innerScrollRect.height;

        // Take into account extra padding added to the scroll container.
        var outerScrollHeight = outerScrollRect.height - GUI.skin.box.padding.vertical;

        // If contents of scroll view haven't exceeded outer container, treat it as scrolled to bottom.
        if (outerScrollHeight > innerScrollHeight)
        {
            return true;
        }

        var scrolledToBottom = Mathf.Approximately(innerScrollHeight, scrollPosition.y + outerScrollHeight);
        return scrolledToBottom;
    }

    /// <summary>
    /// Moves the scroll view down so that the last log is visible.
    /// </summary>
    void ScrollToBottom()
    {
        scrollPosition = new Vector2(0, Int32.MaxValue);
    }

    /// <summary>
    /// Removes old logs that exceed the maximum number allowed.
    /// </summary>
    void TrimExcessLogs()
    {
        if (!restrictLogCount)
        {
            return;
        }

        var amountToRemove = Mathf.Max(logs.Count - maxLogs, 0);

        if (amountToRemove == 0)
        {
            return;
        }

        logs.RemoveRange(0, amountToRemove);
    }
}