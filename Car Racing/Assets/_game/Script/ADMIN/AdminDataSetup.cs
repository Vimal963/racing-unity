using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


public class AdminDataSetup : MonoBehaviour
{

    public static System.Action UPDATEADINSETTING;
    public static System.Action UpdateSettingPlayerLeft;

    [SerializeField] private UIAdmin UIADMIN;


    [Header("Input Timer")]
    public UnityEngine.UI.InputField MM;
    public UnityEngine.UI.InputField SS;


    public static string TPlayer = "";
    public static string roomCode = "";

    private void OnEnable()
    {
        UPDATEADINSETTING += UpdateData;
        UpdateSettingPlayerLeft += UpdateSettingPlayerLeftDataSet;
    }

    private void OnDisable()
    {
        UPDATEADINSETTING -= UpdateData;
        UpdateSettingPlayerLeft -= UpdateSettingPlayerLeftDataSet;
    }


    private void Awake()
    {
        Staticdata.isAdmin = true;
    }

    private void Start()
    {
        UpdateData();
    }


    private void UpdateData()
    {
        Debug.Log("========================Admin_Data_Save===================");
        JSONNode JData = JSON.Parse(Staticdata.AdminSetting.ToString());
        JSONNode data = JData["adminData"];

        //Debug.Log("ADMIN Scene Save json data \n " + Staticdata.AdminSetting.ToString());
        Debug.Log("ADMIN Setting Updated Data" + data.ToString());

        //Debug.Log("RoomCode : " + data["roomCode"]);
        //Debug.Log("mode : " + data["mode"]);
        //Debug.Log("auto : " + data["auto"]);
        //Debug.Log("manual : " + data["manual"]);
        //Debug.Log("timer : " + data["timer"]);
        //Debug.Log("infringements : " + data["infringements"]);
        //Debug.Log("cableSnaps : " + data["cableSnaps"]);
        //Debug.Log("bounds : " + data["bounds"]);
        //Debug.Log("collisions : " + data["collisions"]);
        //Debug.Log("player : " + data["player"]);
        //Debug.Log("team : " + data["team"]);
        //Debug.Log("status : " + data["status"]);
        //Debug.Log("isGameStart : " + data["isGameStart"]);

        TPlayer = data["player"];
        roomCode = data["roomCode"];

        //Debug.Log("timer : " + data["timer"]);
        try
        {
            int TTimer = data["timer"].AsInt;
            Debug.Log("TTimer : " + TTimer);

            int Min = TTimer / 60;
            int Sec = TTimer % 60;
            Debug.Log("Min : " + Min);
            Debug.Log("Sec : " + Sec);

            MM.text = Min.ToString("00");
            SS.text = Sec.ToString("00");

        }
        catch (System.Exception ex)
        {
            Debug.Log("DEFAULT TIMER SET ISSUE ON ADMIN PANEL : " + ex.Message);
        }


        UIADMIN.MRoomCode.text = data["roomCode"];
        UIADMIN.TInfrigments.text = data["infringements"];
        UIADMIN.TCableSnaps.text = data["cableSnaps"];
        UIADMIN.TOutOfBounds.text = data["bounds"];
        UIADMIN.TCollisions.text = data["collisions"];
        UIADMIN.TPlayer.text = data["player"];
        UIADMIN.TTeam.text = data["team"];

        if (data["auto"].AsBool)// for auto restart
        {
            Debug.Log("is Auto restart true");
            Staticdata.I.IsAutoRestart = true;
            UIADMIN.RestartSetting[0].SetActive(true);  // auto restart  
            UIADMIN.RestartSetting[1].SetActive(false); // manual restart 
            UIADMIN.ManualRestart.interactable = false; // restart button
        }
        else//for manual restart
        {
            Debug.Log("is Auto restart false");
            Staticdata.I.IsAutoRestart = false;
            UIADMIN.RestartSetting[0].SetActive(false); // auto restart
            UIADMIN.RestartSetting[1].SetActive(true);  // manual restart
            UIADMIN.ManualRestart.interactable = true;  // restart button
        }

        Staticdata.mGameMode = data["mode"];

        if (data["mode"] == "easy")
        {
            Debug.Log("game mode is easy");
            UIADMIN.GameMode[0].color = Color.white;
            UIADMIN.GameMode[1].color = UIADMIN.InActive;
            UIADMIN.GameMode[2].color = UIADMIN.InActive;
        }
        else if (data["mode"] == "medium")
        {
            Debug.Log("game mode is medium");
            UIADMIN.GameMode[0].color = UIADMIN.InActive;
            UIADMIN.GameMode[1].color = Color.white;
            UIADMIN.GameMode[2].color = UIADMIN.InActive;
        }
        else if (data["mode"] == "hard")
        {
            Debug.Log("game mode is hard");
            UIADMIN.GameMode[0].color = UIADMIN.InActive;
            UIADMIN.GameMode[1].color = UIADMIN.InActive;
            UIADMIN.GameMode[2].color = Color.white;
        }


        if (UIADMIN.Lose.activeInHierarchy || UIADMIN.Win.activeInHierarchy)
            return;

        Debug.Log("Admin Setting isGameStart : " + data["isGameStart"]);
        Debug.Log("Admin data[player].AsInt : " + data["player"].AsInt);
        //if (GameManger.isGameStart)
        if (data["player"].AsInt >= 1)
        {
            Debug.Log("Admin Game start and rejoin players : " + data["isGameStart"]);
            SocketConnector.REJOINADMIN?.Invoke();
        }
        else
        {

            Debug.Log("Admin game not start and admin not rejoin call: ");
        }

    }

