using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum panal
{
    LoginPanal,
    CreateOrJoinPanal,
    PlayPenal,
    GameOverPanal,
    SettingPanal,
    Null
}

[System.Serializable]
public class DisplayPanal
{
    [SerializeField]
    public panal panals;
    [SerializeField]
    public GameObject PanalObject;
}


public class UiManager : MonoBehaviour
{

    public static bool isGameStart = false;

    public static UiManager I;
    public System.Collections.Generic.List<DisplayPanal> PanalList = new System.Collections.Generic.List<DisplayPanal>();
    public static System.Action<panal> Display;

    [Space]
    public TMP_Text RoomInfo;
    public TMP_InputField Roomname;

    [Space]
    public panal curruntpanal;

    [Space]
    public GameObject ReadyBtn;

    [Space]
    public GameObject SubmitBtn;

    [Space]
    public GameObject Win, Lose;

    [Space]
    public GameObject ConvoySelect, ConvoyWarningMess;

    [Space]
    public GameObject Connected, SelectedConvoyIndex, ReSelectConvoyBtn, ReadytoPlay, EntercodeForJoin, FailedJoinMessage; //, RoomNotFound;

    [Space]
    public GameObject WarningImage;

    [Space]
    public GameObject roomCodeEntry;

    [Space]
    public GameObject HomebtnOnGameover;

    [Space]
    public GameObject RestartbtnOnGameOver;

    [Space]
    public GameObject HomebtnOnGameWin;

    [Space]
    public GameObject RestartbtnOnGameWin;

    [Space]
    public GameObject[] gameOverReason;

    [Space]
    public GameObject tofarObject;
    public GameObject tonearObject;

    [Space]
    public GameObject[] ReadyCountDown;

    [Header("Loading Panel")]
    public GameObject LoadingPanel;


    [Header("Game Timer")]
    public TMP_Text mTime;
    //public TMP_Text mMin;
    //public TMP_Text mSec;

    [Header("Warning Play Game")]
    public TMP_Text GamePlayWarning;

    //[Space]
    //public int LastSelectedConvoy = -1;

    //[Space]
    //public UnityEngine.UI.Outline[] SelectedCol;


    private void Awake()
    {
        I = this;
    }

    private void OnEnable()
    {
        Display += DisplayPanels;
    }
    private void OnDisable()
    {
        Display -= DisplayPanels;
    }



    public void Start()
    {
        mTime.gameObject.SetActive(false);
        DisplayPanels(panal.CreateOrJoinPanal);
        Debug.Log("IsAutoRestart Type : " + Staticdata.I.IsAutoRestart);
        if (Staticdata.I.IsAutoRestart)
        {
            // Player id emit
            SocketConnector.RESTART?.Invoke();
            //Staticdata.I.IsAutoRestart = false;
            Roomname.gameObject.SetActive(false);
            SubmitBtn.SetActive(false);

            RejoinPlayerUI();  // AUTO REJOINED ROOM
        }
        //else if (Staticdata.I.IsHome)
        //{
        //    // Player id emit
        //    SocketConnector.HOME?.Invoke();
        //    Staticdata.I.IsHome = false;
        //    //LoginInGame();
        //}

    }

    public void DisplayPanels(panal panal)
    {
        foreach (DisplayPanal p in PanalList)
        {
            p.PanalObject.SetActive(false);
        }
        if (panal != panal.Null)
        {
            PanalList.Find(x => x.panals == panal).PanalObject.SetActive(true);
            curruntpanal = panal;
        }
    }


    public void createRoompanalOpen()
    {
        //SocketConnector.CREATEROOM?.Invoke(string.Empty);
    }

    public void JoinRoomPanalOpen()//JOIN BUTTON
    {
        if (!Roomname.text.Equals(string.Empty))
        {
            SocketConnector.AutoJoinRoom = false;
            SocketConnector.JOINROOM?.Invoke(Roomname.text);
        }
        else
        {
            EntercodeForJoin.SetActive(true);
            FailedJoinMessage.SetActive(false);
            //RoomNotFound.SetActive(false);
            Connected.SetActive(false);
            ReadytoPlay.SetActive(false);
        }

    }
    private int selectedConvoy;
    public void SelectionConvoy(int ConID)
    {
        selectedConvoy = ConID + 1;
        SocketConnector.JOINCONVOY?.Invoke(ConID.ToString());
    }

    //public void SelectionConvoy(int ConID)
    //{
    //    LastSelectedConvoy = ConID;

    //    foreach (var item in SelectedCol)
    //        item.enabled = false;

    //    SelectedCol[ConID].enabled = true;
    //    ConvoyWarningMess.SetActive(false);
    //}

    //public void JoinConvoy()
    //{
    //    if (LastSelectedConvoy == -1)
    //    {
    //        ConvoyWarningMess.SetActive(true);
    //        ConvoyWarningMess.GetComponent<TextMeshProUGUI>().text = "Please Select Any Convoy";
    //    }
    //    else
    //        SocketConnector.JOINCONVOY?.Invoke(LastSelectedConvoy.ToString());
    //}

    public void ReSelectyConvoy()
    {
        SocketConnector.RESELECTVONVOY?.Invoke();
    }

    public void JoinedConvoyUI()
    {
        ConvoySelect.SetActive(false);
        ConvoyWarningMess.SetActive(false);
        ClientConnected();
    }

    public void HostConnected(string roomCode)
    {
        //isHost = true;
        RoomInfo.text = roomCode;
    }

