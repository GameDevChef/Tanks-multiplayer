using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {

    Renderer[] m_renderers;

    Collider[] m_colliders;

    Canvas[] m_canvases;

    PlayerController m_Controller;

    public float m_MaxHealth;

    [SyncVar(hook = "UpdateHealthBar")]
    float m_currentHealth;

    public GameObject m_EffectGO;

    public RectTransform m_HealthRect;
    float m_maxDeltaSize;

    PlayerController m_lastAttacker;

    [SyncVar]
    public bool m_IsDead;

    private void Awake()
    {
        m_renderers = GetComponentsInChildren<Renderer>();
        m_colliders = GetComponentsInChildren<Collider>();
        m_canvases = GetComponentsInChildren<Canvas>();
        m_Controller = GetComponent<PlayerController>();
        m_maxDeltaSize = m_HealthRect.sizeDelta.x;
    }

    private void Start()
    {
        ResetOnSpawn();
    }

    public void TakeDamage(float _amount, PlayerController _attacker)
    {
        if (!isServer)
            return;
        if(_attacker != m_Controller && _attacker != m_Controller)
        {
            m_lastAttacker = _attacker;
        }
        
        m_currentHealth -= _amount;
        if(m_currentHealth <= 0f)
        {
            if (!m_IsDead)
            {
                m_IsDead = true;
                if(m_lastAttacker != null)
                {
                    m_lastAttacker.m_Score++;
                    if(m_lastAttacker.m_Score >= GameManager.Instance.m_MaxScore)
                    {
                        GameManager.Instance.GameOver(m_lastAttacker);
                    }          
                    GameManager.Instance.UpdateScoreBoard(GameManager.Instance.m_playerCount);
                    m_lastAttacker = null;
                }
                RpcDie();
            }
                
        }
    }

    [ClientRpc]
    private void RpcDie()
    {        
        if(m_EffectGO != null)
        {
            GameObject effectInstance = Instantiate(m_EffectGO, transform.position + Vector3.up * .5f, Quaternion.identity);
            Destroy(effectInstance, 3f);
        }
        m_Controller.Disable();
        SetState(false);
        
    }

    private void SetState(bool _state)
    {
        for (int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].enabled = _state;
        }

        for (int i = 0; i < m_colliders.Length; i++)
        {
            m_colliders[i].enabled = _state;
        }

        for (int i = 0; i < m_canvases.Length; i++)
        {
            m_canvases[i].enabled = _state;
        }
    }

    public void UpdateHealthBar(float _currentHealth)
    {
        float currentDeltaSize = m_maxDeltaSize * _currentHealth / m_MaxHealth;
        m_HealthRect.sizeDelta = new Vector2(currentDeltaSize, m_HealthRect.sizeDelta.y);
    }

    public void ResetOnSpawn()
    {
        SetState(true);
        m_currentHealth = m_MaxHealth;
        m_IsDead = false;
    }
}
