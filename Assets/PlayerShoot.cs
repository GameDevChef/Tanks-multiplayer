using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {

    PlayerController m_PlayerController;

    public float m_Firerate;
    float m_waitTime;
    float m_currentWaitTime;

    public Bullet m_bulletPrefab;

    public Transform m_SpawnTransform;

    public ParticleSystem m_MuzzleFlashEffect;

    private void Awake()
    {
        m_PlayerController = GetComponent<PlayerController>();
    }

    private void Start()
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
    private void CmdShoot()
    {
        Bullet bulletInstance = Instantiate(m_bulletPrefab, m_SpawnTransform.position, m_SpawnTransform.rotation);
        
        if(bulletInstance.m_RB != null)
        {
            bulletInstance.m_RB.velocity = bulletInstance.m_Speed * bulletInstance.transform.forward;
            bulletInstance.m_Owner = m_PlayerController;
            NetworkServer.Spawn(bulletInstance.gameObject);
        }

        if (!isServer)
        {
            Debug.Log("nie serwer");
        }
            
        else
        {
            RpcMuzzleFlash();
        }

        
    }

    [ClientRpc]
    void RpcMuzzleFlash()
    {
        if (m_MuzzleFlashEffect)
        {
            m_MuzzleFlashEffect.Play();
        }
    }
}
