using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Prototype.NetworkLobby;

public class GameManager : NetworkBehaviour {

    public static GameManager Instance;

    public static List<PlayerManager> m_AllPlayersList = new List<PlayerManager>();

    [Header("References")]

    [SerializeField]
    public Text m_messageText;

    [SerializeField]
    List<Text> m_nameTextlist;

    [SerializeField]
    List<Text> m_scoreTextList;

    PlayerManager m_winner;

    [Header("Variables")]

    public int m_MaxScore;

    [SyncVar]
    bool m_gameOver;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Server]
    void Start()
    {
        StartCoroutine(GameLoopCO());
    }

    IEnumerator GameLoopCO()
    {
        LobbyManager lobbyManager = LobbyManager.s_Singleton;
        if (lobbyManager != null)
        {
            while(m_AllPlayersList.Count < lobbyManager._playerNumber)
            {
                yield return null;
            }
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(StartGameCO());
            yield return StartCoroutine(PlayCO());
            yield return StartCoroutine(EndGameCO());
            StartCoroutine(GameLoopCO());
        }

        else
        {
            Debug.LogWarning("Start from lobby scene");
        }       
    }

    void EnablePlayers(bool _state)
    {
        for (int i = 0; i < m_AllPlayersList.Count; i++)
        {
            m_AllPlayersList[i].SetState(_state);
        }
    }

    IEnumerator StartGameCO()
    {
        yield return new WaitForSeconds(1f);
        ResetGame();
        RpcStartGame();
        UpdateScoreBoard();
        yield return new WaitForSeconds(1f);
    }

    [ClientRpc]
    void RpcStartGame()
    {
        EnablePlayers(true);
        UpdateMessage("Fight");            
    }

    IEnumerator PlayCO()
    {
        yield return new WaitForSeconds(1f);
        RpcPlayGame();

        while (!m_gameOver)
        {
            yield return null;
        }
    }

    [ClientRpc]
    void RpcPlayGame()
    {
        UpdateMessage("");
    }

    IEnumerator EndGameCO()
    {
        yield return new WaitForSeconds(1f);
        RpcEndGame();
        RpcUpdateMessage("Winner: " + m_winner.GetName());
        yield return new WaitForSeconds(2f);
        ResetGame();

        LobbyManager.s_Singleton._playerNumber = 0;
        LobbyManager.s_Singleton.SendReturnToLobby();
    }

    [ClientRpc]
    void RpcEndGame()
    {
        EnablePlayers(false);
    }

    void UpdateMessage(string _message)
    {
        if (m_messageText != null)
        {
            m_messageText.gameObject.SetActive(true);
            m_messageText.text = _message;
        }
    }

    [ClientRpc]
    void RpcUpdateMessage(string _message)
    {
        UpdateMessage(_message);
    }

    public void GameOver(PlayerManager _winner)
    {
        m_winner = _winner;
        m_gameOver = true;             
    }

    public void UpdateScoreBoard()
    {
        if (!isServer)
            return;

        int count = m_AllPlayersList.Count;

        string[] names = new string[count];
        int[] scores = new int[count];


        for (int i = 0; i < count; i++)
        {
            PlayerManager controller = m_AllPlayersList[i];
            names[i] = controller.GetName();
            scores[i] = controller.m_Score;
        }

        RpcUpdateScoreBoards(names, scores, count);
    }

    [ClientRpc]
    void RpcUpdateScoreBoards(string[] _playerNames, int[] _scores, int _count)
    {
        
        for (int i = 0; i < _count; i++)
        {
            if (!string.IsNullOrEmpty(_playerNames[i]))
            {
                m_nameTextlist[i].text = _playerNames[i];
            }
            
            m_scoreTextList[i].text = _scores[i].ToString();
        }
    }

    void ResetGame()
    {
        for (int i = 0; i < m_AllPlayersList.Count; i++)
        {
            m_AllPlayersList[i].m_Score = 0;

            PlayerHealth playerHealth = m_AllPlayersList[i].GetComponent<PlayerHealth>();

            if(playerHealth != null)
            {
                playerHealth.ResetOnSpawn();
            }
        }
    }
}
