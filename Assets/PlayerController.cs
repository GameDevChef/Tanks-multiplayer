using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    Vector3 m_moveDirection;
    Vector3 m_turretDirection;
    Plane m_plane;

    LayerMask m_groundMask;

    [SerializeField]
    Transform m_turretTransform;

    [SerializeField]
    Transform m_bodyTransform;

    [SerializeField]
    float m_moveSpeed;

    [SerializeField]
    float m_bodyRotationSpeed;

    [SerializeField]
    float m_turretRotationSpeed;
    Rigidbody m_RB;

    private void Awake()
    {
        m_RB = GetComponent<Rigidbody>();
        m_plane = new Plane(Vector2.up, new Vector3(0f, 2.3f, 0f));
        m_groundMask = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        GetTurretDirection();
        GetMoveDirection();
        FaceDirection(m_turretTransform, m_turretDirection, m_turretRotationSpeed);
        FaceDirection(m_bodyTransform, m_moveDirection, m_bodyRotationSpeed);
    }



    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        GetMoveDirection();
        MovePlayer(m_moveDirection);
    }

    private void GetMoveDirection()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        m_moveDirection = new Vector3(h, 0, v);
    }

    private void GetTurretDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 200f, m_groundMask))
        {
            Vector3 dir = hit.point - m_turretTransform.position;
            m_turretDirection = dir;
            m_turretDirection.y = 0f;
            
        }
    }

    void MovePlayer(Vector3 _dir)
    {
        if (_dir == Vector3.zero)
            return;
        Vector3 direction = _dir * m_moveSpeed;
        direction =  Vector3.ClampMagnitude(direction, m_moveSpeed);
        m_RB.velocity = direction;
       
    }

    void FaceDirection(Transform _transform, Vector3 _dir, float _speed)
    {
        if (_dir == Vector3.zero)
            return;
        Quaternion direction = Quaternion.LookRotation(_dir);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, direction, _speed * Time.deltaTime);
    }
}