    public void ChooseConvoy()
    {
        EntercodeForJoin.SetActive(false);
        FailedJoinMessage.SetActive(false);
        roomCodeEntry.SetActive(false);
        SubmitBtn.SetActive(false);
        ConvoyWarningMess.SetActive(false);
        ConvoySelect.SetActive(true);
        ReSelectConvoyBtn.SetActive(false);
        Connected.SetActive(false);

    }

    public void ConvoyMessage(string message)
    {
        EntercodeForJoin.SetActive(false);
        FailedJoinMessage.SetActive(false);
        roomCodeEntry.SetActive(false);
        SubmitBtn.SetActive(false);
        ConvoySelect.SetActive(true);
        ConvoyWarningMess.SetActive(true);
        ConvoyWarningMess.GetComponent<TextMeshProUGUI>().text = message;

    }

    public void RejoinPlayerUI()
    {
        EntercodeForJoin.SetActive(false);
        FailedJoinMessage.SetActive(false);
        Connected.SetActive(true);
        ReSelectConvoyBtn.SetActive(false);
        ReadytoPlay.SetActive(false);
        roomCodeEntry.SetActive(false);
        SubmitBtn.SetActive(false);
        //ReadyBtn.SetActive(true);
        ConvoySelect.SetActive(false);
        ConvoyWarningMess.SetActive(false);
        GamePlayWarning.gameObject.SetActive(false);
        GamePlayWarning.text = "";
        SelectedConvoyIndex.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void ClientConnected()
    {
        //isClient = true;
        EntercodeForJoin.SetActive(false);
        FailedJoinMessage.SetActive(false);
        Connected.SetActive(true);
        ReSelectConvoyBtn.SetActive(true);
        ReadytoPlay.SetActive(false);
        roomCodeEntry.SetActive(false);
        SubmitBtn.SetActive(false);
        //ReadyBtn.SetActive(true);
        ConvoySelect.SetActive(false);
        ConvoyWarningMess.SetActive(false);
        GamePlayWarning.gameObject.SetActive(false);
        GamePlayWarning.text = "";
        SelectedConvoyIndex.GetComponent<TextMeshProUGUI>().text = "You have selected convoy " + selectedConvoy;

    }

    /*
    public void HomeBtnClick()
    {
        //SocketConnector.I.socket.Disconnect();
        //SocketConnector.I.socketManager.Close();
        Staticdata.I.IsHome = true;
        Staticdata.I.mRoomId = "";
        Staticdata.I.mRoomName = "";
        Staticdata.I.LastRoomId = 0;
        //GameManger.mPlayerID = "";
        //Staticdata.I.mteamCode = "";
        SceneManager.LoadScene(0);
    }

    public void RestartBtnClick()
    {
        //SocketConnector.I.socket.Disconnect();
        //SocketConnector.I.socketManager.Close();
        //Staticdata.I.IsAutoRestart = true;
        SceneManager.LoadScene(0);
    }
     */

    public void StartPlayGame()
    {
        ReadyBtn.SetActive(false);
        GamePlayWarning.text = "";
        GamePlayWarning.gameObject.SetActive(false);
        DisplayPanels(panal.PlayPenal);
        AudioManager.I?.GamePlaySoundPlay();
        StartCoroutine(ReadyTimer());
        mTime.gameObject.SetActive(true);
        GameManger.StartPlay?.Invoke();

    }

    public void LoginInGame()
    {
        DisplayPanels(panal.CreateOrJoinPanal);
    }


    public void ReadyInRoom()
    {
        EntercodeForJoin.SetActive(false);
        FailedJoinMessage.SetActive(false);
        Connected.SetActive(false);
        ReSelectConvoyBtn.SetActive(false);
        ReadytoPlay.SetActive(true);
        ReadyBtn.SetActive(false);
    }


    public void JoinRoomFailed(string message)
    {
        //RoomNotFound.SetActive(true);
        Debug.Log(message);
        FailedJoinMessage.SetActive(false);
        FailedJoinMessage.GetComponent<TextMeshProUGUI>().text = message;
        FailedJoinMessage.SetActive(true);
        EntercodeForJoin.SetActive(false);
        Connected.SetActive(false);
        ReSelectConvoyBtn.SetActive(false);
        ReadytoPlay.SetActive(false);
    }


    public void gameOverReasonSet(int index)
    {
        mTime.gameObject.SetActive(false);
        foreach (GameObject p in gameOverReason)
        {
            p.SetActive(false);
        }
        gameOverReason[index].SetActive(true);
    }

    IEnumerator ReadyTimer()
    {
        yield return null;
        for (int i = 0; i < ReadyCountDown.Length; i++)
        {
            if (i > 0)
            {
                ReadyCountDown[i - 1].SetActive(false);
            }
            ReadyCountDown[i].SetActive(true);
            yield return new WaitForSeconds(1.7f);
        }
        yield return new WaitForSeconds(1.7f);
        ReadyCountDown[ReadyCountDown.Length - 1].SetActive(false);


    }


    public void GameTimer(int RTime)
    {
        string RemainTime = Staticdata.GetTimeFormat(RTime);
        mTime.text = RemainTime;
    }


    public void WarningMessage(string mess)
    {
        GamePlayWarning.gameObject.SetActive(true);
        GamePlayWarning.text = mess;
        ConvoyWarningMess.SetActive(false);
        Connected.SetActive(false);
        ReSelectConvoyBtn.SetActive(false);
    }


    public void HideLoader()
    {
        LoadingPanel.SetActive(false);
    }

    public void ShowLoader()
    {
        LoadingPanel.SetActive(true);
    }

}
