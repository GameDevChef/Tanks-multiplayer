using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerAudio : NetworkBehaviour
{
    [Header("References")]

    [SerializeField]
    PlayerSources m_playerSources;

    public void PlayFX(string _name)
    {
        CmdPlayFX(_name);
    }

    [Command]
    void CmdPlayFX(string _name)
    {
        RpcPlayFX(_name);
    }

    [ClientRpc]
    void RpcPlayFX(string _name)
    {
        PlayPlayerFX(_name);
    }

    public void PlayPlayerFX(string _name)
    {
        AudioClip clip = AudioManager.Instance.GetFX(_name);

        if (!m_playerSources.source1.isPlaying)
        {
            m_playerSources.source1.clip = clip;
            m_playerSources.source1.Play();
        }
        else if (!m_playerSources.source2.isPlaying)
        {
            m_playerSources.source2.clip = clip;
            m_playerSources.source2.Play();
        }
        else if (!m_playerSources.source3.isPlaying)
        {
            m_playerSources.source3.clip = clip;
            m_playerSources.source3.Play();
        }
        else if (!m_playerSources.source4.isPlaying)
        {
            m_playerSources.source4.clip = clip;
            m_playerSources.source4.Play();
        }
        else
        {
            m_playerSources.source1.clip = clip;
            m_playerSources.source1.Play();
        }
    }
}

[System.Serializable]
public class PlayerSources
{
    public AudioSource source1;
    public AudioSource source2;
    public AudioSource source3;
    public AudioSource source4;
}

