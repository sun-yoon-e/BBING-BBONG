using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneMain : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    public Material material00;
    public Material material01;
    public Material material02;
    public Material material03;

    public Material body;
    public Material body3;
    public Material handle;
    public Material mirror;
    public Material wheel;

    public GameObject AIObject;
    public GameObject playerObject;
    public GameObject currentPlayer;
    public Text scoreTable;

    public GameObject gameOverPanel;
    public Text gameOverScoreBoard;

    public gameTimer timer;
    
    public GameObject pizza;

    private int[] scores = null;
    private GameObject[] players = null;
    //private bool materialSet = false;
    private Vector3 scaleChange;
    
    SoundManager soundManager;

    public void Start()
    {
        Cursor.visible = false;
        
        //배경음 전환
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundManager.bgmNum = 1;
        soundManager.PlayeBGM();
        soundManager.reload = true;

        gameClient.OnAIPositionUpdated += AIPositionUpdated;
        gameClient.OnAIFired += AIFired;

        gameClient.OnScoreUpdated += OnScoreUpdated;
        gameClient.OnPositionUpdated += OnPositionUpdated;
        gameClient.OnFired += OnFired;

        gameClient.OnGameStateChanged += (o, e) => OnGameStateChanged(e.gameState);
        gameClient.OnGameSceneChanged += (o, e) => OnGameSceneChanged(e.scene);

        if (gameClient.isGameStarted)
        {
            OnGameStateChanged(gameClient.isGameStarted);
        }

        scaleChange = new Vector3(2.0f, 2.0f, 2.0f);
    }

    public void OnDestroy()
    {
        scores = null;
        players = null;
        //materialSet = false;
        gameOverPanel = null;
    }

    void OnScoreUpdated(object caller, ScoreUpdateEventArgs args)
    {
        Debug.Log("OnScoreUpdated()");
        scores = args.scores;
        string text = "점수표\n";

        //for (int i = 0; i < scores.Length; i++)
        //{
        //    if (scores[i] == -1)
        //    {
        //        continue;
        //    }

        //    //text += $"플레이어 {i + 1}: {scores[i]}\n";
        //}

        //if (scores[0] != -1)
        text += $"{gameClient.client_nick1} : {scores[0]}\n";
        text += $"{gameClient.client_nick2} : {scores[1]}\n";
        text += $"{gameClient.client_nick3} : {scores[2]}\n";
        text += $"{gameClient.client_nick4} : {scores[3]}\n";

        scoreTable.text = text;
    }

    Material decideMaterial(int playerId)
    {
        if (playerId % 4 == 0)
        {
            Debug.Log("decideMaterial() 00 : " + playerId);
            return material00;
        }
        if (playerId % 4 == 1)
        {
            Debug.Log("decideMaterial() 01 : " + playerId);
            return material01;
        }
        if (playerId % 4 == 2)
        {
            Debug.Log("decideMaterial() 02 : " + playerId);
            return material02;
        }
        Debug.Log("decideMaterial() 03 : " + playerId);
        return material03;
    }
    public void AIFired(object sender, AIFireEventArgs args)
    {
        GameObject pizzaObject = Instantiate(pizza);
        pizzaObject.transform.position = args.position;
        var dir = args.targetPosition - args.position;
        pizzaObject.transform.forward = dir;

        Debug.Log("AIFireUpdate");
        //args.AIID;
    }

    public void AIPositionUpdated(object sender, AIPositionUpdateEventArgs args)
    {
        AIObject.gameObject.SetActive(true);
        Destroy(players[args.AIID - 1]);

        players = new GameObject[args.AIID-1];
        Debug.Log($"new player : + {args.AIID - 1}");

        if (gameClient.ai[args.AIID] == true)
        {
            players[args.AIID - 1] = Instantiate(AIObject, Vector3.zero, new Quaternion(0, 0, 0, 0));
            var characterTransform = players[args.AIID - 1].transform.Find("Box001");
            if (characterTransform != null)
            {
                characterTransform.gameObject.GetComponent<Renderer>().material = decideMaterial(args.AIID - 1);
            }
            var bike00 = players[args.AIID].transform.Find("pasted__pasted__pSphere8");
            bike00.gameObject.GetComponent<Renderer>().material = body;
            var bike01 = players[args.AIID].transform.Find("pasted__pasted__polySurface88");
            bike01.gameObject.GetComponent<Renderer>().material = body;

            var bike10 = players[args.AIID].transform.Find("pPlane1");
            bike10.gameObject.GetComponent<Renderer>().material = body3;
            var bike11 = players[args.AIID].transform.Find("pPlane2");
            bike11.gameObject.GetComponent<Renderer>().material = body3;
            var bike12 = players[args.AIID].transform.Find("pPlane3");
            bike12.gameObject.GetComponent<Renderer>().material = body3;
            var bike13 = players[args.AIID].transform.Find("pPlane4");
            bike13.gameObject.GetComponent<Renderer>().material = body3;
            var bike14 = players[args.AIID].transform.Find("pPlane5");
            bike14.gameObject.GetComponent<Renderer>().material = body3;
            var bike15 = players[args.AIID].transform.Find("pPlane6");
            bike15.gameObject.GetComponent<Renderer>().material = body3;
            var bike16 = players[args.AIID].transform.Find("pasted__pasted__polySurface81");
            bike16.gameObject.GetComponent<Renderer>().material = body3;
            var bike17 = players[args.AIID].transform.Find("pasted__pasted__polySurface82");
            bike17.gameObject.GetComponent<Renderer>().material = body3;

            var bike20 = players[args.AIID].transform.Find("pasted__pasted__pCube2");
            bike20.gameObject.GetComponent<Renderer>().material = wheel;
            var bike21 = players[args.AIID].transform.Find("BackWheel");
            bike21.gameObject.GetComponent<Renderer>().material = wheel;
            var bike22 = players[args.AIID].transform.Find("FrontWheel");
            bike22.gameObject.GetComponent<Renderer>().material = wheel;

            var bike30 = players[args.AIID].transform.Find("Object002");
            bike30.gameObject.GetComponent<Renderer>().material = handle;
            var bike31 = players[args.AIID].transform.Find("pasted__pasted__pCylinder12");
            bike31.gameObject.GetComponent<Renderer>().material = handle;
            var bike32 = players[args.AIID].transform.Find("pasted__pasted__polySurface87");
            bike32.gameObject.GetComponent<Renderer>().material = handle;
            var bike33 = players[args.AIID].transform.Find("CenterMirror");
            bike33.gameObject.GetComponent<Renderer>().material = handle;

            var bike40 = players[args.AIID].transform.Find("RightMirror");
            bike40.gameObject.GetComponent<Renderer>().material = mirror;
            var bike41 = players[args.AIID].transform.Find("LeftMirror");
            bike41.gameObject.GetComponent<Renderer>().material = mirror;
        }

        Debug.Log("AIPositionUpdate");
        //args.AIID;
        //args.position;
        //args.rotation;
    }

    void OnPositionUpdated(object caller, PositionUpdateEventArgs args)
    {
        Debug.Log("GameMain PositionUpdate");
        if (!gameClient.isReadyToControl) return;
        Debug.Log("OnPositionUpdated() args.player : " + args.players);

        if (players == null || players.Length != args.players)
        {
            if (players != null)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    Destroy(players[i]);
                }
            }

            players = new GameObject[args.players];
            Debug.Log("new player : " + args.players);

            for (int i = 0; i < args.players; i++)
            {
                if (gameClient.ai[i] == false)
                {
                    players[i] = Instantiate(playerObject, Vector3.zero, new Quaternion(0, 0, 0, 0));
                    var characterTransform = players[i].transform.Find("Box001");
                    if (characterTransform != null)
                    {
                        Debug.Log("characterTransform() i = " + i + ", args.player : " + args.players);
                        characterTransform.gameObject.GetComponent<Renderer>().material = decideMaterial(i);
                    }
                    var bike00 = players[i].transform.Find("pasted__pasted__pSphere8");
                    bike00.gameObject.GetComponent<Renderer>().material = body;
                    var bike01 = players[i].transform.Find("pasted__pasted__polySurface88");
                    bike01.gameObject.GetComponent<Renderer>().material = body;

                    var bike10 = players[i].transform.Find("pPlane1");
                    bike10.gameObject.GetComponent<Renderer>().material = body3;
                    var bike11 = players[i].transform.Find("pPlane2");
                    bike11.gameObject.GetComponent<Renderer>().material = body3;
                    var bike12 = players[i].transform.Find("pPlane3");
                    bike12.gameObject.GetComponent<Renderer>().material = body3;
                    var bike13 = players[i].transform.Find("pPlane4");
                    bike13.gameObject.GetComponent<Renderer>().material = body3;
                    var bike14 = players[i].transform.Find("pPlane5");
                    bike14.gameObject.GetComponent<Renderer>().material = body3;
                    var bike15 = players[i].transform.Find("pPlane6");
                    bike15.gameObject.GetComponent<Renderer>().material = body3;
                    var bike16 = players[i].transform.Find("pasted__pasted__polySurface81");
                    bike16.gameObject.GetComponent<Renderer>().material = body3;
                    var bike17 = players[i].transform.Find("pasted__pasted__polySurface82");
                    bike17.gameObject.GetComponent<Renderer>().material = body3;

                    var bike20 = players[i].transform.Find("pasted__pasted__pCube2");
                    bike20.gameObject.GetComponent<Renderer>().material = wheel;
                    var bike21 = players[i].transform.Find("BackWheel");
                    bike21.gameObject.GetComponent<Renderer>().material = wheel;
                    var bike22 = players[i].transform.Find("FrontWheel");
                    bike22.gameObject.GetComponent<Renderer>().material = wheel;

                    var bike30 = players[i].transform.Find("Object002");
                    bike30.gameObject.GetComponent<Renderer>().material = handle;
                    var bike31 = players[i].transform.Find("pasted__pasted__pCylinder12");
                    bike31.gameObject.GetComponent<Renderer>().material = handle;
                    var bike32 = players[i].transform.Find("pasted__pasted__polySurface87");
                    bike32.gameObject.GetComponent<Renderer>().material = handle;
                    var bike33 = players[i].transform.Find("CenterMirror");
                    bike33.gameObject.GetComponent<Renderer>().material = handle;

                    var bike40 = players[i].transform.Find("RightMirror");
                    bike40.gameObject.GetComponent<Renderer>().material = mirror;
                    var bike41 = players[i].transform.Find("LeftMirror");
                    bike41.gameObject.GetComponent<Renderer>().material = mirror;
                }
            }
        }
        else
        {
            Debug.Log("OnPositionUpdated() else : " + players.Length);
        }

        // 플레이어 크기 동일하게 조절
        for (int i = 0; i < players.Length; i++)
        {
            Debug.Log("Player" + i + " Scale: " + players[i].transform.localScale);
            if (gameClient.clientId == i)
            {
                players[i].SetActive(false);
                continue;
            }
            players[i].transform.localScale = scaleChange;

            var eulerAngles = new Vector3(
                args.rotation[i].x,
                args.rotation[i].y,
                args.rotation[i].z
            );

            var rotation = new Quaternion();
            rotation.eulerAngles = eulerAngles;

            players[i].transform.position = args.position[i];
            players[i].transform.rotation = rotation;
        }
    }

    void OnGameStateChanged(bool gameState)
    {
        if (gameState)
        {
            if(gameOverPanel != null) gameOverPanel.SetActive(false);
        }
        else
        {
            gameOverPanel.SetActive(true);
            timer.stopTimer();

            if (scores != null)
            {
                //var scoreMap = new Dictionary<int, int>();
                var scoreMap = new Dictionary<string, int>();
                //for (int i = 0; i < scores.Length; i++)
                //{
                //    //scoreMap.Add(i, scores[i]);
                //}
                scoreMap.Add(gameClient.client_nick1, scores[0]);
                scoreMap.Add(gameClient.client_nick2, scores[1]);
                scoreMap.Add(gameClient.client_nick3, scores[2]);
                scoreMap.Add(gameClient.client_nick4, scores[3]);

                string text = "";
                int j = 1;

                foreach (var item in scoreMap.OrderBy(x => x.Value).Reverse())
                {
                    //text += $"{j++}등 플레이어 {item.Key + 1} - {item.Value}점\n";
                    text += $"{j++}등 - {item.Key + 1} - {item.Value}점\n";
                }

                gameOverScoreBoard.text = text;
            }
            else
            {
                gameOverScoreBoard.text = "점수를 가져올 수 없습니다.";
            }
        }
    }

    void OnGameSceneChanged(int scene)
    {
        Debug.Log("OnGameSceneChanged(): " + scene);
        //if (scene == 1)
        //{
        //    Debug.Log("SceneManager.LoadScene() Scenes/LoginScene");
        //    gameOverPanel.SetActive(false);

        //    SceneManager.LoadScene("Scenes/LoginScene", LoadSceneMode.Single);
        //}
        if (scene == 2)
        {
            Debug.Log("SceneManager.LoadScene() Scenes/Scenes/WaitingRoomScene");
            gameOverPanel.SetActive(false);
            SceneManager.LoadScene("Scenes/WaitingRoomScene", LoadSceneMode.Single);
        }
    }

    public void OnFired(object sender, FireEventArgs args)
    {
        GameObject pizzaObject = Instantiate(pizza);
        pizzaObject.transform.position = args.position;
        var dir = args.targetPosition - args.position;
        pizzaObject.transform.forward = dir;
    }
}