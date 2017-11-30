using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

    public static GameManager Instance;

    public List<PlayerController> m_AllPlayersList = new List<PlayerController>();

    public List<Text> m_NameTextsList = new List<Text>();

    public List<Text> m_PlayerScoresList = new List<Text>();

    public Text m_MessageText;

    public int m_MinPlayers;

    public int m_MaxPlayers;

    public int m_MaxScore;

    [SyncVar]
    bool  m_gameOver;

    PlayerController m_Winner;
    
    [SyncVar (hook = "UpdateScoreBoard")]
    public int m_playerCount;

    public Color[] m_PlayerColors;

    private void Awake()
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

    private void Start()
    {
        StartCoroutine(GameLoopCO());
    }

    private IEnumerator GameLoopCO()
    {
        yield return StartCoroutine(EnterLobbyCO());
        yield return StartCoroutine(PlayCO());
        yield return StartCoroutine(EndGameCO());
        StartCoroutine(GameLoopCO());
    }

    private IEnumerator EnterLobbyCO()
    {
        DisablePlayers();
        while (m_playerCount < m_MinPlayers)
        {
            UpdateMessage("Waiting for players");
            yield return null;
        }
    }

    void UpdateMessage(string _message)
    {
        if (!isServer)
            return;
        RpcUpdateMessage(_message);
    }

    [ClientRpc]
    void RpcUpdateMessage(string _message)
    {
        if (m_MessageText != null)
        {
            m_MessageText.gameObject.SetActive(true);
            m_MessageText.text = _message;
        }
    }


    public void GameOver(PlayerController _winner)
    {
        m_Winner = _winner;
        m_gameOver = true;
        
      
    }

    private IEnumerator PlayCO()
    {
        DisablePlayers();
        UpdateScoreBoard(m_playerCount);
        yield return new WaitForSeconds(1f);
        UpdateMessage("3");
        yield return new WaitForSeconds(1f);
        UpdateMessage("2");
        yield return new WaitForSeconds(1f);
        UpdateMessage("1");
        yield return new WaitForSeconds(1f);
        UpdateMessage("Fight");
        yield return new WaitForSeconds(1f);
        UpdateMessage("");

        EnablePlayers();
        while (!m_gameOver)
        {
            yield return null;
        }
        
    }

    private IEnumerator EndGameCO()
    {
        DisablePlayers();
        yield return new WaitForSeconds(1f);
        if (m_Winner != null)
        {
            UpdateMessage("Winner: " + m_Winner.GetName());
        }
        
        yield return new WaitForSeconds(2f);     
        UpdateMessage("Restarting...");
        yield return new WaitForSeconds(2f);
        ResetGame();
        yield return null;
    }

    [ClientRpc]
    void RpcSetPlayersState(bool _state)
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < allPlayers.Length; i++)
        {
            allPlayers[i].enabled = _state;
        }
    }

    void EnablePlayers()
    {
        if (!isServer)
            return;
        RpcSetPlayersState(true);
    }


    void DisablePlayers()
    {
        if (!isServer)
            return;
        RpcSetPlayersState(false);
    }

    public void AddPlayer(PlayerSetup _setup)
    {
        if(m_playerCount < m_MaxPlayers)
        {
            m_AllPlayersList.Add(_setup.GetComponent<PlayerController>());
            _setup.m_PlayerColor = m_PlayerColors[m_playerCount];
            _setup.m_PlayerNum = m_playerCount + 1;
            m_playerCount++;
            
            
        }
    }

    public void UpdateScoreBoard(int _count)
    {
        if (!isServer)
            return;

        string[] names = new string[_count];
        int[] scores = new int[_count];


        for (int i = 0; i < _count; i++)
        {
            PlayerController controller = m_AllPlayersList[i];
            names[i] = controller.GetName();
            scores[i] = controller.m_Score;
        }

        RpcUpdateScoreBoards(names, scores, _count);
    }

    [ClientRpc]
    void RpcUpdateScoreBoards(string[] _playerNames, int[] _scores, int _count)
    {
        for (int i = 0; i < _count; i++)
        {
            if (!string.IsNullOrEmpty(_playerNames[i]))
            {
                m_NameTextsList[i].text = _playerNames[i];
            }
            m_PlayerScoresList[i].text = _scores[i].ToString();
        }
    }

    void ResetGame()
    {
        if (!isServer)
            return;

        RpcResetGame();
        m_Winner = null;
        m_gameOver = false;
    }

    [ClientRpc]
    private void RpcResetGame()
    {
        PlayerController[] controllers = FindObjectsOfType<PlayerController>();

        for (int i = 0; i < controllers.Length; i++)
        {
            controllers[i].m_Score = 0;
        }
    }
}
