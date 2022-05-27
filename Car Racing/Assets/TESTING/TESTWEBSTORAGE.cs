//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class TESTWEBSTORAGE : MonoBehaviour
//{

//    [SerializeField] private Button MGetData;
//    [SerializeField] private Button MSetData;

//    [Space]
//    [SerializeField] private InputField MKey;
//    [SerializeField] private InputField MValue;

//    //[Space]
//    //[SerializeField] private Text MGetValue;


//    //private void Awake()
//    //{
//    //    if (LocalStorageData.HasKey("123"))
//    //    {
//    //        string GETDATA2 = LocalStorageData.GetString("123");
//    //        //MGetValue.text = GETDATA2;
//    //        Debug.Log("Awake GETDATA2 : " + GETDATA2);
//    //    }
//    //}

//    //private void OnEnable()
//    //{
//    //    if (LocalStorageData.HasKey("123"))
//    //    {
//    //        string GETDATA1 = LocalStorageData.GetString("123");
//    //        //MGetValue.text = GETDATA1;
//    //        Debug.Log("OnEnable GETDATA1 : " + GETDATA1);
//    //    }
//    //}

//    private void Start()
//    {
//        if (LocalStorageData.HasKey("admin"))
//        {
//            string adminkey = LocalStorageData.GetString("admin");
//            //MGetValue.text = GETDATA3;
//            Debug.Log("Start admin : " + adminkey);
//        }
//    }


//    public void GetManuallyData()
//    {
//        string GETDATAME = LocalStorageData.GetString(MKey.text);
//        //MGetValue.text = GETDATAME;
//        Debug.Log("GETDATAME : " + GETDATAME);
//    }


//    public void SetManuallyData()
//    {
//        LocalStorageData.SetString(MKey.text, MValue.text);
//        //MGetValue.text = "Key : " + MKey.text + " Value : " + MValue.text;
//        Debug.Log("SETDATAME : ");
//    }

//}
