using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneMain : MonoBehaviour
{
    public static GameSceneMain instacne;
    private GameClient gameClient = GameClient.Instance;

    public Material material00;
    public Material material01;
    public Material material02;
    public Material material03;

    public GameObject AIObject;
    public GameObject playerObject;
    public GameObject currentPlayer;
    public Text scoreTable;

    public GameObject gameOverPanel;
    public Text gameOverScoreBoard;

    //public gameTimer timer;
    public Text timerText;

    public GameObject pizza;
    public GameObject[] players = null;
    private int[] scores = null;

    SoundManager soundManager;
    RoadGenerator road;
    CreateAIID AIID;

    public void Start()
    {
        instacne = this;
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();

        Cursor.visible = false;
        scores = new int[4];

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

        players = new GameObject[4];

        if (gameClient.client_nick == gameClient.client_nick1)
        {
            var m = currentPlayer.transform.Find("Controller/Rider/Box001");
            m.gameObject.GetComponent<Renderer>().material = decideMaterial(0);
            gameClient.playerRoomNum = 0;
        }
        else if (gameClient.client_nick == gameClient.client_nick2)
        {
            var m = currentPlayer.transform.Find("Controller/Rider/Box001");
            m.gameObject.GetComponent<Renderer>().material = decideMaterial(1);
            gameClient.playerRoomNum = 1;
        }
        else if (gameClient.client_nick == gameClient.client_nick3)
        {
            var m = currentPlayer.transform.Find("Controller/Rider/Box001");
            m.gameObject.GetComponent<Renderer>().material = decideMaterial(2);
            gameClient.playerRoomNum = 2;
        }
        else if (gameClient.client_nick == gameClient.client_nick4)
        {
            var m = currentPlayer.transform.Find("Controller/Rider/Box001");
            m.gameObject.GetComponent<Renderer>().material = decideMaterial(3);
            gameClient.playerRoomNum = 3;
        }
        currentPlayer.transform.position = road.vertices[road.vertices.Length / 2 + (road.xSize + 1) * gameClient.playerRoomNum + 1];

        for (int i = 0; i < 4; i++)
        {
            if (gameClient.client_host)
            {
                if (gameClient.ai_client[i])
                {
                    players[i] = Instantiate(AIObject);
                    players[i].transform.position = road.vertices[road.vertices.Length / 2 + (road.xSize + 1) * i + 1];
                    //players[i].transform.rotation = Quaternion.Euler(0, 0, 0);

                    AIID = players[i].transform.Find("AIID").GetComponent<CreateAIID>();
                    AIID.idNum = i;

                    var m = players[i].transform.Find("Controller/Rider/Box001");
                    m.gameObject.GetComponent<Renderer>().material = decideMaterial(i);
                }
                else
                {
                    if (gameClient.playerRoomNum == i) continue;
                    players[i] = Instantiate(playerObject);
                    //players[i].transform.position = road.vertices[road.vertices.Length / 2 + (road.xSize + 1) * i + 1];
                    //players[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                    var m = players[i].transform.Find("Rider/Box001");
                    m.gameObject.GetComponent<Renderer>().material = decideMaterial(i);
                }
            }
            else
            {
                if (gameClient.playerRoomNum == i) continue;
                players[i] = Instantiate(playerObject);
                //players[i].transform.position = road.vertices[road.vertices.Length / 2 + (road.xSize + 1) * i + 1];
                //players[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                var m = players[i].transform.Find("Rider/Box001");
                m.gameObject.GetComponent<Renderer>().material = decideMaterial(i);
            }

            scores[i] = 0;
        }

        string text = "점수표\n";

        text += $"{gameClient.client_nick1} : {scores[0]}\n";
        text += $"{gameClient.client_nick2} : {scores[1]}\n";
        text += $"{gameClient.client_nick3} : {scores[2]}\n";
        text += $"{gameClient.client_nick4} : {scores[3]}\n";

        scoreTable.text = text;
    }

    public void OnDestroy()
    {
        scores = null;
        players = null;
        gameOverPanel = null;
    }

    Material decideMaterial(int playerId)
    {
        if (playerId % 4 == 0)
        {
            //Debug.Log("decideMaterial() 0, id :" + playerId);
            return material00;
        }
        if (playerId % 4 == 1)
        {
            //Debug.Log("decideMaterial() 1, id : " + playerId);
            return material01;
        }
        if (playerId % 4 == 2)
        {
            //Debug.Log("decideMaterial() 2, id : " + playerId);
            return material02;
        }
        //Debug.Log("decideMaterial() 3, id : " + playerId);
        return material03;
    }

    void OnScoreUpdated(object caller, ScoreUpdateEventArgs args)
    {
        //Debug.Log("OnScoreUpdated()");
        for (int i = 0; i < 4; i++)
        {
            if (args.id == i) scores[i] = args.score;
            //Debug.Log(i + "'s score : " + scores[i]);
        }

        if (scores != null)
        {
            string text = "점수표\n";
            var scoreMap = new Dictionary<string, int>();
            scoreMap.Add(gameClient.client_nick1, scores[0]);
            scoreMap.Add(gameClient.client_nick2, scores[1]);
            scoreMap.Add(gameClient.client_nick3, scores[2]);
            scoreMap.Add(gameClient.client_nick4, scores[3]);

            foreach (var item in scoreMap.OrderByDescending(x => x.Value).ThenBy(y => y.Key))
            {
                text += $"{item.Key} : {item.Value}\n";
            }
            scoreTable.text = text;
        }
    }

    public void AIFired(object sender, AIFireEventArgs args)
    {
        //Debug.Log("AIFireUpdate");

        GameObject pizzaObject = Instantiate(pizza);
        pizzaObject.transform.position = args.position;
        var dir = args.targetPosition - args.position;
        pizzaObject.transform.forward = dir;

        var animator = players[args.AIID].GetComponent<PizzaAnimator>();
        animator.isAnimated = true;

        for (int i = 0; i < 4; i++)
        {
            if (i == args.AIID) scores[i] += 50;
        }

        if (scores != null)
        {
            string text = "점수표\n";
            var scoreMap = new Dictionary<string, int>();
            scoreMap.Add(gameClient.client_nick1, scores[0]);
            scoreMap.Add(gameClient.client_nick2, scores[1]);
            scoreMap.Add(gameClient.client_nick3, scores[2]);
            scoreMap.Add(gameClient.client_nick4, scores[3]);

            foreach (var item in scoreMap.OrderByDescending(x => x.Value).ThenBy(y => y.Key))
            {
                text += $"{item.Key} : {item.Value}\n";
            }
            scoreTable.text = text;
        }
    }

    public void AIPositionUpdated(object sender, AIPositionUpdateEventArgs args)
    {
        //Debug.Log("AIPositionUpdate");
        //Debug.Log(args.AIID);
        if (gameClient.ai_client[args.AIID] == true && !gameClient.client_host)
        {
            players[args.AIID].transform.position = args.position;
            players[args.AIID].transform.rotation = Quaternion.Euler(args.rotation);
        }
    }

    void OnPositionUpdated(object caller, PositionUpdateEventArgs args)
    {
        //Debug.Log("GameMain PositionUpdate");
        if (!gameClient.isReadyToControl) return;
        //Debug.Log("OnPositionUpdated() 플레이어 수 : " + args.players);

        for (int i = 0; i < args.players; i++)
        {
            if (gameClient.playerRoomNum != i)
            {
                if (gameClient.ai_client[i] == false)
                {
                    players[i].transform.position = args.position[i];
                    players[i].transform.rotation = Quaternion.Euler(args.rotation[i]);
                }
            }
        }
    }

    void OnGameStateChanged(bool gameState)
    {
        if (gameState)
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }
        else
        {
            gameTimer.instance.isStart = false;
            gameOverPanel.SetActive(true);
            gameOverScoreBoard.gameObject.SetActive(true);
            timerText.gameObject.SetActive(false);

            if (scores != null)
            {
                var scoreMap = new Dictionary<string, int>();
                scoreMap.Add(gameClient.client_nick1, scores[0]);
                scoreMap.Add(gameClient.client_nick2, scores[1]);
                scoreMap.Add(gameClient.client_nick3, scores[2]);
                scoreMap.Add(gameClient.client_nick4, scores[3]);

                string text = "";
                int j = 1;

                foreach (var item in scoreMap.OrderByDescending(x => x.Value).ThenBy(y => y.Key))
                {
                    text += $"{j++}등 : {item.Key} - {item.Value}점\n";
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
        //Debug.Log("OnGameSceneChanged(): " + scene);
        //if (scene == 1)
        //{
        //    Debug.Log("SceneManager.LoadScene() Scenes/LoginScene");
        //    gameOverPanel.SetActive(false);

        //    SceneManager.LoadScene("Scenes/LoginScene", LoadSceneMode.Single);
        //}
        if (scene == 2)
        {
            //Debug.Log("SceneManager.LoadScene() Scenes/Scenes/WaitingRoomScene");
            gameOverPanel.SetActive(false);
            //SceneManager.LoadScene("Scenes/WaitingRoomScene", LoadSceneMode.Single);
            gameClient.ExitRoom();
            DontDestroyObject.instance.SceneChange();
        }
    }

    public void OnFired(object sender, FireEventArgs args)
    {
        GameObject pizzaObject = Instantiate(pizza);
        pizzaObject.transform.position = args.position;
        var dir = args.targetPosition - args.position;
        pizzaObject.transform.forward = dir;

        var animator = players[args.playerIndex].GetComponent<PizzaAnimator>();
        animator.isAnimated = true;
    }
}