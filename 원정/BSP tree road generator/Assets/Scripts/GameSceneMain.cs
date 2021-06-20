using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    private bool materialSet = false;

    public void Awake()
    {
        gameClient.OnScoreUpdated += OnScoreUpdated;
        gameClient.OnPositionUpdated += OnPositionUpdated;
        gameClient.OnGameStateChanged += (o, e) => OnGameStateChanged(e.gameState);
        gameClient.OnFired += OnFired;

        if (gameClient.isGameStarted)
        {
            OnGameStateChanged(gameClient.isGameStarted);
        }
    }

    void OnScoreUpdated(object caller, ScoreUpdateEventArgs args)
    {
        scores = args.scores;
        string text = "점수표\n";

        for (int i = 0; i < scores.Length; i++)
        {
            if (scores[i] == -1)
            {
                continue;
            }

            text += $"플레이어 {i + 1}: {scores[i]}\n";
        }

        scoreTable.text = text;
    }

    Material decideMaterial(int playerId)
    {
        if (playerId % 4 == 0)
            return material00;
        if (playerId % 4 == 1)
            return material01;
        if (playerId % 4 == 2)
            return material02;
            
        return material03;
    }

    void OnPositionUpdated(object caller, PositionUpdateEventArgs args)
    {
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
            
            for (int i = 0; i < args.players; i++)
            {
                players[i] = Instantiate(playerObject, Vector3.zero, new Quaternion(0, 0, 0, 0));
                var characterTransform = players[i].transform.Find("Box001");
                if (characterTransform != null)
                {
                    characterTransform.gameObject.GetComponent<Renderer>().material = decideMaterial(i);
                }

            }
        }

        for (int i = 0; i < players.Length; i++)
        {
            if (gameClient.clientId == i)
            {
                players[i].SetActive(false);
                continue;
            }

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
            gameOverPanel.SetActive(false);
            timer.StartTimer();
        }
        else
        {
            gameOverPanel.SetActive(true);
            timer.stopTimer();

            var scoreMap = new Dictionary<int, int>();
            for (int i = 0; i < scores.Length; i++)
            {
                scoreMap.Add(i, scores[i]);
            }

            string text = "";
            int j = 1;

            foreach (var item in scoreMap.OrderBy(x => x.Value).Reverse())
            {
                text += $"{j++}등 플레이어 {item.Key + 1} - {item.Value}점\n";
            }

            gameOverScoreBoard.text = text;
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
