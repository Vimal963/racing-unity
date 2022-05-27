using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class UIAdmin : MonoBehaviour
{

    public static Action WinUI;
    public static Action<int> LoseUI;

    public static Action<bool, string> UPDATESTART;


    [Header("Result UI")]
    public GameObject Lose;
    public GameObject Win;
    [Space]
    public GameObject[] reson;


    [Space]
    [Space]
    [Space]

    public Color InActive;
    public Image[] GameMode;

    [Header("Restart Setting")]
    public GameObject[] RestartSetting; // 0 is auto restart and 1 is manual restart


    [Header("Manual Restart")]
    public Button ManualRestart;

    [Header("Set Timer")]
    public Button SetTimer;

    [Header("Input Timer")]
    public InputField MM;
    public InputField SS;


    [Header("Room Data")]
    public Text TInfrigments;
    public Text TCableSnaps;
    public Text TOutOfBounds;
    public Text TCollisions;

    [Header("Room Data")]
    public Text TTeam;
    public Text TPlayer;

    [Space]
    public Text MRoomCode;

    [Header("Admin Start")]
    public Button AdminStart;

    [Header("Game Timer")]
    public TMP_Text mTime;

    [Header("Warning Play Game")]
    public GameObject WarningBoder;
    public TMP_Text GamePlayWarning;

    public static Action<string, string> PTData;
    public static Action<string> GameModes, Restart, WarningGamePlay;   //, GameOverData;
    public static Action<int> TimeGet;



    private void OnEnable()
    {
        GameModes += OnGameMode;
        Restart += OnRestart;
        TimeGet += GameTimer;
        PTData += OnPTData;
        //GameOverData += OnGameOverData;
        WarningGamePlay += WarningMessage;

        UPDATESTART += UpdateStartButton;

        WinUI += GameWinUI;
        LoseUI += GameOverUI;
    }

    private void OnDisable()
    {
        GameModes -= OnGameMode;
        Restart -= OnRestart;
        TimeGet -= GameTimer;
        PTData -= OnPTData;
        //GameOverData -= OnGameOverData;
        WarningGamePlay -= WarningMessage;

        UPDATESTART -= UpdateStartButton;

        WinUI -= GameWinUI;
        LoseUI -= GameOverUI;
    }

    private void Start()
    {
        AdminStart.interactable = false;

    }


    private void UpdateStartButton(bool isEnable, string warningMsg)
    {
        GamePlayWarning.text = " ";
        AdminStart.interactable = isEnable ? false : true;
        if (isEnable && warningMsg != null) 
        {
            Debug.Log("  -------------------warning msg  -------------   "+ warningMsg);
            WarningBoder.gameObject.SetActive(true);
            GamePlayWarning.text = warningMsg;
        }
        
        else if (!isEnable)
        {
            Debug.Log("Hide Warning Msg---------------------->  " + isEnable);
            HideWarning();
        }
        
    }


    public void PlayGame() // play button
    {
        SocketConnector.STARTGAME?.Invoke(SocketConnector.I.mRoomCode);
    }

    public void ManualRestartGame()
    {
        SocketConnector.RESTARTGAME.Invoke();
        //SocketConnector.RESTARTADMIN?.Invoke(true);  // auto-restart true
    }

    public void ChangeGameMode(string mode)// game mode button
    {
        SocketConnector.GAMEMODE?.Invoke(mode);
    }


    public void ChangeRestartMode(bool auto)
    {
        SocketConnector.RESTARTADMIN?.Invoke(auto);
    }


    public void SetTime()
    {
        string Min;
        string Sec;


        if (MM.text != "")
            Min = MM.text;
        else
            Min = MM.placeholder.GetComponent<Text>().text;
        //Min = "20";


        if (SS.text != "")
            Sec = SS.text;
        else
            Sec = SS.placeholder.GetComponent<Text>().text;
        //Sec = "00";

        Debug.Log("Set time is minm : " + MM.text + "  -- min is " + Min.ToString());
        int TSec = (int.Parse(Min.ToString()) * 60) + int.Parse(Sec);
        SocketConnector.SETTIME?.Invoke(TSec.ToString());
    }

    public void GameTimer(int RTime)
    {
        string RemainTime = Staticdata.GetTimeFormat(RTime);
        mTime.text = RemainTime;
    }


    private void OnGameMode(string obj)
    {

        JSONNode data = JSON.Parse(obj);

        if (data["mode"] == "easy")
        {
            Debug.Log("game mode is easy");
            GameMode[0].color = Color.white;
            GameMode[1].color = InActive;
            GameMode[2].color = InActive;
        }
        else if (data["mode"] == "medium")
        {
            Debug.Log("game mode is medium");
            GameMode[0].color = InActive;
            GameMode[1].color = Color.white;
            GameMode[2].color = InActive;
        }
        else if (data["mode"] == "hard")
        {
            Debug.Log("game mode is hard");
            GameMode[0].color = InActive;
            GameMode[1].color = InActive;
            GameMode[2].color = Color.white;
        }
    }


    private void OnRestart(string obj)
    {
        JSONNode data = JSON.Parse(obj);
        Debug.Log("OnAuto restart Data Lisen " + data.ToString());
        if (data["auto"].AsBool == true)
        {
            Debug.Log("is Auto restart true");
            Staticdata.I.IsAutoRestart = true;
            RestartSetting[0].SetActive(true);
            RestartSetting[1].SetActive(false);
            ManualRestart.interactable = false;
        }
        else
        {
            Debug.Log("is Auto restart false");
            Staticdata.I.IsAutoRestart = false;
            RestartSetting[0].SetActive(false);
            RestartSetting[1].SetActive(true);
            ManualRestart.interactable = true;
        }
    }


    public void OnPTData(string lplayer, string lteam)
    {
        TPlayer.text = lplayer;
        TTeam.text = lteam;
    }


    public void GameWinUI()
    {
        Win.SetActive(true);
        Lose.SetActive(false);
        Invoke(nameof(HideUI), 15);
    }

    public void GameOverUI(int gameOverReson)
    {
        if (Win.activeInHierarchy) return;
        if (Lose.activeInHierarchy) return;
        Debug.Log("GameOver UIAdimin " + gameOverReson);
        foreach (var item in reson)
        {
            item.SetActive(false);
        }
        reson[gameOverReson].SetActive(true);
        Lose.SetActive(true);
        Win.SetActive(false);

        Invoke(nameof(HideUI), 7);
    }


    public void HideUI()
    {
        mTime.text = "";
        Lose.SetActive(false);
        Win.SetActive(false);
    }


    public void WarningMessage(string mess)
    {
        WarningBoder.gameObject.SetActive(true);
        GamePlayWarning.text = mess;
        //Invoke(nameof(HideWarning), 4);
    }

    public void HideWarning()
    {
        WarningBoder.gameObject.SetActive(false);
        GamePlayWarning.text = "";
    }

    //private void OnGameOverData(string obj)
    //{
    //    JSONNode data = JSON.Parse(obj);
    //    TInfrigments.text = data["infringements"];
    //    TCableSnaps.text = data["cableSnaps"];
    //    TOutOfBounds.text = data["bounds"];
    //    TCollisions.text = data["collisions"];
    //}

    //public void CopyToClipboard() => copyToClipboard(MRoomCode.text);


#if UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void copyToClipboard(string mtext);
#endif


    //public void CopyToClipboard(Text s)
    //{
    //    TextEditor te = new TextEditor();
    //    te.text = s.text;
    //    te.SelectAll();
    //    te.Copy();
    //}


    //public static void CopyToClipboard(string str)
    //{
    //    GUIUtility.systemCopyBuffer = str;
    //}

}
