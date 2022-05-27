using System;
using System.Runtime.InteropServices;
using BestHTTP.SocketIO;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum SocketState
{
    Connected,
    Disconnected,
    Error
}

public class SocketConnector : MonoBehaviour
{
    // ADMIN EVENTS
    //public static Action<bool, bool> RELOAD;
    public static Action<bool> RESTARTADMIN;
    public static Action RESTARTGAME, REJOINADMIN;
    public static Action<string> STARTGAME, GAMEMODE, SETTIME;     // ADMIN START THE GAME (TPDATA, GAMEOVERDATA REMOVE)


    // USER EVENTS
    public static Action<string> SENDPOSITION, JOINROOM, JOINCONVOY, CREATEROOM, GAMEWINME;  //READYPLAYER JOINTBRACK

    public static Action<string, string, string> GAMEOVERME;

    public static Action RESTART, PLAYERREJOIN, RESELECTVONVOY;
    //public static Action HOME;

    public static SocketConnector I;

    public SocketManager socketManager;

    public Socket socket;

    public SocketState socketState;

    public static bool AutoJoinRoom = false;

    internal string mRoomCode = "";
    internal string isAdmin = "";
    internal string isLaunch = "";
    internal string isSpactate = "";
    internal string mToken = "";


    private void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(I);
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        STARTGAME += StartGame;
        RESTARTGAME += RestartByAdminGame;
        GAMEMODE += GameMode;
        RESTARTADMIN += RestartAdmin;
        SETTIME += SetTime;
        REJOINADMIN += ReJoinAdmin;

        //TPDATA += TPData;
        //GAMEOVERDATA += GameOverData;

