using System.Collections;
using SimpleJSON;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    private float hmove;
    private float vmove;
    private float offsetmovSpeed = 70f;

    private Cinemachine.CinemachineVirtualCamera mCinemachineCamera;

    public bool isStart;
    private Transform PlayerOffset;
    private Transform You;

    [SerializeField] private Rigidbody2D rb;
    private PlayerInfo playerInfo;
    private GameManger gameManger;
    public GameObject precar, nextcar;
    public bool Isprevius;
    public bool isnext;

    Vector3 youOffset;

    public static System.Action<string> MessageAction;
    //public static System.Action<string> GameDifficulty;

    private float MaxCableSnap = 6.5f;       // EASY MODE DEFAULT
    private float toNearDistance = 4.5f;     // EASY MODE DEFAULT
    private float toFarDistance = 6f;        // EASY MODE DEFAULT
    private float ControllerSpeed = 12f;     // EASY MODE DEFAULT

    public static string PlayerID;

    private void OnEnable()
    {
        MessageAction += MessageShow;
        //GameDifficulty += OnGameDifficulty;
    }
    private void OnDisable()
    {
        MessageAction -= MessageShow;
        //GameDifficulty -= OnGameDifficulty;
    }

    //private bool iswarning, istofar, istonear;
    private Vector3 PlayerPosition;

    public Vector3 GetPosition(float z)
    {
        Vector3 pos;
        if (z == 0)
            pos = new Vector3(0, 3, 0);
        else if (z == 90)
            pos = new Vector3(-3, 0, 0);
        else if (z == 180)
            pos = new Vector3(0, -3, 0);
        else if (z == 270)
            pos = new Vector3(3, 0, 0);
        else
            pos = Vector3.zero;

        return pos;
    }


    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0f;

        //JSONNode JData = JSON.Parse(Staticdata.AdminSetting.ToString());
        //JSONNode data = JData["adminData"];

        PlayerID = GameManger.mPlayerID;

        Debug.Log("mode : " + Staticdata.mGameMode);

        switch (Staticdata.mGameMode)
        {
            case "easy":
                {
                    MaxCableSnap = 6.5f;
                    toNearDistance = 4.5f;
                    toFarDistance = 6f;
                    ControllerSpeed = 12f;
                    //OnGameDifficulty("easy", 12f);
                    break;
                }
            //case "medium":
            //    {
            //        MaxCableSnap = 6.25f;
            //        toNearDistance = 4.25f;
            //        toFarDistance = 5.75f;
            //        ControllerSpeed = 24f;
            //        //OnGameDifficulty("medium", 24f);
            //        break;
            //    }

            case "medium":
                {
                    MaxCableSnap = 6.5f;
                    toNearDistance = 4.5f;
                    toFarDistance = 6f;
                    ControllerSpeed = 12f;
                    //OnGameDifficulty("easy", 12f);
                    break;
                }
            //case "hard":
            //    {
            //        MaxCableSnap = 6f;
            //        toNearDistance = 4f;
            //        toFarDistance = 5.5f;
            //        ControllerSpeed = 36f;
            //        //OnGameDifficulty("hard", 36f);
            //        break;
            //    }
            case "hard":
                {
                    MaxCableSnap = 6.25f;
                    toNearDistance = 4.25f;
                    toFarDistance = 5.75f;
                    ControllerSpeed = 24f;
                    //OnGameDifficulty("medium", 24f);
                    break;
                }
            default:
                break;
        }

    }


    public void setdetails(Cinemachine.CinemachineVirtualCamera cam, Transform pOffset, Transform y, GameManger _gameManager, PlayerInfo _playerInfo, Vector3 rot)
    {
        Debug.Log("SETDETAILS :  " + transform.position);
        playerInfo = _playerInfo;
        PlayerOffset = pOffset;
        PlayerOffset.position = Vector2.zero;
        mCinemachineCamera = cam;
        mCinemachineCamera.transform.rotation = Quaternion.Euler(rot);
        You = y;
        //You.rotation = mCinemachineCamera.transform.rotation;
        PlayerPosition = GetPosition(rot.z);

        youOffset.y = 6;
        You.SetParent(this.gameObject.transform);
        You.localPosition = new Vector3(0, 2, 0);
        You.localRotation = Quaternion.Euler(0, 0, 0);
        //You.position = transform.position + youOffset;
        //You.position = transform.position + PlayerOffset.position;

        gameManger = _gameManager;
        Isprevius = true;
        isnext = true;
        GameManger.SendPlayerData?.Invoke(transform);
    }


    public void PlayerStart()
    {
        isStart = false;
        You.gameObject.SetActive(true);
        mCinemachineCamera.m_Lens.OrthographicSize = 140;
        mCinemachineCamera.gameObject.transform.position = new Vector3(0, 0, -10f);
        mCinemachineCamera.LookAt = PlayerOffset;
        StartCoroutine(zoomin());
    }

    public void GameOverEffect()
    {
        if (UiManager.I.Win.activeInHierarchy) return;
        if (UiManager.I.Lose.activeInHierarchy) return;
        isStart = false;
        You.gameObject.SetActive(false);
        mCinemachineCamera.LookAt = PlayerOffset;
        mCinemachineCamera.Follow = PlayerOffset;
        StartCoroutine(zoomout());
    }

    public void GameWinEffect()
    {
        isStart = false;
        You.gameObject.SetActive(false);
        mCinemachineCamera.LookAt = PlayerOffset;
        mCinemachineCamera.Follow = PlayerOffset;
        StartCoroutine(zoomoutWin());
    }

    public IEnumerator zoomout()
    {
        UiManager.Display?.Invoke(panal.GameOverPanal);
        UiManager.I?.Lose.SetActive(true);
        while (mCinemachineCamera.m_Lens.OrthographicSize < 140)
        {
            PlayerOffset.position = Vector3.MoveTowards(PlayerOffset.position, new Vector2(0, 0), offsetmovSpeed * Time.deltaTime);
            mCinemachineCamera.m_Lens.OrthographicSize += .4f;
            yield return new WaitForSeconds(.01f);
        }
        //UiManager.I?.HomebtnOnGameover.SetActive(true);
        //UiManager.I?.RestartbtnOnGameOver.SetActive(true);
    }

    public IEnumerator zoomoutWin()
    {
        UiManager.Display?.Invoke(panal.GameOverPanal);
        UiManager.I?.Win.SetActive(true);
        UiManager.I?.Lose.SetActive(false);
        while (mCinemachineCamera.m_Lens.OrthographicSize < 140)
        {
            PlayerOffset.position = Vector3.MoveTowards(PlayerOffset.position, new Vector2(0, 0), offsetmovSpeed * Time.deltaTime);
            mCinemachineCamera.m_Lens.OrthographicSize += .4f;
            yield return new WaitForSeconds(.01f);
        }
        //UiManager.I?.HomebtnOnGameWin.SetActive(true);
        //UiManager.I?.RestartbtnOnGameWin.SetActive(true);
    }


    public IEnumerator zoomin()
    {

        //You.position = transform.localPosition + youOffset;
        //You.rotation = transform.rotation;

        //transform.rotation = Quaternion.Euler(0, 0, 0);
        yield return new WaitForSeconds(1f);
        mCinemachineCamera.Follow = PlayerOffset;
        while (mCinemachineCamera.m_Lens.OrthographicSize > 18)
        {
            PlayerOffset.position = Vector3.MoveTowards(PlayerOffset.position, transform.position, offsetmovSpeed * Time.deltaTime);
            mCinemachineCamera.m_Lens.OrthographicSize -= .4f;
            yield return new WaitForSeconds(.01f);
        }
        //You.position = transform.position + PlayerPosition + PlayerOffset.position;
        isStart = true;
        UiManager.isGameStart = true;
    }

    void Update()
    {
        if (isStart)
        {
            Carmovement();
        }
    }

    public void Carmovement()
    {
        hmove = Input.GetAxis("Horizontal");

        vmove = Input.GetAxis("Vertical");

        if (vmove != 0 || hmove != 0)
        {

            if (hmove > 0)
            {
                //Debug.Log("right forword  ");
                transform.Rotate(Vector3.forward * -hmove * Time.deltaTime * ControllerSpeed * 20);
                Vector2 movement = Vector2.up * hmove;
                transform.Translate(movement * Time.deltaTime * ControllerSpeed);
            }
            else if (hmove < 0)
            {
                //Debug.Log("left forword  ");
                transform.Rotate(-Vector3.forward * hmove * Time.deltaTime * ControllerSpeed * 20);
                Vector2 movement = Vector2.up * -hmove;
                transform.Translate(movement * Time.deltaTime * ControllerSpeed);
            }

            else if (vmove != 0)
            {
                //Debug.Log("Move Forword  ");
                Vector2 movement = Vector2.up * vmove;
                transform.Translate(movement * Time.deltaTime * ControllerSpeed);
            }

            PlayerOffset.position = transform.position;
            GameManger.SendPlayerData?.Invoke(transform);

            CheckNextGameOver();
        }
    }


    string oppID = "";
    public void CheckNextGameOver()
    {
        float distanceP = 0;
        float distanceN = 0;


        if (nextcar != null && isnext)
        {
            distanceN = Vector2.Distance(transform.position, nextcar.transform.position);
        }
        if (precar != null && Isprevius)
        {
            distanceP = Vector2.Distance(transform.position, precar.transform.position);
        }


        if (distanceN < toNearDistance && distanceN > 0)
        {
            //Debug.Log("-------11111--");
            UiManager.I.tonearObject.SetActive(true);
            oppID = nextcar.GetComponent<PlayerInfo>().ID;
            WarningMessage(GameManger.mPlayerID, oppID, "near");
        }
        else if (distanceP < toNearDistance && distanceP > 0)
        {
            //Debug.Log("-------2222--");
            UiManager.I.tonearObject.SetActive(true);
            oppID = precar.GetComponent<PlayerInfo>().ID;
            WarningMessage(GameManger.mPlayerID, oppID, "near");
        }
        else if (distanceN > toFarDistance && distanceN < 8f)
        {
            //Debug.Log("-------3333--");
            UiManager.I.tofarObject.SetActive(true);
            oppID = nextcar.GetComponent<PlayerInfo>().ID;
            WarningMessage(GameManger.mPlayerID, oppID, "far");
        }
        else if (distanceP > toFarDistance && distanceP < 8f)
        {
            //Debug.Log("-------4444--");
            UiManager.I.tofarObject.SetActive(true);
            oppID = precar.GetComponent<PlayerInfo>().ID;
            WarningMessage(GameManger.mPlayerID, oppID, "far");
        }
        else if (oppID != "")
        {
            //Debug.Log("-------5555--");
            if (UiManager.I.tofarObject.activeInHierarchy || UiManager.I.tonearObject.activeInHierarchy)
                 
            {
                //Debug.Log("-------66666--");
                UiManager.I.tofarObject.SetActive(false);
                UiManager.I.tonearObject.SetActive(false);
                WarningMessage(GameManger.mPlayerID, oppID, "offWarning");
            }
        }


        if (distanceN > MaxCableSnap)
        {
            isnext = false;
            nextcar.GetComponent<PlayerInfo>().cabaljoint.enabled = false;
            isStart = false;
            SocketConnector.GAMEOVERME?.Invoke(GameManger.mPlayerID, Staticdata.I.LastRoomId.ToString(), "2");
            //GameManger.JointBreak?.Invoke(nextcar.GetComponent<PlayerInfo>().ID.ToString());
        }
        else if (distanceP > MaxCableSnap)
        {
            Isprevius = false;
            playerInfo.cabaljoint.enabled = false;
            isStart = false;
            Debug.Log("Joint Break pID : " + playerInfo.ID);
            SocketConnector.GAMEOVERME?.Invoke(GameManger.mPlayerID, Staticdata.I.LastRoomId.ToString(), "2");
            //GameManger.JointBreak?.Invoke(playerInfo.ID.ToString());
        }
    }

    void WarningMessage(string ownId, string oppId, string message)
    {
        JSONNode data = new JSONObject
        {
            ["ownPlayerId"] = ownId,
            ["oppPlayerId"] = oppId,
            ["message"] = message
        };

        //Debug.Log("WARNING   MSG " + data.ToString());
        SocketConnector.I.socket?.Emit("nearAndFarMessage", data.ToString());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isStart) return;
        //if (GameManger.mIsGameOver) return;
        try
        {
            if (collision.gameObject.CompareTag("Road"))
            {
                Debug.Log("Send Reason =  1", gameObject); // reson : Out of bound 
                SocketConnector.GAMEOVERME?.Invoke(GameManger.mPlayerID, Staticdata.I.LastRoomId.ToString(), "1");
            }
            else if (collision.gameObject.CompareTag("Obstacles")) // Obstacles
            {
                Debug.Log("Send Reason Obstacles =  0 "); // reson : Obstacles 
                SocketConnector.GAMEOVERME?.Invoke(GameManger.mPlayerID, Staticdata.I.LastRoomId.ToString(), "0");  // reson 0 : collision
            }
            else if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Send Reason =  0", gameObject);  // reson : Collision 
                SocketConnector.GAMEOVERME?.Invoke(GameManger.mPlayerID, Staticdata.I.LastRoomId.ToString(), "0");
            }
        }
        catch
        {
            Debug.Log("Handled exception on Collision Enter gameover ");
        }
    }



    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    try
    //    {
    //        if (gameManger.Owner.ID != GameManger.mPlayerID)
    //        {
    //            //Debug.Log("GameWin return for some other car triggered");
    //            return;
    //        }
    //        //if (collision.gameObject.CompareTag("Win"))
    //        //{
    //        //    //Debug.Log("GameWin");
    //        //    SocketConnector.GAMEWINME?.Invoke(gameManger.Owner.ID.ToString());

    //        //}
    //        //if (collision.gameObject.CompareTag("Road") && isStart)
    //        //{
    //        //    //iswarning = true;
    //        //    //Debug.Log("trigger Enter");
    //        //    //StartCoroutine(Warning());
    //        //    Debug.Log("tringger Enter ------------------>>>>  ");
    //        //    UiManager.I.WarningImage.SetActive(true);
    //        //}

    //    }
    //    catch (System.Exception ex)
    //    {
    //        //Debug.Log("TRIGGER ERROR : " + ex);
    //    }
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        try
        {


            if (gameManger.Owner.ID != GameManger.mPlayerID)
            {
                //Debug.Log("GameWin return for some other car triggered");
                return;
            }
            if (collision.gameObject.CompareTag("Win"))
            {
                //Debug.Log("GameWin");
                SocketConnector.GAMEWINME?.Invoke(gameManger.Owner.ID.ToString());

            }
            
        }
        catch(System.Exception ex)
        {
            //Debug.Log("TRIGGER ERROR : " + ex);
        }

        //if (collision.gameObject.CompareTag("Road") && isStart)
        //    {
        //    Debug.Log("tringger Exit ------------------>>>>  "  + gameObject.tag);
        //    UiManager.I.WarningImage.SetActive(false);
        //    //iswarning = false;
        //}
    }

    public void MessageShow(string mess)
    {
        JSONNode jData = JSON.Parse(mess);

        string lType = jData["message"];

        //var oppPlayerId = jData["oppPlayerId"];
        //var ownPayerId = jData["ownPayerId"];

        //if (ownPayerId != GameManger.mPlayerID) return;

        switch (lType)
        {
            case "near":
                UiManager.I.tofarObject.SetActive(false);    //false
                UiManager.I.tonearObject.SetActive(false);   //false
                UiManager.I.tonearObject.SetActive(true);   //true
                break;

            case "far":
                UiManager.I.tofarObject.SetActive(false);    //false
                UiManager.I.tonearObject.SetActive(false);   //false
                UiManager.I.tofarObject.SetActive(true);    //true
                break;

            case "offWarning":
                UiManager.I.tofarObject.SetActive(false);    //false
                UiManager.I.tonearObject.SetActive(false);   //false
                break;
            default:
                UiManager.I.tofarObject.SetActive(false);    //false
                UiManager.I.tonearObject.SetActive(false);   //false
                break;
        }
    }

}
