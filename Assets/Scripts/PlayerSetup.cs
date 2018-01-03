using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Text m_PlayernameText;

    [SyncVar(hook = "UpdateColor")]
    public Color m_PlayerColor;

    [SyncVar(hook = "UpdateName")]
    public string m_PlayerName;

    public override void OnStartClient()
    {
        base.OnStartClient();

       // UpdateColor(m_PlayerColor);
        UpdateName(m_PlayerName);

        if (isServer)
            return;

        PlayerManager playerManager = GetComponent<PlayerManager>();

        if (playerManager != null)
        {
            GameManager.m_AllPlayersList.Add(playerManager);
        }
    }

    private void Start()
    { 
                         
    }


    void UpdateColor(Color _color)
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].material.color = _color;
        }
    }

    void UpdateName(string m_Name)
    {
        if (m_PlayernameText != null)
        {
            m_PlayernameText.enabled = true;
            m_PlayernameText.text = m_Name;
        }
    }


    

  
}