        JOINROOM += JoinRoom;
        JOINCONVOY += JoinConvoy;
        RESELECTVONVOY += ReSelectConvoy;
        SENDPOSITION += SendPosToServer;
        GAMEOVERME += GameOver;
        GAMEWINME += GameWin;
        RESTART += RestartGamePlay;
        PLAYERREJOIN += PlayerRejoinEmit;
        //READYPLAYER += ReadyPlayer;
        //JOINTBRACK += JointBrack;
        //HOME += PlayerLeftRoom;
    }

    private void OnDisable()
    {
        STARTGAME -= StartGame;
        RESTARTGAME -= RestartByAdminGame;
        GAMEMODE -= GameMode;
        RESTARTADMIN -= RestartAdmin;
        SETTIME -= SetTime;
        REJOINADMIN -= ReJoinAdmin;

        //TPDATA -= TPData;
        //GAMEOVERDATA -= GameOverData;

        JOINROOM -= JoinRoom;
        JOINCONVOY -= JoinConvoy;
        RESELECTVONVOY -= ReSelectConvoy;
        SENDPOSITION -= SendPosToServer;
        GAMEOVERME -= GameOver;
        GAMEWINME -= GameWin;
        RESTART -= RestartGamePlay;
        PLAYERREJOIN -= PlayerRejoinEmit;
        //READYPLAYER -= ReadyPlayer;
        //JOINTBRACK -= JointBrack;
        //HOME -= PlayerLeftRoom;
    }

    private void Start()
    {
        if (socket == null)
            ConnectWithSocket();

    }

    private void ConnectWithSocket()
    {

        if (socket == null)
        {
            socketManager = new SocketManager(new Uri(Staticdata.I.ServerURL));
            socketManager.Open();
            socket = socketManager.GetSocket();

            socket.On(SocketIOEventTypes.Connect, OnSocketConnected);
            socket.On(SocketIOEventTypes.Disconnect, OnSocketDisconnected);
            socket.On(SocketIOEventTypes.Error, OnSocketError);
        }

        //if (socket == null)
        //TimeSpan miliSecForReconnect = TimeSpan.FromMilliseconds(0.0);
        //SocketOptions options = new SocketOptions();
        //options = new SocketOptions
        //{
        //    ReconnectionAttempts = 0,
        //    AutoConnect = false,
        //    ReconnectionDelay = miliSecForReconnect
        //};

        //ObservableDictionary<string, string> observableDictionary = new ObservableDictionary<string, string>
        //{
        //    { "Name", String.IsNullOrEmpty("") ? "Guest": "" }
        //};
        //options.AdditionalQueryParams = observableDictionary;


    }

    private void OnSocketConnected(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("=== OnSocket Connected!");
        socketState = SocketState.Connected;
        RegisterEvents();
    }

    private void OnSocketDisconnected(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("=== OnSocket DisConnected!");
        socketState = SocketState.Disconnected;

    }

    private void OnSocketError(Socket socket, Packet packet, object[] args)
    {
        ////Debug.LogError("Socket Get Error");
        socketState = SocketState.Error;
        Error error = args[0] as Error;
        switch (error.Code)
        {
            case SocketIOErrors.User:
                Debug.Log("Exception in an event handler!" + error.Message);
                break;
            case SocketIOErrors.Internal:
                Debug.Log("Internal error! Message: " + error.Message);
                break;
            default:
                Debug.Log("Server error! Message: " + error.Message);
                break;
        }
    }


    private void RegisterEvents()
    {
        Debug.Log("Register All Events");

        // ADMIN EVENTS
        socket.On("adminSettingDataAction", OnAdminSettingCallback);
        socket.On("gameStartAdminAction", OnStartGame);
        socket.On("timerAction", OnStartGameTime);
        socket.On("gameRestartManualAdminAction", OnReStartGame);       // admin click on restart button when manually 
        socket.On("gameModeAdminAction", OnGameMode);
        socket.On("gameRestartAdminAction", OnRestartAdmin); // autorestart option
        socket.On("adminJoinSpactateAndRejoin", OnAdminRejoin);
        socket.On("startButtonAction", OnCheckStartButton);
        //socket.On("startButtonAction", OnConvoyLisenAdmin);


        // USERS EVENTS
        socket.On("playerAction", JoinFailed);   // JOIN ROOM FAILD
        socket.On("playerListAction", OnRoomJoin);
        socket.On("playerRejoinInJoinAction", OnAutoRoomJoin);
        socket.On("chooseConvoyAction", OnConvoyJoin);
        socket.On("reSelectConvoyAction", OnConvoyReselect);
        socket.On("playerDataAction", OnGetOtherPlayerPos);
        socket.On("gameOverAction", OnGameOverByPlayer);
        socket.On("gameWinAction", OnGameWinByPlayer);
        socket.On("gameRestartAction", Restart);
        socket.On("nearAndFarMessageAction", OnWarningMessage);

        socket.On("playerRejoinAction", PlayerRejoin);
        socket.On("playerLeftAction", PlayerLeft);

        socket.On("obstaclesAction", GameRoadSet);

        //socket.On("playerLeftAction", OnGameOverByPlayer);
        //socket.On("playerLeftAction", OnJointBreack);
        //socket.On("receiveJoin", OnJointBreack);
        //socket.On("playerReadyAction", OnPlayerReady);


        //if (HasCheckReloaded())
        //{
        //    Debug.Log("Page is reloaded truee");
        //    JSONNode data = new JSONObject
        //    {
        //        ["roomName"] = PlayerPrefs.GetString("roomCode").ToString(),
        //        ["playerId"] = PlayerPrefs.GetString("playerId").ToString()
        //    };
        //    Debug.Log("Reload page event push" + data.ToString());
        //    socket?.Emit("disconnectmanually", data.ToString());
        //}
        //else
        //{
        //    Debug.Log("Page is Not reloaded");
        //}

        //if (LocalStorageData.HasSessionKey("spactate"))
        //{
        //    UiManager.I.ShowLoader();
        //    LocalSaveRoomID = LocalStorageData.GetSessionString("roomCode");
        //    Debug.Log("Start Local Save RoomID : " + LocalSaveRoomID);

        //    AdminSettings(true);
        //}                       https://highroad.rohei.com/Launch/?roomCode?1935&&admin?true&&lanch?true&&spactate?false&&token?61794aff4843ec0dac18fbb3
        //string MainURL = "http://192.168.40.89:2323/carRacingLaunch/?roomCode?7690&&admin?true&&lanch?true&&spactate?false&&token?61794aff4843ec0dac18fbb3";
        //string MainURL = "http://192.168.40.89:2323/carRacingLaunch/?roomCode?9744&&admin?true&&lanch?true&&spactate?false&&token?61794aff4843ec0dac18fbb3";
        //string MainURL = "http://192.168.40.27:2323/carRasingGame";
        string MainURL = Application.absoluteURL;

        Debug.Log("MainURL Length : " + MainURL.Length);

        if (MainURL.Length <= 50)// for player
        {
            Debug.Log("User Login ");

            print("RoomCode " + PlayerPrefs.GetString("roomCode"));
            if (PlayerPrefs.HasKey("roomCode"))
            {
                AutoJoinRoom = true;
                JoinRoom(PlayerPrefs.GetString("roomCode"));
            }
        }
        else // for admin
        {
            var roomsplit = MainURL.Split("?"[0])[2];
            var adminsplit = MainURL.Split("?"[0])[3];
            var launchsplit = MainURL.Split("?"[0])[4];
            var spactatesplit = MainURL.Split("?"[0])[5];
            var tokensplit = MainURL.Split("?"[0])[6];

            mRoomCode = roomsplit.Split("&&"[0])[0];
            isAdmin = adminsplit.Split("&&"[0])[0];
            isLaunch = launchsplit.Split("&&"[0])[0];
            isSpactate = spactatesplit.Split("&&"[0])[0];
            mToken = tokensplit.Split("&&"[0])[0];

            Debug.Log("lRoomCode : " + mRoomCode);
            Debug.Log("isAdmin : " + isAdmin);
            Debug.Log("isLaunch : " + isLaunch);
            Debug.Log("isSpactate : " + isSpactate);
            Debug.Log("isToken : " + mToken);

            UiManager.I.ShowLoader();
            AdminSettings();
        }
    }


    #region Admin Event
    private void AdminSettings()
    {
        JSONNode data = new JSONObject
        {
            ["roomCode"] = mRoomCode.ToString(),
            ["token"] = mToken.ToString(),
            ["admin"] = true
        };

        Debug.Log("AdminSettings Data push : " + data.ToString());

        socket?.Emit("adminSettingData", data.ToString());
    }

    private void StartGame(string roomCode) //---------> play button
    {
        JSONNode data = new JSONObject
        {
            ["roomCode"] = roomCode.ToString(),
            ["isGameStart"] = true
        };
        Debug.Log("StartGame Emit : " + roomCode);
        socket?.Emit("gameStartAdmin", data.ToString());
    }

    private void RestartByAdminGame()
    {
        JSONNode data = new JSONObject
        {
            ["roomCode"] = mRoomCode
        };
        Debug.Log("gameRestartManualAdmin Emit : " + data.ToString());
        socket?.Emit("gameRestartManualAdmin", data.ToString());
    }

    private void GameMode(string gameMode)
    {
        JSONNode data = new JSONObject
        {
            ["roomCode"] = mRoomCode,
            ["mode"] = gameMode.ToString()
        };

        Debug.Log("GameMode data : " + gameMode + " RoomCode : " + mRoomCode);
        Debug.Log("GameMode Emit : " + data.ToString());
        socket.Emit("gameModeAdmin", data.ToString());
    }

    private void RestartAdmin(bool isAuto)
    {
        JSONNode data = new JSONObject
        {
            ["roomCode"] = mRoomCode,
            ["auto"] = isAuto,
            ["manual"] = !isAuto
        };
        Debug.Log("RestartAdmin Emit : " + data.ToString());
        socket?.Emit("gameRestartAdmin", data.ToString());
    }

    private void SetTime(string ltimer)
    {
        JSONNode data = new JSONObject
        {
            ["roomCode"] = mRoomCode,
            ["timer"] = ltimer.ToString()
        };
        Debug.Log("SetTime Emit : " + data.ToString());
        socket?.Emit("setTimerByAdmin", data.ToString());
    }

    private void ReJoinAdmin()
    {
        JSONNode data = new JSONObject
        {
            ["roomCode"] = mRoomCode,
            ["token"] = mToken,
            ["admin"] = true
        };
        Debug.Log("ReJoinAdmin Data push : " + data.ToString());
        socket.Emit("rejoinAdmin", data.ToString());
    }


    #endregion Admin Event



    #region Player Event Send

    private void JoinRoom(string Roomname)
    {
        string lType = AutoJoinRoom ? "rejoin" : "join";

        JSONNode data = new JSONObject
        {
            ["roomName"] = Roomname.ToString(),
            ["playerId"] = PlayerPrefs.GetString("playerId").ToString(),
            ["type"] = lType
        };

        Debug.Log("Join Room Push" + data.ToString());
        socket?.Emit("joinRoom", data.ToString());
    }

    private void  JoinConvoy(string convoyNo)
    {
        JSONNode data = new JSONObject
        {
            ["playerId"] = GameManger.mPlayerID,
            ["convoyCode"] = convoyNo.ToString(),
            ["roomName"] = Staticdata.I.LastRoomId.ToString()
        };

        Debug.Log("Join Convoy " + data.ToString());
        socket.Emit("chooseConvoy", data.ToString());
    }

    public void ReSelectConvoy()
    {
        JSONNode data = new JSONObject
        {
            ["playerId"] = GameManger.mPlayerID.ToString()
        };
        Debug.Log("Push reSelectConvoy Data To Server : " + data.ToString());
        socket?.Emit("reSelectConvoy", data.ToString());
    }

    private void SendPosToServer(string data)
    {
        //Debug.Log("SendPosToServer Push Data =" + data.ToString());
        socket?.Emit("playerData", data.ToString());
    }

    private void GameOver(string _playerID, string _roomID, string _reason)
    {
        if (UiManager.I.curruntpanal == panal.GameOverPanal)
        {
            Debug.Log("GameOver Panel open and  return when emit game over data");
            return;
        }

        JSONNode data = new JSONObject
        {
            ["playerId"] = _playerID.ToString(),
            ["roomId"] = _roomID.ToString(),
            ["reason"] = _reason.ToString()
        };

        Debug.Log("gameOverData by me " + data.ToString());
        socket?.Emit("gameOver", data.ToString());
    }

    private void GameWin(string arg1)
    {
        Debug.Log("Win Data push : " + arg1.ToString() + "player ID : " + arg1.ToString());
        JSONNode data = new JSONObject
        {
            ["playerId"] = arg1.ToString()
        };
        socket?.Emit("gameWin", data.ToString());
    }

    public void PlayerRejoinEmit()
    {
        JSONNode data = new JSONObject
        {
            ["roomCode"] = Staticdata.I.mRoomName
        };
        Debug.Log("Push Player Rejoin Data To Server : " + data.ToString());
        socket?.Emit("playerRejoin", data.ToString());
    }

    private void RestartGamePlay()
    {
        JSONNode data = new JSONObject
        {
            ["playerId"] = GameManger.mPlayerID,
            ["roomName"] = Staticdata.I.mRoomName
        };
        Debug.Log("Push Player Restart Data To Server : " + data.ToString());
        socket?.Emit("gameRestart", data.ToString());
    }

    #endregion



    #region Admin Event Recieved

    private void OnAdminSettingCallback(Socket socket, Packet packet, object[] args)
    {
        if (Staticdata.isAdmin)
            Debug.Log("**** OnAdminSettingCallback : " + args[0].ToString());

        Staticdata.AdminSetting = args[0].ToString();

        if (Staticdata.isAdmin)
        {
            AdminDataSetup.UPDATEADINSETTING.Invoke();
            return;
        }
        else if (isSpactate == "true")
        {
            SceneManager.LoadScene(2);
        }
        else if (isLaunch == "true")
        {
            SceneManager.LoadScene(1);
        }
    }

    private void OnStartGame(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("OnStartGame Action =====" + args[0].ToString());

        if (!Staticdata.isAdmin)// for player
        {
            if (Staticdata.I.LastRoomId == 0)
                return;

            if (GameManger.mPlayerID != null)
                GameManger.StartGame?.Invoke(args[0].ToString());
        }

        if (Staticdata.isAdmin) // for admin
        {
            JSONNode data = JSON.Parse(args[0].ToString());
            if (data["admin"].AsBool == false)
            {
                Debug.Log(":::::Admin Start game Listen ::::: Any one team has only one player to play game");
                UIAdmin.WarningGamePlay.Invoke(data["message"]);
                return;
            }
            GameManger.StartGame?.Invoke(args[0].ToString());
            GameManger.isGameStart = true;
        }
    }

    private void OnStartGameTime(Socket socket, Packet packet, object[] args)
    {
        //Debug.Log("----------OnStartGameTime Action-------- " + args[0].ToString());
        GameManger.Timer?.Invoke(args[0].ToString());
    }

    private void OnReStartGame(Socket socket, Packet packet, object[] args)     // ADMIN MANUAL RESTART GAME CALLBACK
    {
        Debug.Log("OnReStartGame Action =====" + args[0].ToString());
        if (!Staticdata.isAdmin)
        {
            if (Staticdata.I.LastRoomId == 0)
                return;
        }
        else
        {
            JSONNode data = JSON.Parse(args[0].ToString());
            if (data["admin"].AsBool == false)
            {
                UIAdmin.WarningGamePlay.Invoke(data["message"]);
                Debug.Log("Game already started and manual restart button click by admin RETURN ");
                return;
            }
            Debug.Log("Manual restart by admin success and autorestart true");
            RESTARTADMIN?.Invoke(true);  // autorestart on after manual restart click

            //GameManger.isGameStart = true;
            //Debug.Log("Admin StartGame TRUE");
        }

        if (GameManger.mPlayerID != null)
        {
            Debug.Log("Return player on admin start event listern :: Player id is null " + GameManger.mPlayerID);
            GameManger.AdminRestart?.Invoke(args[0].ToString());
        }
    }

    private void OnGameMode(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("OnGameMode Action =====" + args[0].ToString());

        if (Staticdata.isAdmin)
            UIAdmin.GameModes.Invoke(args[0].ToString());
    }

    private void OnRestartAdmin(Socket socket, Packet packet, object[] args)    // ADMIN AUTO / MANUAL RESTART OPTION CHANGE CALLBACK
    {
        //Debug.Log("OnRestartAdmin Action =====" + args[0].ToString());
        if (Staticdata.isAdmin)
            UIAdmin.Restart.Invoke(args[0].ToString());
    }

    private void OnAdminRejoin(Socket socket, Packet packet, object[] args)
    {
        if (Staticdata.isAdmin)
            GameManger.RejoinAdmin.Invoke(args[0].ToString());
    }

    private void OnCheckStartButton(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("Start button ------" + args[0].ToString());
        if (Staticdata.isAdmin)
        {
            JSONNode data = JSON.Parse(args[0].ToString());
            bool isActive = data["status"].AsBool;
            string warningMsg = data["message"];
            UIAdmin.UPDATESTART?.Invoke(isActive, warningMsg);

        }
    }

    //private void OnConvoyLisenAdmin(Socket socket, Packet packet, object[] args)
    //{

    //    JSONNode data = JSON.Parse(args[0].ToString());
    //    bool isEnable = data["status"].AsBool;
    //    if (Staticdata.isAdmin)
    //    {
    //        UIAdmin.UPDATESTART.Invoke(isEnable);
    //    }
    //}


    //private void OnSetTime(Socket socket, Packet packet, object[] args)
    //{
    //    //Debug.Log("Set Time By Admin Listen =====" + args[0].ToString());
    //}

    #endregion Admin Event Recieved



    #region Event Recieved

    private void OnRoomJoin(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("------ OnRoomJoined ------: " + args[0].ToString());
        UiManager.I.HideLoader();

        JSONNode data = JSON.Parse(args[0].ToString());

        //if (data["playerId"].Equals(PlayerPrefs.GetString("playerId")))
        //{
        //    GameManger.mPlayerID = data["playerId"];
        //    Staticdata.I.mRoomName = PlayerPrefs.GetString("roomCode");
        //    //PLAYERREJOIN?.Invoke();
        //    UiManager.I.RejoinPlayerUI();
        //}
        //else
        //{
        //    GameManger.OnRoomJoined?.Invoke(args[0].ToString());
        //}
        GameManger.OnRoomJoined?.Invoke(args[0].ToString());

    }

    private void OnAutoRoomJoin(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("------ OnRoomAutoJoined ------: " + args[0].ToString());
        UiManager.I.HideLoader();

        JSONNode data = JSON.Parse(args[0].ToString());

        if (data["playerId"].Equals(PlayerPrefs.GetString("playerId")))
        {
            GameManger.mPlayerID = data["playerId"];
            Staticdata.I.mRoomName = PlayerPrefs.GetString("roomCode");
            Staticdata.I.LastRoomId = int.Parse(PlayerPrefs.GetString("roomCode"));
            Staticdata.I.mRoomId = PlayerPrefs.GetString("roomCode");
            UiManager.I.RejoinPlayerUI();
            Staticdata.isRestartScene = true;
            Debug.Log("Rejoin Special case ===   " + data["rejoinPlayer"].AsBool + "  isStart  ---  " + data["isStart"].AsBool);

            if (data["rejoinPlayer"].AsBool && data["isStart"].AsBool)
            {
                Debug.Log("Rejoin Special case ==============rejoinPlayer--->>  " + data["rejoinPlayer"].AsBool + " ----- isStart ---- " + data["isStart"].AsBool);
                GameManger.OnRoomReJoined.Invoke();
                //PLAYERREJOIN?.Invoke();
            }
            else if (!data["isStart"].AsBool)
            {
                Debug.Log("-------------isStartfalse---------------  " + data["isStart"].AsBool);
                PLAYERREJOIN?.Invoke();
            }
        }
    }

    private void OnConvoyJoin(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("------ OnConvoyJoin------ : " + args[0].ToString());

        GameManger.OnConvoyJoined?.Invoke(args[0].ToString());
    }

    private void OnConvoyReselect(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("------ OnConvoyReselect------ : " + args[0].ToString());
        GameManger.OnReSelectConvoy?.Invoke(args[0].ToString());
    }

    private void OnGetOtherPlayerPos(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("******************* OnGetOtherPlayerPos ******************* " + args[0].ToString());
        //if (GameManger.mPlayerID != null)
        //Debug.Log("OnGetOtherPlayerPos ******************* " + args[0].ToString());
        GameManger.GetPosfromServer?.Invoke(args[0].ToString());
    }

    private void GameRoadSet(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("----------- GameRoadSet ------------ " + args[0].ToString());
        JSONNode data = JSON.Parse(args[0].ToString());
        int lRoadID = data["roadId"].AsInt;
        GameManger.SetRoad?.Invoke(lRoadID);
    }

    private void OnGameOverByPlayer(Socket socket, Packet packet, object[] args)
    {
        //Debug.Log("Gameover Listion  &&&&&&  " + args[0].ToString());
        if (!Staticdata.isAdmin)
        {
            if (UiManager.isGameStart)
                UiManager.isGameStart = false;
        }
        //if (LocalStorageData.HasKey("roomCode"))
        //int Sid = SceneManager.GetActiveScene().buildIndex;
        //if (Sid == 1 || Sid == 2)

        if (Staticdata.isAdmin)
        {
            Debug.Log("Admin SETTING call game over new data: ");
            AdminSettings();
        }

        GameManger.GameOverbyOther?.Invoke(args[0].ToString());
    }

    private void OnGameWinByPlayer(Socket socket, Packet packet, object[] args)
    {
        //Debug.Log("GameWin Listion %%%%%%%  " + args[0].ToString());
        //Debug.Log("-------------GameWin Listen ---------  " + args[0].ToString());

        GameManger.GameWinbyOther?.Invoke(args[0].ToString());
    }

    private void PlayerLeft(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("------ PlayeLeftAction  Lisen ------ : " + args[0].ToString());
        // if (GameManger.mPlayerID != null)
        Staticdata.AdminSetting = args[0].ToString();
        if (Staticdata.isAdmin)
        {
            //AdminDataSetup.UpdateSettingPlayerLeft.Invoke();
            AdminDataSetup.UpdateSettingPlayerLeft.Invoke();

            GameManger.RejoinAdmin.Invoke(args[0].ToString());
            return;
        }

        GameManger.PlayerLeft?.Invoke(args[0].ToString());
    }

    private void PlayerRejoin(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("------ Player Rejoin Lisen ------ : " + args[0].ToString());
        GameManger.PlayerRejoin?.Invoke(args[0].ToString());

        if (Staticdata.isAdmin)
            AdminSettings();
    }

    private void Restart(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("------------ OnRestart Lisen HERE ----------- " + args[0].ToString());
        //if (GameManger.mPlayerID == null || GameManger.mPlayerID == "") //  || Staticdata.I.LastRoomId != 0  || mRoomID

        if (!Staticdata.isAdmin)
        {
            if (Staticdata.I.LastRoomId == 0) //  || Staticdata.I.LastRoomId != 0  || mRoomID
                return;

            if (!UiManager.I.Connected.activeInHierarchy)  // FOR REJOIN PLAYE NOT USE THIS
            {
                if (string.IsNullOrEmpty(Staticdata.mGameTeam)) // rejoin player doesnt have room code then 
                {
                    return;
                }
                //Debug.Log("Restart listen and go ahed------------------ ");
            }
        }

        JSONNode data = JSON.Parse(args[0].ToString());
        if (Staticdata.isAdmin)
        {
            Debug.Log("json RoomCode " + data["roomCode"] + "  AdminData Setup room Code :: " + AdminDataSetup.roomCode);
            //if (data["roomCode"] != AdminDataSetup.roomCode)
            if (data["message"].ToString().Length > 5)
            {
                Debug.Log(":::  At Restart listen ::: Warning " + data["message"]);
                //Debug.Log(":::  At Restart listen ::: Any one team has only one player to play game");
                UIAdmin.WarningGamePlay.Invoke(data["message"]);
                return;
            }

        }
        else if (!Staticdata.isAdmin)
        {
            if (data["roomCode"] != Staticdata.I.mRoomName)
            {
                UiManager.I.WarningMessage(data["message"]);
                Debug.Log(":::  At Restart listen ::: Any one team has only one player to play game");
                return;
            }
        }

        GameManger.OnRestart?.Invoke(args[0].ToString());

    }

    private void JoinFailed(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("*JoinFailed Lisen HERE * " + args[0].ToString());
        if (!AutoJoinRoom)
            GameManger.OnFailedJoin?.Invoke(args[0].ToString());
    }

    private void OnWarningMessage(Socket socket, Packet packet, object[] args)
    {
        //Debug.LogError("****************** OnWarningMessage Listion  ******************  " + args[0].ToString());
        PlayerControler.MessageAction?.Invoke(args[0].ToString());
    }


    #endregion


    public static bool HasCheckReloaded()
    {
        Debug.Log("HasCheckReloaded call :: ");
        return (pageReloadCheck() == 1);
    }

    [DllImport("__Internal")]
    private static extern int pageReloadCheck();


}