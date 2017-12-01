using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    public Rigidbody m_RB;

    public float m_Speed;

    public float m_Lifetime;

    public ParticleSystem m_Effect;

    public List<string> m_BounceTagList;

    public List<string> m_CollisionTagList;

    public int m_MaxBounces;

    public float m_Damage;

    public PlayerManager m_Owner;

    private void Awake()
    {
        m_RB = GetComponent<Rigidbody>();
    }

   
    private void Start()
    {
        Invoke("Destroy", m_Lifetime);
    }

    void Destroy()
    {
        if (m_Effect != null)
        {
            m_Effect.transform.parent = null;
            m_Effect.Play();
        }
        gameObject.SetActive(false);
        if (isServer)
            Destroy(gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        if(m_RB.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(m_RB.velocity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_BounceTagList.Contains(collision.gameObject.tag))
        {
            if(m_MaxBounces <= 0)
            {
                Destroy();
            }
            else
            {
                m_MaxBounces--;
            }
        }

        else if (m_CollisionTagList.Contains(collision.gameObject.tag))
        {           
            Destroy();
            PlayerHealth health = collision.transform.root.GetComponent<PlayerHealth>();
            
            if(health != null)
            {
                health.TakeDamage(m_Damage, m_Owner);
            }
        }
    }


}