    private void UpdateSettingPlayerLeftDataSet()
    {
        JSONNode JData = JSON.Parse(Staticdata.AdminSetting.ToString());
        JSONNode data = JData;

        //Debug.Log("ADMIN Scene Save json data \n " + Staticdata.AdminSetting.ToString());
        Debug.Log("adminsetting----------------------- " + Staticdata.AdminSetting);
        Debug.Log("ADMIN Setting Updated Data" + data.ToString());

        //Debug.Log("RoomCode : " + data["roomCode"]);
        //Debug.Log("mode : " + data["mode"]);
        //Debug.Log("auto : " + data["auto"]);
        //Debug.Log("manual : " + data["manual"]);
        //Debug.Log("timer : " + data["timer"]);
        //Debug.Log("infringements : " + data["infringements"]);
        //Debug.Log("cableSnaps : " + data["cableSnaps"]);
        //Debug.Log("bounds : " + data["bounds"]);
        //Debug.Log("collisions : " + data["collisions"]);
        //Debug.Log("player : " + data["player"]);
        //Debug.Log("team : " + data["team"]);
        //Debug.Log("status : " + data["status"]);
        //Debug.Log("isGameStart : " + data["isGameStart"]);

        TPlayer = data["player"];
        roomCode = data["roomName"];

        //Debug.Log("timer : " + data["timer"]);
        try
        {
            int TTimer = data["timer"].AsInt;
            Debug.Log("TTimer : " + TTimer);

            int Min = TTimer / 60;
            int Sec = TTimer % 60;
            Debug.Log("Min : " + Min);
            Debug.Log("Sec : " + Sec);

            MM.text = Min.ToString("00");
            SS.text = Sec.ToString("00");

        }
        catch (System.Exception ex)
        {
            Debug.Log("DEFAULT TIMER SET ISSUE ON ADMIN PANEL : " + ex.Message);
        }


        UIADMIN.MRoomCode.text = data["roomName"];
        UIADMIN.TInfrigments.text = data["infringements"];
        UIADMIN.TCableSnaps.text = data["cableSnaps"];
        UIADMIN.TOutOfBounds.text = data["bounds"];
        UIADMIN.TCollisions.text = data["collisions"];
        UIADMIN.TPlayer.text = data["totalPlayer"];
        UIADMIN.TTeam.text = data["totalTeam"];
        //UIADMIN.TPlayer.text = data["player"];
        //UIADMIN.TTeam.text = data["team"];

        if (data["auto"].AsBool)// for auto restart
        {
            Debug.Log("is Auto restart true");
            Staticdata.I.IsAutoRestart = true;
            UIADMIN.RestartSetting[0].SetActive(true);  // auto restart  
            UIADMIN.RestartSetting[1].SetActive(false); // manual restart 
            UIADMIN.ManualRestart.interactable = false; // restart button
        }
        else//for manual restart
        {
            Debug.Log("is Auto restart false");
            Staticdata.I.IsAutoRestart = false;
            UIADMIN.RestartSetting[0].SetActive(false); // auto restart 
            UIADMIN.RestartSetting[1].SetActive(true);  // manual restart
            UIADMIN.ManualRestart.interactable = true;  // restart button
        }

        Staticdata.mGameMode = data["mode"];

        if (data["mode"] == "easy")
        {
            Debug.Log("game mode is easy");
            UIADMIN.GameMode[0].color = Color.white;
            UIADMIN.GameMode[1].color = UIADMIN.InActive;
            UIADMIN.GameMode[2].color = UIADMIN.InActive;
        }
        else if (data["mode"] == "medium")
        {
            Debug.Log("game mode is medium");
            UIADMIN.GameMode[0].color = UIADMIN.InActive;
            UIADMIN.GameMode[1].color = Color.white;
            UIADMIN.GameMode[2].color = UIADMIN.InActive;
        }
        else if (data["mode"] == "hard")
        {
            Debug.Log("game mode is hard");
            UIADMIN.GameMode[0].color = UIADMIN.InActive;
            UIADMIN.GameMode[1].color = UIADMIN.InActive;
            UIADMIN.GameMode[2].color = Color.white;
        }


        if (UIADMIN.Lose.activeInHierarchy || UIADMIN.Win.activeInHierarchy)
            return;

        Debug.Log("Admin Setting isGameStart : " + data["isGameStart"]);
        Debug.Log("Admin data[player].AsInt : " + data["player"].AsInt);
    }

    //private void OnApplicationPause(bool pause)
    //{
    //    if (!pause) return;
    //    LocalStorageData.SetString("launch", "true");
    //    LocalStorageData.SetString("spactate", "false");
    //}

    //private void OnApplicationQuit()
    //{
    //    LocalStorageData.SetString("launch", "true");
    //    LocalStorageData.SetString("spactate", "false");
    //}

    //private void OnDestroy()
    //{
    //    //SocketConnector.RELOAD?.Invoke(true,false);
    //    LocalStorageData.SetString("launch", "true");
    //    LocalStorageData.SetString("spactate", "false");
    //}


}
