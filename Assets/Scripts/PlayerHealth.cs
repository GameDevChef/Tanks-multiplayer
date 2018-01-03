using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {

    [Header("References")]

    [SerializeField]
    GameObject m_deathEffect;

    [SerializeField]
    RectTransform m_healthRect;

    PlayerManager m_lastAttacker;

    [HideInInspector]
    public PlayerManager m_Controller;

    Renderer[] m_renderers;

    Collider[] m_colliders;

    Canvas[] m_canvases;

    [Header("Variables")]

    [SyncVar]
    public bool m_IsDead;

    [SerializeField]
    public float m_maxHealth;

    [SyncVar(hook = "UpdateHealthBar")]
    float m_currentHealth;

    float m_maxDeltaSize;

    void Awake()
    {
        GetReferences();
        m_maxDeltaSize = m_healthRect.sizeDelta.x;
    }

    void GetReferences()
    {
        m_renderers = GetComponentsInChildren<Renderer>();
        m_colliders = GetComponentsInChildren<Collider>();
        m_canvases = GetComponentsInChildren<Canvas>();
        m_Controller = GetComponent<PlayerManager>();
    }

    void Start()
    {
        ResetOnSpawn();
    }

    public bool TakeDamage(float _amount, PlayerManager _attacker, PlayerHealth _health, bool _canDestroySelf)
    {


        if (!isServer)
            return false;

        if (!_canDestroySelf && _attacker == _health.m_Controller)
        {
            Debug.Log("player");
            return false;
        }
        else
        {
            if (_attacker != m_Controller && _attacker != m_Controller)
            {
                m_lastAttacker = _attacker;
            }

            m_currentHealth -= _amount;
            if (m_currentHealth <= 0f)
            {
                if (!m_IsDead)
                {
                    m_IsDead = true;
                    if (m_lastAttacker != null)
                    {
                        m_lastAttacker.m_Score++;
                        GameManager.Instance.UpdateScoreBoard();
                        if (m_lastAttacker.m_Score >= GameManager.Instance.m_MaxScore)
                        {
                            GameManager.Instance.GameOver(m_lastAttacker);
                        }
                        m_lastAttacker = null;
                    }
                    RpcDie();
                }
            }           
        }
        return true;
    }

    [ClientRpc]
    void RpcDie()
    {        
        if(m_deathEffect != null)
        {
            GameObject effectInstance = Instantiate(m_deathEffect, transform.position + Vector3.up * .5f, Quaternion.identity);
            Destroy(effectInstance, 3f);
        }
        m_Controller.Disable();
        SetState(false);
        
    }

    void SetState(bool _state)
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
        float currentDeltaSize = m_maxDeltaSize * _currentHealth / m_maxHealth;
        m_healthRect.sizeDelta = new Vector2(currentDeltaSize, m_healthRect.sizeDelta.y);
    }

    public void ResetOnSpawn()
    {
        SetState(true);
        m_currentHealth = m_maxHealth;
        m_IsDead = false;
    }
}
