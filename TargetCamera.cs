using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetCamera : MonoBehaviour {
    static public TargetCamera S;
    public bool editMode = true;
    public GameObject fpCamera;
    public float maxPosDeviation = 1f;
    public float maxTarDeviation = .5f;
    public string deviationEasing = Easing.Out;
    public float passingAccuracy = .7f;

    public bool checkToDeletePlayerPrefs = false;
    public bool ________________;

    public Rect camRectNormal;

    public int shotNum;
    public GUIText shotCounter, shotRating;
    public GUITexture checkMark;
    public Shot lastShot;
    public int numShots;
    public Shot[] playerShots;
    public float[] playerRatings;
    public GUITexture whiteOut;

    void Awake()
    {
        S = this;
    }
    
	// Use this for initialization
	void Start () {
        GameObject go = GameObject.Find("ShotCounter");
        shotCounter = go.GetComponent<GUIText>();
        go = GameObject.Find("ShotRating");
        shotRating = go.GetComponent<GUIText>();
        go = GameObject.Find("_Check_64");
        checkMark = go.GetComponent<GUITexture>();
        go = GameObject.Find("WhiteOut");
        whiteOut = go.GetComponent<GUITexture>();
        checkMark.enabled = false;
        whiteOut.enabled = false;

        Shot.LoadShots();
        if (Shot.shots.Count > 0)
        {
            shotNum = 0;
            
            ShowShot(Shot.shots[shotNum]);
        }

        Cursor.visible = false;

        camRectNormal = GetComponent<Camera>().rect;
	}
	
	// Update is called once per frame
	void Update () {
        Shot sh;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            sh = new Shot();
            sh.position = fpCamera.transform.position;
            sh.rotation = fpCamera.transform.rotation;

            Ray ray = new Ray(sh.position, fpCamera.transform.forward);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                sh.target = hit.point;
            }
            if (editMode)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Shot.shots.Add(sh);
                    shotNum = Shot.shots.Count - 1;

                }
                else if (Input.GetMouseButtonDown(1))
                {
                    Shot.ReplaceShot(shotNum, sh);
                    ShowShot(Shot.shots[shotNum]);
                }
                
            }
            else
            {
                float acc = Shot.Compare(Shot.shots[shotNum], sh);
                lastShot = sh;
                playerShots[shotNum] = sh;
                playerRatings[shotNum] = acc;
                ShowShot(sh);
                Invoke("ShowCurrentShot", 1);

            }
            this.GetComponent<AudioSource>().Play();
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
            }
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                GetComponent<Camera>().rect = camRectNormal;
            }
            //ShowShot(sh);

            Utils.tr(sh.ToXML());

            Shot.shots.Add(sh);
            shotNum = Shot.shots.Count - 1;

        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            shotNum--;
            if (shotNum < 0) shotNum = Shot.shots.Count - 1;
            ShowShot(Shot.shots[shotNum]);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            shotNum++;
            if (shotNum >= Shot.shots.Count) shotNum = 0;
            ShowShot(Shot.shots[shotNum]);
        }

    }
    public void ShowShot(Shot sh)
    {
        StartCoroutine(WhiteOutTargetWindow());
        transform.position = sh.position;
        transform.rotation = sh.rotation;
    }

    public void ShowCurrentShot()
    {
        ShowShot(Shot.shots[shotNum]);
    }
    public IEnumerator WhiteOutTargetWindow()
    {
        whiteOut.enabled = true;
        yield return new WaitForSeconds(.05f);
        whiteOut.enabled = false;
    }
    public void OnDrawGizmos()
    {
        List<Shot> shots = Shot.shots;
        for (int i=0; i<shots.Count; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(shots[i].position, .5f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(shots[i].position, shots[i].target);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(shots[i].target, .25f);

            if (checkToDeletePlayerPrefs)
            {
                Shot.DeleteShots();
                checkToDeletePlayerPrefs = false;
                shotNum = 0;
            }

            
        }if(lastShot != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(lastShot.position, .25f);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(lastShot.position, lastShot.target);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lastShot.target, .125f);
        }
    }
}
