using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;
using TMPro;
using System.Collections;

public class GameManger : MonoBehaviour
{
    public static Action<Transform> SendPlayerData;

    public static Action<string> StartGame, AdminRestart, Timer, RejoinAdmin;  // ADMIN START GAME LISTEN

    public static Action<string> GetPosfromServer, GameOverbyOther, PlayerLeft, PlayerRejoin, GameWinbyOther; //PlayerReadyEvent  GetJoitBreak, JointBreak
    public static Action<int> SetRoad;
    public static Action<string> OnRoomJoined, OnConvoyJoined, OnReSelectConvoy;
    public static Action OnRoomReJoined;


    public static Action StartPlay, EndPlay;
    public static Action<string> OnRestart;
    public static Action<string> OnFailedJoin;

    public Cinemachine.CinemachineVirtualCamera vcamera;

    public List<PlayerInfo> playersPrefab;
    [SerializeField]
    public List<PlayerInfo> playerInfos;

    public Transform playerOffset;

    public Transform you;

    public PlayerInfo Owner = null;

    public TeamData teamData;

    public GameObject playerParent;

    public PlayerControler playerControler;

    public static string mPlayerID = null;

    public static bool isGameStart = false;

    [Header("Count Down")]
    public TextMeshProUGUI CountDownOver;
    //public TextMeshProUGUI CountDownWin;

    [Header("Game Difficulty")]
    public GameObject[] Medium;
    public GameObject[] Hard;
    public int FixRoadID = 0;


    private void OnEnable()
    {

        StartGame += OnGameStart;
        AdminRestart += AdminManualyRestart;
        Timer += GameTimer;
        RejoinAdmin += OnAdminRejoin;


        OnFailedJoin += JoinRoomFailed;
        SocketConnector.JOINROOM += SetRoomNumber;
        OnRoomJoined += OnJoinRoomLisen;
        OnRoomReJoined += OnRejoine;
        OnConvoyJoined += OnJoinConvoyLisen;
        OnReSelectConvoy += OnReselectConvoyLisen;
        StartPlay += StartPlayGame;
        GetPosfromServer += GetDataFromServer;
        SendPlayerData += SendDataToServer;
        GameOverbyOther += GameOverByOtherPlayer;
        GameWinbyOther += GameWinByOtherPlayer;
        SetRoad += SetRoadID;

        //JointBreak += DisconnectJoint;
        PlayerLeft += OnPlayerLeft;
        PlayerRejoin += PlayerReJoin;
        OnRestart += OnRestartListen;
        //PlayerReadyEvent += OnPlayerReady;
        //GetJoitBreak += GetJointBreakRecived;
    }

