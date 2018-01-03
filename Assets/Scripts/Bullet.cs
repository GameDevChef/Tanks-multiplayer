using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    [Header("References")]

    [SerializeField]
    ParticleSystem m_hitEffect;

    public PlayerManager m_Owner;

    [HideInInspector]
    public Rigidbody m_RB;

    [Header("Variables")]
    
    public float m_speed;

    [SerializeField]
    float m_lifetime;

    [SerializeField]
    int m_maxBounces;

    [SerializeField]
    float m_damage;

    [SerializeField]
    List<string> m_bounceTagList;

    [SerializeField]
    List<string> m_collisionTagList;

    bool m_canDestroyPlayer;


    void Awake()
    {
        m_RB = GetComponent<Rigidbody>();
        m_canDestroyPlayer = false;
    }
  
    IEnumerator InitCO()
    {
        yield return new WaitForSeconds(1f);
        m_canDestroyPlayer = true;
        yield return new WaitForSeconds(m_lifetime - 1f);
        Destroy();

    }

    void Destroy()
    {
        if (m_hitEffect != null)
        {
            m_hitEffect.transform.parent = null;
            m_hitEffect.Play();
        }
        gameObject.SetActive(false);
        if (isServer)
            Destroy(gameObject);
    }

    void OnCollisionExit(Collision _collision)
    {
        if(m_RB.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(m_RB.velocity);
        }
    }

    public void Init(PlayerManager _PlayerController)
    {
        Debug.Log(_PlayerController);
        m_Owner = _PlayerController;
        m_RB.velocity = m_speed * transform.forward;
        gameObject.SetActive(true);
        StartCoroutine(InitCO());
    }

    void OnCollisionEnter(Collision _collision)
    {
        if (m_bounceTagList.Contains(_collision.gameObject.tag))
        {
            if (m_maxBounces <= 0)
            {
                Destroy();
            }
            else
            {
                m_maxBounces--;
            }
        }

        else if (m_collisionTagList.Contains(_collision.gameObject.tag))
        {
            if (!isServer)
                return;

            PlayerHealth health = _collision.transform.root.GetComponent<PlayerHealth>();

            if (health != null)
            {
                if(health.TakeDamage(m_damage, m_Owner, health, m_canDestroyPlayer))
                {
                    Destroy();
                }
            }           
        }
    }

}
