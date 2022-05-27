using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearFarEdgeMsg : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<PlayerInfo>().ID == GameManger.mPlayerID) 
        {
            //iswarning = true;
            //Debug.Log("trigger Enter");
            //StartCoroutine(Warning());
            Debug.Log("tringger Enter ------------------>>>>  ");
            UiManager.I.WarningImage.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<PlayerInfo>().ID == GameManger.mPlayerID)
        {
            Debug.Log("tringger Exit ------------------>>>>  " + gameObject.tag);
            UiManager.I.WarningImage.SetActive(false);
            //iswarning = false;
        }
    }
}