    private void OnDisable()
    {
        StartGame -= OnGameStart;
        AdminRestart -= AdminManualyRestart;
        Timer -= GameTimer;
        RejoinAdmin -= OnAdminRejoin;

        OnFailedJoin -= JoinRoomFailed;
        SocketConnector.JOINROOM -= SetRoomNumber;
        OnRoomJoined -= OnJoinRoomLisen;
        OnRoomReJoined -= OnRejoine;
        OnConvoyJoined -= OnJoinConvoyLisen;
        OnReSelectConvoy -= OnReselectConvoyLisen;
        StartPlay -= StartPlayGame;
        GetPosfromServer -= GetDataFromServer;
        SendPlayerData -= SendDataToServer;
        GameOverbyOther -= GameOverByOtherPlayer;
        GameWinbyOther -= GameWinByOtherPlayer;
        SetRoad -= SetRoadID;

        //JointBreak -= DisconnectJoint;
        PlayerLeft -= OnPlayerLeft;
        PlayerRejoin -= PlayerReJoin;
        OnRestart -= OnRestartListen;
        //PlayerReadyEvent -= OnPlayerReady;
        //GetJoitBreak -= GetJointBreakRecived;

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Debug.Log("Delete All PlayerPrefs");
            PlayerPrefs.DeleteAll();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("PlayerID: " + mPlayerID);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Playerprefs PlayerID: " + PlayerPrefs.GetString("playerId"));
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Playerprefs RoomCode: " + PlayerPrefs.GetString("roomCode"));
        }
    }



    private void OnJoinRoomLisen(string data)
    {
        Debug.Log("OnJoinRoomLisen data : " + data.ToString());

        JSONNode jData = JSON.Parse(data);
        mPlayerID = jData["playerId"];

        if (Staticdata.I.LastRoomId == 0) return;
        UiManager.I?.ChooseConvoy();
        AudioManager.I?.WaitingForReadyPlay();
    }

    private void OnRejoine()
    {
        if (UiManager.I.Connected.activeInHierarchy)  // FOR REJOIN PLAYER USE THIS
        {
            Debug.Log("RestRT gAME ----------------------");
            Staticdata.I.IsAutoRestart = true;
            Staticdata.isRestartScene = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }


    private void OnJoinConvoyLisen(string data)// for join convay
    {
        JSONNode jData = JSON.Parse(data);

        Debug.Log("JOIN OnJoinConvoyLisen LISTEN : " + data.ToString());
        //Debug.Log("jData[playerId] : " + jData["playerId"]);
        //Debug.Log("jData[status] : " + jData["status"]);
        //Debug.Log("status : " + jData["status"].AsBool);


        if (jData["status"].AsBool == false && jData["playerId"] == mPlayerID)
        {
            Debug.Log("Return and mess : " + jData["message"]);
            UiManager.I?.ConvoyMessage(jData["message"]);
            return;
        }
        if (jData["status"].AsBool == false)
        {
            Debug.Log("Simple Return without mess other player lisen : " + jData["message"]);
            return;
        }

        Debug.Log("mode : " + jData["mode"]);
        Debug.Log("auto : " + jData["auto"]);
        Debug.Log("manual : " + jData["manual"]);
        Debug.Log("timer : " + jData["timer"]);
        Debug.Log("totalPlayer : " + jData["totalPlayer"]);
        Debug.Log("totalTeam : " + jData["totalTeam"]);

        Staticdata.I.IsAutoRestart = jData["auto"].AsBool;
        Staticdata.mGameMode = jData["mode"];
        Staticdata.mGameTime = jData["timer"];


        if (Staticdata.isAdmin)
        {
            Debug.Log("totalPlayer : " + jData["totalPlayer"]);
            Debug.Log("totalTeam : " + jData["totalTeam"]);
            string tplayer = jData["totalPlayer"];
            string tteam = jData["totalTeam"];
            UIAdmin.PTData.Invoke(tplayer, tteam);
        }


        MyPlayers allPlayers = JsonConvert.DeserializeObject<MyPlayers>(data);

        Debug.Log("MyPlayers allPlayers data   =====  " + allPlayers.playerData.Count);
        Debug.Log("mplaeyrID   =====  " + mPlayerID);

        if (!Staticdata.isAdmin)
        {
            if (Staticdata.I.LastRoomId == 0)
            {
                Debug.Log("Join Convoy Last room id is 0 and return");
                return;
            }
        }

        foreach (PlayerData pl in allPlayers.playerData)
        {
            Debug.Log("pl.playerID   =====  " + pl.playerId);

            Debug.Log("playerInfos list   =====  " + playerInfos.Count);

            if (!playerInfos.Exists(x => x.ID == pl.playerId))
            {
                Vector3 pos = teamData.teamDetails[pl.teamCode].Teams[pl.position].PlayerPosition;
                Vector3 rot = teamData.teamDetails[pl.teamCode].Teams[pl.position].PlayerRotation;
                PlayerInfo player = Instantiate(playersPrefab[pl.teamCode % 4], pos, Quaternion.Euler(rot), playerParent.transform);
                player.transform.eulerAngles = rot;
                player.gameObject.name = "Car " + pl.teamCode + " + " + pl.position;
                Debug.Log("Team: " + pl.teamCode + " Pos: " + pl.position + " PosV: " + pos + " Rot: " + rot);
                player.setDetails(pl.playerId, allPlayers.roomCode, pl.position, pl.teamCode);

                try
                {
                    if (pl.position > 0)
                    {
                        Debug.Log("pl.position :  " + pl.position);
                        //player.SetJoint(playerInfos[playerInfos.Count - 1].gameObject);

                        if (playerInfos.FindLast(x => x.GroupName == pl.teamCode))
                        {
                            player.SetJoint(playerInfos[playerInfos.FindLastIndex(x => x.GroupName == pl.teamCode)].gameObject);
                        }
                        //player.SetJoint(playerInfos[playerInfos.Count - 1].gameObject);
                    }
                }
                catch
                {
                    Debug.Log("Handling exception on player connected");
                }
                playerInfos.Add(player);

                //Debug.Log("Add Player :  " + player.ID + " Name" + player.name);
                //Debug.Log("Owner :  " + Owner?.ID);
                Debug.Log("pl.isOwn :  " + pl.isOwn);

                if (Staticdata.isAdmin && Owner == null)
                {
                    Owner.ID = "0";
                    return;
                }

                if (Owner == null && pl.isOwn && mPlayerID == pl.playerId)
                {
                    Staticdata.mGameTeam = jData["totalTeam"];

                    //PlayerPrefs.SetString("playerId", mPlayerID);
                    PlayerPrefs.SetString("playerId", pl.playerId + "");
                    PlayerPrefs.SetString("roomCode", Staticdata.I.mRoomName);
                    //PlayerPrefs.SetString("teamCode", pl.teamCode + "");

                    UiManager.I?.ClientConnected();
                    AudioManager.I?.WaitingForReadyPlay();

                    Debug.Log("Add Player :  Convoy " + player.ID + " Name" + player.name);
                    //mPlayerID = pl.playerId;
                    Owner = player;
                    player.GetComponent<PlayerControler>().enabled = true;
                    player.GetComponentInChildren<CircleCollider2D>().enabled = true;
                    playerControler = player.GetComponent<PlayerControler>();
                    playerControler.setdetails(vcamera, playerOffset, you.transform, this, player, rot);
                }
            }
        }
    }


    private void OnReselectConvoyLisen(string obj)
    {
        if (Staticdata.isAdmin)
        {
            JSONNode jsondata = JSON.Parse(obj);
            string tp = jsondata["totalPlayer"];
            string tt = jsondata["totalTeam"];
            Debug.Log("tp : " + tp);
            Debug.Log("tt : " + tt);
            UIAdmin.PTData.Invoke(tp, tt);
            Debug.Log("OnReselectConvoyLisen  and return :");
            return;
        }

        JSONNode data = JSON.Parse(obj);
        Debug.Log("OnReselectConvoyLisen : " + data.ToString());
        Debug.Log("mPlayerID : " + mPlayerID);
        Debug.Log("data[playerId] : " + data["playerId"]);
        Debug.Log("mPlayerID : " + mPlayerID);

        if (data["playerId"].Equals(mPlayerID))
        {
            Debug.Log("Self player UI reselct ui and return: ");
            you.transform.parent = null;
            foreach (Transform item in playerParent.transform)
            {
                Destroy(item.gameObject);
            }
            playerInfos.Clear();
            Owner = null;
            UiManager.I?.ChooseConvoy();
            return;
        }
        else
        {
            PlayerReJoin(obj);
        }
        //SocketConnector.PLAYERREJOIN?.Invoke();
    }


    private void JoinRoomFailed(string mess)
    {
        JSONNode data = JSON.Parse(mess);
        UiManager.I?.JoinRoomFailed(data[0]);
    }

    private void SetRoomNumber(string obj)
    {
        Debug.Log("Set room number : " + obj.ToString());
        Staticdata.I.mRoomId = obj;
        Staticdata.I.mRoomName = obj;
        Staticdata.I.LastRoomId = int.Parse(obj);
        PlayerPrefs.SetString("roomCode", obj);
    }


    private void StartPlayGame()
    {
        if (playerControler != null)
        {
            isGameStart = true;
            playerControler.PlayerStart();
        }

    }


    private void OnPlayerLeft(string obj)
    {
        JSONNode data = JSON.Parse(obj);
        Debug.Log("OnPlayerLeft Action : is game start::  " + UiManager.isGameStart);

        if (!Staticdata.isAdmin)
        {
            PlayerInfo player = playerInfos.Find(x => x.ID == data["playerId"]);
            Destroy(player.gameObject);
            playerInfos.Remove(player);
        }
        //GameOverbyOther?.Invoke(obj.ToString());

        if (!Staticdata.isAdmin)
        {
            if (!isGameStart && UiManager.I.Lose.activeInHierarchy)
                return;
        }

        if (isGameStart)
        {
            Debug.Log("Player left and game started :: Game over call");
            GameOverbyOther?.Invoke(obj.ToString());
        }
        else if (!Staticdata.isAdmin)
        {
            Debug.Log("Player left and game not started :: PLAYERREJOIN call");
            SocketConnector.PLAYERREJOIN?.Invoke();
        }

    }


    private void PlayerReJoin(string obj)
    {
        if (!Staticdata.isAdmin && Owner == null && !SocketConnector.AutoJoinRoom)
        {
            Debug.Log("PlayerReJoin return owner is null : ");
            return;
        }
        JSONNode jsondata = JSON.Parse(obj);


        //JSONNode jsondata = JSON.Parse(obj);
        JSONNode _playerData = jsondata["playerData"];

        Debug.Log("PlayerReJoin data when PlayerReJoin  =====  " + jsondata.ToString());
        Debug.Log("_playerData  =====  " + _playerData.ToString());
        Debug.Log("Staticdata.I.mRoomName : " + Staticdata.I.mRoomName);


        Debug.Log("mode : " + jsondata["mode"]);
        Debug.Log("auto : " + jsondata["auto"]);
        Debug.Log("manual : " + jsondata["manual"]);
        Debug.Log("timer : " + jsondata["timer"]);
        Debug.Log("totalPlayer : " + jsondata["totalPlayer"]);
        Debug.Log("totalTeam : " + jsondata["totalTeam"]);

        Staticdata.I.IsAutoRestart = jsondata["auto"].AsBool;
        Staticdata.mGameMode = jsondata["mode"];
        Staticdata.mGameTime = jsondata["timer"];
        Staticdata.mGameTeam = jsondata["totalTeam"];

        //if (!Staticdata.isAdmin)

        //{
        //    if (jsondata["roomCode"] != Staticdata.I.mRoomName)
        //    {
        //        Debug.Log("Room Not Match and return when PlayerReJoin Listen ");
        //        return;
        //    }
        //}

        you.transform.parent = null;

        foreach (Transform item in playerParent.transform)
        {
            Destroy(item.gameObject);
        }
        playerInfos.Clear();


        if (!Staticdata.isAdmin)
        {
            //UiManager.I?.RejoinPlayerUI();

            if (!Staticdata.isAdmin)
                AudioManager.I?.WaitingForReadyPlay();
        }


        for (int i = 0; i < _playerData.Count; i++)
        {

            if (playerInfos.Find(x => x.ID == _playerData[i]["playerId"])) continue;

            //Debug.Log("_playerData[playerId].Value  MIN TTOTAL PLAYER TIME TIME CALL:  " + _playerData[i]["playerId"].Value);

            Vector3 pos = teamData.teamDetails[_playerData[i]["teamCode"]].Teams[_playerData[i]["position"]].PlayerPosition;
            Vector3 rot = teamData.teamDetails[_playerData[i]["teamCode"]].Teams[_playerData[i]["position"]].PlayerRotation;
            //Debug.Log("PlayerTeam code when rejoin: " + _playerData[i]["teamCode"]);
            PlayerInfo player = Instantiate(playersPrefab[_playerData[i]["teamCode"].AsInt % 4], pos, Quaternion.Euler(rot), playerParent.transform);
            player.gameObject.name = "Car " + _playerData[i]["teamCode"] + " + " + _playerData[i]["position"];
            //Debug.LogWarning("Team: " + _playerData[i]["teamCode"] + " Pos: " + _playerData[i]["position"] + " PosV: " + pos + " Rot: " + rot);

            player.setDetails(_playerData[i]["playerId"], jsondata["roomCode"], _playerData[i]["position"], _playerData[i]["teamCode"]);

            try
            {
                //Debug.Log("playerInfos count:  " + playerInfos.Count);
                if (playerInfos.Count + 1 >= 1)    //noOfListPlayerJson
                {
                    if (playerInfos.FindLast(x => x.GroupName == _playerData[i]["teamCode"].AsInt))
                    {
                        player.SetJoint(playerInfos[playerInfos.FindLastIndex(x => x.GroupName == _playerData[i]["teamCode"].AsInt)].gameObject);
                    }
                }
            }
            catch
            {
                Debug.Log("Handling exception on PlayerReJoin joints creation issue ");
            }
            playerInfos.Add(player);


            if (Staticdata.isAdmin) continue;

            if (_playerData[i]["playerId"] == mPlayerID)
            {
                Debug.Log("Add Player PlayerReJoin :  " + player.ID + " Name" + player.name);
                mPlayerID = _playerData[i]["playerId"].Value;
                Owner = player;
                player.GetComponentInChildren<CircleCollider2D>().enabled = true;
                player.GetComponent<PlayerControler>().enabled = true;
                playerControler = player.GetComponent<PlayerControler>();
                playerControler.setdetails(vcamera, playerOffset, you.transform, this, player, rot);

            }
        }



    }


    private void OnGameStart(string obj)
    {
        JSONNode data = JSON.Parse(obj);
        Debug.Log("Game Start listen : " + data.ToString());

        if (!Staticdata.isAdmin)
        {
            if (Owner == null)
            {
                Debug.Log("Returned those player who didnt choice convoy!!:");
                return;
            }
            Debug.Log("OnGameStart data[roomCode] : " + data["roomCode"]);
            Debug.Log("OnGameStart Staticdata.I.LastRoomId : " + Staticdata.I.LastRoomId.ToString());

            if (data["roomCode"] != Staticdata.I.LastRoomId.ToString())
            {
                Debug.Log("OnGameStart return room not match : ");
                return;
            }

            if (data["isGameStart"].AsBool == true)
            {
                Debug.Log("Game Start Lisen");
                UiManager.I?.StartPlayGame();
            }


        }

        //foreach (var item in Medium)
        //    item.SetActive(false);
        //foreach (var item in Hard)
        //    item.SetActive(false);

        //if (Staticdata.mGameMode == "easy")
        //{
        //    Debug.Log("Easy Game Road");
        //}
        //else if (Staticdata.mGameMode == "medium")
        //{
        //    Debug.Log("Medium Game Road Set " + FixRoadID);
        //    Medium[FixRoadID].SetActive(true);
        //}
        //else if (Staticdata.mGameMode == "hard")
        //{
        //    Debug.Log("Hard Game Road Set " + FixRoadID);
        //    Hard[FixRoadID].SetActive(true);
        //}

    }

    private void GameTimer(string obj)
    {
        JSONNode data = JSON.Parse(obj);
        //Debug.Log("Game Timer Lisen Data  : " + data.ToString());

        int TRemainSec = data["time"];
        string message = data["message"];
        bool status = data["status"].AsBool;

        if (!Staticdata.isAdmin)
        {
            UiManager.I.GameTimer(TRemainSec);

            if (TRemainSec == 0)
                Debug.Log("status : " + status + " message : " + message);

            if (status)
            {
                Debug.Log("Time up and game over");
                SocketConnector.GAMEOVERME?.Invoke(mPlayerID, Staticdata.I.LastRoomId.ToString(), "1");
            }

        }
        else if (Staticdata.isAdmin)
        {
            UIAdmin.TimeGet.Invoke(TRemainSec);
        }
    }

    private void OnAdminRejoin(string data)
    {

        JSONNode jsondata = JSON.Parse(data);
        JSONNode _playerDetails = jsondata["playerDetails"];
        Debug.Log("On Admin Rejoin lisen data  : " + jsondata.ToString());
        //JSONNode playerDataTeams = jsondata["playerData"];

        //Debug.Log("On Admin Rejoin Lisen Data");

        // when game mode now easy and game started -> after change mode to medium or hard and admin call rejoin then admin view medium or hard mode

        if (_playerDetails.Count == 0) return;
        var cmode = jsondata["currentGameMode"];

        foreach (var item in Medium)
            item.SetActive(false);
        foreach (var item in Hard)
            item.SetActive(false);

        Debug.Log("Current Game Mode : " + cmode);
        if (cmode == "easy")
        {
            Debug.Log("Easy Game Road");
        }
        else if (cmode == "medium")
        {
            Debug.Log("Medium Game Road Set " + FixRoadID);
            Medium[FixRoadID].SetActive(true);
        }
        else if (cmode == "hard")
        {
            Debug.Log("Hard Game Road Set " + FixRoadID);
            Hard[FixRoadID].SetActive(true);
        }


        for (int i = 0; i < _playerDetails.Count; i++)
        {
            if (playerInfos.Find(x => x.ID == _playerDetails[i]["playerId"])) continue;

            Debug.Log("_playerData[playerId].Value  MIN TTOTAL PLAYER TIME TIME CALL:  " + _playerDetails[i]["playerId"].Value);

            float xpos = jsondata["playerDetails"][i]["carPosition"]["x"];
            float ypos = jsondata["playerDetails"][i]["carPosition"]["y"];
            float zpos = jsondata["playerDetails"][i]["carPosition"]["z"];

            float xrot = jsondata["playerDetails"][i]["rotation"]["x"];
            float yrot = jsondata["playerDetails"][i]["rotation"]["y"];
            float zrot = jsondata["playerDetails"][i]["rotation"]["z"];

            Vector3 pos = new Vector3(xpos, ypos, zpos);
            Vector3 rot = new Vector3(xrot, yrot, zrot);
            Debug.Log("pos: " + pos);
            if (xpos == 0 && ypos == 0)
            {
                Debug.Log("position null or x and y both 0 : ");
                continue;
            }
            //Debug.Log("teamCode: " + _playerDetails[i]["teamCode"]);
            //Debug.Log("position: " + _playerDetails[i]["position"]);

            //PlayerInfo player = Instantiate(playersPrefab[_playerData[i]["teamCode"].AsInt % 4], pos, Quaternion.Euler(rot), playerParent.transform);
            PlayerInfo player = Instantiate(playersPrefab[_playerDetails[i]["teamCode"].AsInt % 4], pos, Quaternion.Euler(rot), playerParent.transform);
            player.gameObject.name = "Car " + _playerDetails[i]["teamCode"] + " + " + _playerDetails[i]["position"];

            player.setDetails(_playerDetails[i]["playerId"], jsondata["roomCode"], _playerDetails[i]["position"], _playerDetails[i]["teamCode"]);

            try
            {
                if (playerInfos.Count + 1 >= 1)    //noOfListPlayerJson
                {
                    //Debug.Log("Group name:::  " + playerInfos[playerInfos.Count - 1].gameObject.GetComponent<PlayerInfo>().GroupName);
                    //Debug.Log("GetServer team code :  " + _playerData[i]["teamCode"]);

                    if (playerInfos.FindLast(x => x.GroupName == _playerDetails[i]["teamCode"].AsInt))
                    {
                        player.SetJoint(playerInfos[playerInfos.FindLastIndex(x => x.GroupName == _playerDetails[i]["teamCode"].AsInt)].gameObject);
                    }

                    //if (teamData[i]["teamCode"] == playerInfos[playerInfos.Count - 1].gameObject.GetComponent<PlayerInfo>().GroupName)
                    //    player.SetJoint(playerInfos[playerInfos.Count - 1].gameObject);
                }
            }
            catch
            {
                Debug.Log("Handling exception on admin rejoin  joint issue ");
            }
            playerInfos.Add(player);

        }



    }

    public void SetRoadID(int lRoadID)
    {
        FixRoadID = lRoadID;
        foreach (var item in Medium)
            item.SetActive(false);
        foreach (var item in Hard)
            item.SetActive(false);

        if (Staticdata.mGameMode == "easy")
        {
            Debug.Log("Easy Game Road");
        }
        else if (Staticdata.mGameMode == "medium")
        {
            Debug.Log("Medium Game Road Set--------------- " + FixRoadID);
            Medium[FixRoadID].SetActive(true);
        }
        else if (Staticdata.mGameMode == "hard")
        {
            Debug.Log("Hard Game Road Set----------------- " + FixRoadID);
            Hard[FixRoadID].SetActive(true);
        }
    }

    private void GameOverByOtherPlayer(string obj)
    {
        foreach (var item in Medium)
            item.SetActive(false);
        foreach (var item in Hard)
            item.SetActive(false);

        isGameStart = false;

        JSONNode data = JSON.Parse(obj);
        Debug.Log("Game over by other player Lisen :  " + data.ToString());


        //if (Staticdata.isAdmin)
        //{
        //    Debug.Log("GameOver In game manager lisen adn send adminUI : " + data["reason"].AsInt);
        //    UIAdmin.LoseUI(data["reason"].AsInt);
        //}

        if (!Staticdata.isAdmin)
        {
            if (UiManager.I.Connected.activeInHierarchy) return;

            UiManager.isGameStart = false;
            Staticdata.I.IsAutoRestart = data["auto"].AsBool;
            Staticdata.mGameMode = data["mode"];
            Staticdata.mGameTime = data["timer"];

            if (mPlayerID == null || mPlayerID == "")
            {
                Debug.Log("mPlayerId Null and return");
                return;
            }

            if (data["roomName"].AsInt != int.Parse(Staticdata.I.mRoomName))
            {
                Debug.Log("Game Over Room missMatch  and return: ");
                return;
            }


            AudioManager.I?.loseGamePlay();
            if (data["reason"].AsInt < 5)
            {
                Debug.Log("UI Update reson is :  " + data["reason"].AsInt);
                UiManager.I?.gameOverReasonSet(data["reason"].AsInt);
            }

            playerControler?.GameOverEffect();


        }

        playerInfos.Remove(playerInfos.Find(x => x.ID == Owner.ID));
        Destroy(playerInfos.Find(x => x.ID == Owner.ID));
        you.transform.parent = null;
        foreach (Transform item in playerParent.transform)
        {
            //Debug.Log("Destroy All player : " + item.name);
            Destroy(item.gameObject);
        }
        playerInfos.Clear();


        if (!Staticdata.isAdmin)
        {
            if (Staticdata.I.IsAutoRestart)
                StartCoroutine(AutoRestartGame());
            else
                CountDownOver.text = "";
        }

    }

    private void GameWinByOtherPlayer(string obj)
    {

        //foreach (var item in Medium)
        //    item.SetActive(false);
        //foreach (var item in Hard)
        //    item.SetActive(false);

        isGameStart = false;
        JSONNode data = JSON.Parse(obj);
        //Debug.Log("WIN DATA LISEN data : " + data.ToString());
        //if (data[Staticdata.I.roomId].AsInt != Owner.RoomID)
        //    return;
        Debug.Log("data[status] " + data["status"]);
        //Debug.Log("data[playerId] " + data["playerId"]);
        //if (data["status"].ToString() == "false")
        if (data["status"].AsBool)
        {
            //if (Staticdata.isAdmin)
            //    UIAdmin.WinUI.Invoke();
            Debug.Log("sss");
            if (!Staticdata.isAdmin)
            {
                if (UiManager.isGameStart)
                    UiManager.isGameStart = false;
            }

            //Debug.Log("WIN GAME");
            UiManager.isGameStart = false;
            AudioManager.I?.Win();
            playerControler?.GameWinEffect();
            //StartCoroutine(AutoRestartGame());
        }

    }

    private void AdminManualyRestart(string obj)
    {
        if (string.IsNullOrEmpty(Staticdata.mGameTeam))  // Remove for rejoin player havent select convoy and dont have team code
        {
            return;
        }
        JSONNode data = JSON.Parse(obj);
        Debug.Log("AdminManualyRestart obj : " + data.ToString());


        if (data["roomCode"] == Staticdata.I.mRoomName)
        {
            StartCoroutine(AutoRestartGame());
        }

    }

    IEnumerator AutoRestartGame()  // Same as On Click Button Restart Click
    {

        CountDownOver.gameObject.SetActive(true);
        for (int i = 5; i >= 0; i--)
        {
            CountDownOver.text = i.ToString();
            //CountDownWin.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        Staticdata.I.IsAutoRestart = true;
        CountDownOver.text = "";
        CountDownOver.gameObject.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void OnRestartListen(string data)
    {
        JSONNode jsondata = JSON.Parse(data);
        JSONNode _playerData = jsondata["playerData"];
        Debug.Log("OnRestertListen data when restart  =====  " + jsondata.ToString());
        Debug.Log("Staticdata.I.mRoomName : " + Staticdata.I.mRoomName);

        if (Staticdata.isAdmin)
        {
            if (jsondata["totalPlayer"] != null)
            {
                string tplayer = jsondata["totalPlayer"];
                string tteam = jsondata["totalTeam"];
                UIAdmin.PTData.Invoke(tplayer, tteam);
            }
        }



        if (!Staticdata.isAdmin)
        {
            if (UiManager.I.Connected.activeInHierarchy && Staticdata.isRestartScene)  // FOR REJOIN PLAYER USE THIS
            {
                Staticdata.I.IsAutoRestart = true;
                Staticdata.isRestartScene = false;
                //StartCoroutine(AutoRestartGame());
                if (SocketConnector.AutoJoinRoom)
                {
                    UiManager.I.StartPlayGame();
                }

                if (!SocketConnector.AutoJoinRoom)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                }
            }

            if (jsondata["roomCode"] != Staticdata.I.mRoomName)
            {
                Debug.Log("Room Not Match and return when restart Listen ");
                return;
            }
        }

        you.transform.parent = null;
        foreach (Transform item in playerParent.transform)
        {
            //Debug.Log("Destroy All player : " + item.name);
            Destroy(item.gameObject);
        }
        playerInfos.Clear();
        //UiManager.I?.ReadyInRoom(); // Forcefully ready UI show
        if (!Staticdata.isAdmin)
            AudioManager.I?.WaitingForReadyPlay();

        //Debug.Log("jsondata[totalPlayer] : " + jsondata["totalPlayer"]);

        var jtotalPlayer = jsondata["totalPlayer"];

        //Debug.Log("jsondata[].AsArray.Count : " + jsondata["playerData"].AsArray.Count);
        Debug.Log("totalPlayer save count : " + jtotalPlayer);
        Debug.Log("jsondata[playerData].Count : " + jsondata["playerData"].Count);
        Debug.Log("totalPlayer save count asInt : " + jsondata["totalPlayer"].AsInt);
        //if (jsondata["playerData"].Count != int.Parse(jtotalPlayer))

        if (jsondata["playerData"].Count != jsondata["totalPlayer"].AsInt)
        {
            Debug.Log("RETURN IN RESTART HERE : ");
            return;
        }


        for (int i = 0; i < _playerData.Count; i++)
        {

            if (playerInfos.Find(x => x.ID == _playerData[i]["playerId"])) continue;

            Debug.Log("_playerData[playerId].Value  MIN TTOTAL PLAYER TIME TIME CALL:  " + _playerData[i]["playerId"].Value);

            Vector3 pos = teamData.teamDetails[_playerData[i]["teamCode"]].Teams[_playerData[i]["position"]].PlayerPosition;
            Vector3 rot = teamData.teamDetails[_playerData[i]["teamCode"]].Teams[_playerData[i]["position"]].PlayerRotation;
            Debug.Log("PlayerTeam code when rejoin: " + _playerData[i]["teamCode"]);
            PlayerInfo player = Instantiate(playersPrefab[_playerData[i]["teamCode"].AsInt % 4], pos, Quaternion.Euler(rot), playerParent.transform);
            player.gameObject.name = "Car " + _playerData[i]["teamCode"] + " + " + _playerData[i]["position"];
            //Debug.LogWarning("Team: " + _playerData[i]["teamCode"] + " Pos: " + _playerData[i]["position"] + " PosV: " + pos + " Rot: " + rot);

            player.setDetails(_playerData[i]["playerId"], jsondata["roomCode"], _playerData[i]["position"], _playerData[i]["teamCode"]);

            try
            {
                //Debug.Log("playerInfos count:  " + playerInfos.Count);
                if (playerInfos.Count + 1 >= 1)    //noOfListPlayerJson
                {
                    //Debug.Log("Group name:::  " + playerInfos[playerInfos.Count - 1].gameObject.GetComponent<PlayerInfo>().GroupName);
                    //Debug.Log("GetServer team code :  " + _playerData[i]["teamCode"]);

                    if (playerInfos.FindLast(x => x.GroupName == _playerData[i]["teamCode"].AsInt))
                    {
                        player.SetJoint(playerInfos[playerInfos.FindLastIndex(x => x.GroupName == _playerData[i]["teamCode"].AsInt)].gameObject);
                    }

                    //if (_playerData[i]["teamCode"] == playerInfos[playerInfos.Count - 1].gameObject.GetComponent<PlayerInfo>().GroupName)
                    //    player.SetJoint(playerInfos[playerInfos.Count - 1].gameObject);
                }
            }
            catch
            {
                Debug.Log("Handling exception on restart joints creation issue ");
            }
            playerInfos.Add(player);


            if (Staticdata.isAdmin) continue;

            if (_playerData[i]["playerId"] == mPlayerID)
            {
                Debug.Log("Add Player Restart :  " + player.ID + " Name" + player.name);
                //mPlayerID = _playerData[i]["playerId"].Value;
                Owner = player;

                player.GetComponentInChildren<CircleCollider2D>().enabled = true;
                player.GetComponent<PlayerControler>().enabled = true;
                playerControler = player.GetComponent<PlayerControler>();
                playerControler.setdetails(vcamera, playerOffset, you.transform, this, player, rot);
            }
        }

        //Staticdata.mGameMode = jsondata["mode"];
        //foreach (var item in Medium)
        //item.SetActive(false);
        //foreach (var item in Hard)
        //item.SetActive(false);

        //if (Staticdata.mGameMode == "easy")
        //{
        //    Debug.Log("Easy Game Road Set ");
        //}
        //else if (Staticdata.mGameMode == "medium")
        //{
        //    Debug.Log("Medium Game Road Set " + FixRoadID);
        //    Medium[FixRoadID].SetActive(true);
        //}
        //else if (Staticdata.mGameMode == "hard")
        //{
        //    Debug.Log("Hard Game Road Set " + FixRoadID);
        //    Hard[FixRoadID].SetActive(true);
        //}


        if (Staticdata.isAdmin)
        {
            isGameStart = true;
        }

        Debug.Log("RESTART BY PLAYER FORCE CALL MANUAL OR AUTO");

        if (mPlayerID != null && jsondata["status"].AsBool)
            UiManager.I?.StartPlayGame();

        //if (jsondata["auto"].AsBool == true)
        //{
        //    if (mPlayerID != null)
        //        UiManager.I?.StartPlayGame();
        //}
        //else
        //{
        //    Debug.Log("Restart game by player : STARTGAME CALLED");
        //    SocketConnector.STARTGAME?.Invoke(Staticdata.I.mRoomName);
        //}

    }


    private void SendDataToServer(Transform pt)
    {
        PlayerDetails playerdetails = new PlayerDetails()
        {
            //playerId = Staticdata.I.mPlayerID,
            playerId = Owner.ID,
            position = new Position()
            {
                x = pt.position.x,
                y = pt.position.y,
                z = pt.position.z,
            },
            rotation = new Rotation()
            {
                x = pt.rotation.eulerAngles.x,
                y = pt.rotation.eulerAngles.y,
                z = pt.rotation.eulerAngles.z,
            },
            scale = new Scale()
            {
                x = pt.localScale.x,
                y = pt.localScale.y,
                z = pt.localScale.z,
            },
        };

        string senddata = JsonConvert.SerializeObject(playerdetails);
        //Debug.Log("Send updated car position Data :  " + senddata);
        //Debug.Log("Send updated car Data player : " + Staticdata.I.playerId);
        SocketConnector.SENDPOSITION?.Invoke(senddata);
    }

    //private void DisconnectJoint(string id)
    //{
    //    Debug.Log("DisconnectJoint game over call :   " + id);
    //    SocketConnector.GAMEOVERME?.Invoke(Owner.ID.ToString(), Owner.RoomID.ToString(), "2");
    //}


    private void GetDataFromServer(string data)
    {
        JSONNode jsonNode = JSON.Parse(data);
        JSONNode playerDetail = jsonNode["playerDetails"];
        //Debug.Log("Get Data from Server jsondata :   " + jsonNode.ToString() + " LENGTH " + playerDetail.Count);
        //AllPlayerlist allPlayerlist = JsonConvert.DeserializeObject<AllPlayerlist>(data);
        //if (allPlayerlist.roomCode != roomcode)

        if (!Staticdata.isAdmin)
        {
            Debug.Log("=========RoomCode======== " + jsonNode["roomCode"]);
            Debug.Log("=========LastRoomId======== " + Staticdata.I.LastRoomId);
            if (jsonNode["roomCode"] != Staticdata.I.LastRoomId)
            {
                Debug.Log("ROOM MISMATCH RETURN " + " ");
                return;
            }
        }

        for (int i = 0; i < playerDetail.Count; i++)
        {
            //if (playerDetail[i]["playerId"] != Owner.ID)
            if (playerDetail[i]["playerId"] != Owner.ID)
            {
                //PlayerInfo p = playerInfos.Find(x => x.ID == playerDetail[i]["playerId"]);

                PlayerInfo p = playerInfos.Find(x => x.ID == playerDetail[i]["playerId"]);
                if (p.ID == null || p.ID.Equals("")) return;

                p.Playertransform.position = new Vector3(playerDetail[i]["position"]["x"].AsFloat, playerDetail[i]["position"]["y"].AsFloat, playerDetail[i]["position"]["z"].AsFloat);

                p.Playertransform.localRotation = Quaternion.Euler(playerDetail[i]["rotation"]["x"].AsFloat, playerDetail[i]["rotation"]["y"].AsFloat, playerDetail[i]["rotation"]["z"].AsFloat);

                p.Playertransform.localScale = new Vector3(playerDetail[i]["scale"]["x"].AsFloat, playerDetail[i]["scale"]["y"].AsFloat, playerDetail[i]["scale"]["z"].AsFloat);

            }
        }

    }

}


[System.Serializable]
public class MyPlayers
{
    public int roomCode { get; set; }
    public List<PlayerData> playerData = new List<PlayerData>();
}

public class PlayerData
{
    public string playerId { get; set; }
    public int position { get; set; }
    public bool isOwn { get; set; }
    public int teamCode { get; set; }
}

public class AllPlayerlist
{
    public int roomCode { get; set; }
    public List<PlayerDetails> playerDetails = new List<PlayerDetails>();
}

public class PlayerDetails
{
    public string playerId { get; set; }
    public Position position { get; set; }
    public Rotation rotation { get; set; }
    public Scale scale { get; set; }
}

public class Position
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
}

public class Rotation
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
}

public class Scale
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
}

public enum GameOverReasion
{
    CollisionDetect,
    OutOfBounds,
    JointBreak
}
