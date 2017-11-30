using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour {

    [SyncVar(hook = "UpdateColor")]
    public Color m_PlayerColor;

    public string m_BaseName;

    [SyncVar(hook = "UpdateName")]
    public int m_PlayerNum;

    public Text m_PlayernameText;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (m_PlayernameText != null)
        {
            m_PlayernameText.enabled = false;          
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetupPlayer();
        
    }

    private void Start()
    {
        
        if (!isLocalPlayer)
        {
            UpdateColor(m_PlayerColor);
            UpdateName(m_PlayerNum);
        }
        
    }

    void UpdateColor(Color _color)
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].material.color = _color;
        }
    }

    void UpdateName(int _num)
    {
        if (m_PlayernameText != null)
        {
            m_PlayernameText.enabled = true;
            m_PlayernameText.text = m_BaseName + " " + _num;
        }
    }

    [Command]
    void CmdSetupPlayer()
    {
        GameManager.Instance.AddPlayer(this);
    }

    

  
}
