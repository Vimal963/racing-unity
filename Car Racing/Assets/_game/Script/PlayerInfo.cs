using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{

    public string ID = "0";

    public int RoomID = 0;

    public int GroupName = 0;

    public int PlayerNo = 0;

    public float distoflastcar = .6f;

    public Transform Playertransform;

    public Rigidbody2D body;

    public HingeJoint2D joint;

    public GameObject[] cables;

    public HingeJoint2D cabaljoint;

    public List<GameObject> cablelist = new List<GameObject>();


    public void SetJoint(GameObject previusObject)
    {
        //Debug.Log("previusObject : " + previusObject.name);
        //Debug.Log("cables.Length : " + cables.Length);
        cablelist.Clear();
        GetComponent<PlayerControler>().precar = previusObject;
        previusObject.GetComponent<PlayerControler>().nextcar = gameObject;

        Rigidbody2D previusbody = previusObject.GetComponent<Rigidbody2D>();
        for (int i = 0; i < cables.Length; i++)
        {
            //Debug.Log("cables.Length : " + cables.Length);
            GameObject c = Instantiate(cables[i], previusObject.transform.position, this.transform.localRotation);
            c.transform.SetParent(previusObject.transform);
            HingeJoint2D hj = c.GetComponent<HingeJoint2D>();
            hj.connectedBody = previusbody;
            cablelist.Add(c);
            if (i == 0)
                hj.connectedAnchor = new Vector2(0, -2f);
            else
                hj.connectedAnchor = new Vector2(0, -1.5f);

            //hj.connectedAnchor = Vector2.zero;
            //hj.anchor = Vector2.zero;

            if (i < cables.Length - 1)
            {
                previusbody = c.GetComponent<Rigidbody2D>();
            }
            else
            {
                joint.enabled = true;
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedBody = c.GetComponent<Rigidbody2D>();
                joint.connectedAnchor = new Vector2(0, -distoflastcar);
            }
            if (i == 2)
            {
                cabaljoint = hj;
            }
        }

    }

    public void setDetails(string _id, int _roomId, int _playerno, int _teamID)
    {
        ID = _id;
        RoomID = _roomId;
        PlayerNo = _playerno;
        GroupName = _teamID;
        Playertransform = transform;
    }



    private void OnDestroy()
    {
        foreach (var item in cablelist)
        {
            DestroyImmediate(item.gameObject);
        }
        cablelist.Clear();

        GameObject nextcar = GetComponent<PlayerControler>().nextcar;
        if (nextcar == null)
            return;

        nextcar.GetComponent<PlayerInfo>().RemoveCableOnly();
    }


    public void RemoveCableOnly()
    {
        foreach (var item in cablelist)
        {
            DestroyImmediate(item.gameObject);
        }
        cablelist.Clear();
    }



    public Vector2 getcenterpos(Vector2 firstpos, Vector2 lastpos)
    {
        float firstx = firstpos.x;
        float firsty = firstpos.y;
        float lastx = lastpos.x;
        float lasty = lastpos.y;

        float centerx = ((firstx + lastx) / 2);
        float centery = ((firsty + lasty) / 2);
        return new Vector2(centerx, centery);

    }

}