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

    public void Awake()
    {
        Cursor.visible = false;
        
        //배경음 전환
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundManager.bgmNum = 1;
        soundManager.PlayeBGM();
        soundManager.reload = true;
        
        gameClient.OnScoreUpdated += OnScoreUpdated;
        gameClient.OnPositionUpdated += OnPositionUpdated;
        gameClient.OnGameStateChanged += (o, e) => OnGameStateChanged(e.gameState);
        gameClient.OnGameSceneChanged += (o, e) => OnGameSceneChanged(e.scene);
        gameClient.OnFired += OnFired;

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

        if (scores[0] == -1)
            text += $"{gameClient.client_nick1} : {scores[0]}\n";
        if (scores[1] == -1)
            text += $"{gameClient.client_nick2} : {scores[1]}\n";
        if (scores[2] == -1)
            text += $"{gameClient.client_nick3} : {scores[2]}\n";
        if (scores[3] == -1)
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

    void OnPositionUpdated(object caller, PositionUpdateEventArgs args)
    {
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
                players[i] = Instantiate(playerObject, Vector3.zero, new Quaternion(0, 0, 0, 0));
                var characterTransform = players[i].transform.Find("Box001");
                if (characterTransform != null)
                {
                    Debug.Log("characterTransform() i = " + i + ", args.player : " + args.players);
                    characterTransform.gameObject.GetComponent<Renderer>().material = decideMaterial(i);
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
            if(timer != null) timer.StartTimer();
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}