using System;
using TMPro;
using UnityEngine;

public class SPACTATEADMIN : MonoBehaviour
{

    public static Action<int> TimeGet;


    [Header("Game Timer")]
    public TMP_Text mTime;



    private void Awake()
    {
        Staticdata.isAdmin = true;
    }

    private void OnEnable()
    {
        TimeGet += GameTimer;
    }

    private void OnDisable()
    {
        TimeGet -= GameTimer;
    }


    public void GameTimer(int RTime)
    {
        try
        {
            string RemainTime = Staticdata.GetTimeFormat(RTime);
            mTime.text = RemainTime;

        }
        catch (Exception ex)
        {
            Debug.Log("SPACTATEADMIN Get Timer Error : " + ex.Message);
        }

    }


    private void Start()
    {
        SocketConnector.REJOINADMIN?.Invoke();
    }

    //private void OnApplicationPause(bool pause)
    //{
    //    if (!pause) return;
    //    LocalStorageData.SetString("spactate", "true");
    //    LocalStorageData.SetString("launch", "false");
    //}

    //private void OnApplicationQuit()
    //{
    //    LocalStorageData.SetString("spactate", "true");
    //    LocalStorageData.SetString("launch", "false");
    //}

    //private void OnDestroy()
    //{
    //    //SocketConnector.RELOAD?.Invoke(false,true);
    //    LocalStorageData.SetString("spactate", "true");
    //    LocalStorageData.SetString("launch", "false");
    //}



}
