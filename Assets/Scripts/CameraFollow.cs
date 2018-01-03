using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    Transform m_target;

    Vector3 m_dampVelocity;

    [SerializeField]
    Vector3 m_offset;

    IEnumerator Start()
    {
        
        yield return new WaitForSeconds(1f);
        FindLocalPlayer();
        
        if (m_target == null)
        {
            Debug.LogError("no target");
            yield break;
        }
    }

    void FindLocalPlayer()
    {
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();
        Debug.Log(players.Length);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].isLocalPlayer)
            {
                m_target = players[i].transform;
                return;
            }
        }
    }

    void LateUpdate()
    {
        if (m_target == null)
            return;
        Vector3 toPos = m_target.position + m_offset;
        transform.position = Vector3.SmoothDamp(transform.position, toPos, ref m_dampVelocity, 1f);
    }
}
