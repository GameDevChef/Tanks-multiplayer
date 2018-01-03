using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {

    [Header("References")]

    [SerializeField]
    Transform m_SpawnTransform;

    [SerializeField]
    ParticleSystem m_MuzzleFlashEffect;

    [SerializeField]
    Bullet m_bulletPrefab;

    PlayerManager m_PlayerController;

    PlayerAudio m_playerAudio;

    [Header("Variables")]

    [SerializeField]
    float m_Firerate;

    float m_waitTime;

    float m_currentWaitTime;



    void Awake()
    {
        m_PlayerController = GetComponent<PlayerManager>();
        m_playerAudio = GetComponent<PlayerAudio>();
    }

    void Start()
    {
        m_waitTime = 1 / m_Firerate;
    }

    public void Tick()
    {
        m_currentWaitTime += Time.deltaTime;
        if (Input.GetMouseButton(0))
        {
            if(m_currentWaitTime > m_waitTime)
            {
                CmdShoot();
                m_currentWaitTime = 0f;
            }
        }
    }

    [Command]
    void CmdShoot()
    {
        Bullet bulletInstance = Instantiate(m_bulletPrefab, m_SpawnTransform.position, m_SpawnTransform.rotation);
        
        if(bulletInstance.m_RB != null)
        {
            bulletInstance.Init(m_PlayerController);
            //bulletInstance.m_Owner = m_PlayerController;
            //bulletInstance.m_RB.velocity = bulletInstance.m_speed * bulletInstance.transform.forward;
            //bulletInstance.gameObject.SetActive(true);
            NetworkServer.Spawn(bulletInstance.gameObject);
        }

        if (isServer)
        {
            RpcMuzzleFlash();
        }     
    }

    [ClientRpc]
    void RpcMuzzleFlash()
    {
        m_playerAudio.PlayPlayerFX("shootFX");
        if (m_MuzzleFlashEffect)
        {
            m_MuzzleFlashEffect.Play();
        }
    }
}
