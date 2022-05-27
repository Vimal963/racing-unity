using System;
using UnityEngine;

public class Staticdata : MonoBehaviour
{
    public static Staticdata I;
    public bool IsAutoRestart;
    public bool IsHome;
    public int LastRoomId;

    //http://192.168.40.89:2323/#/admin/dashboard
    //public readonly string ServerURL = "http://13.213.210.228:3306/socket.io/"; // LIVE
    //public readonly string ServerURL = "https://admin.rohei.com/socket.io/"; // live
    //public readonly string ServerURL = "https://www.highroadadmin.rohei.com/socket.io/"; // live
    public readonly string ServerURL = "http://13.213.210.228:3006/socket.io/"; // live
    //public readonly string ServerURL = "http://13.213.210.228:4545/socket.io/"; // live
    //public readonly string ServerURL = "http://192.168.40.89:2323/socket.io/"; // Local
    //public readonly string ServerURL = "http://192.168.40.89:2323/socket.io/"; // Local

    //public readonly string ServerURL = "https://newamicablecrew.vasundharaapps.com/socket.io/"; // Local

    public static string AdminSetting;
    public static bool isAdmin = false;


    public string mplayerPosition;     
    public string mRoomName;
    public string mRoomId;
    //public string mteamCode;


    // Player gameover listen data and save for next game after reload scene
    public static string mGameMode;
    public static string mGameTime;
    public static string mGameTeam;
    //public static string mGamePlayer;


    public static bool isRestartScene = false;


    public static string GetTimeFormat(float sec)
    {
        TimeSpan t = TimeSpan.FromSeconds(sec);

        string answer = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

        return answer;
    }



    private void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(I);
        }
        //else
        //{
        //    Destroy(this);
        //}
    }


}