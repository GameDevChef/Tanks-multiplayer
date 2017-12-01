using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour {

    PlayerSetup m_PlayerSetup;
    PlayerHealth m_PlayerHealth;
    PlayerShoot m_PlayerShoot;

    Vector3 m_moveDirection;
    Vector3 m_turretDirection;
    Vector3 m_originalSpawnPos;

    NetworkStartPosition[] m_spawnPosArray;

    LayerMask m_groundMask;

    public GameObject m_SpawnEffectPrefab;

    public int m_Score;

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
        m_PlayerHealth = GetComponent<PlayerHealth>();
        m_PlayerSetup = GetComponent<PlayerSetup>();
        m_PlayerShoot = GetComponent<PlayerShoot>();
        m_RB = GetComponent<Rigidbody>();
        m_groundMask = LayerMask.GetMask("Ground");

    }

    public override void OnStartLocalPlayer()
    {
        m_spawnPosArray = FindObjectsOfType<NetworkStartPosition>();
        m_originalSpawnPos = transform.position;
    }

    private void Update()
    {
        if (!isLocalPlayer || m_PlayerHealth.m_IsDead)
            return;

        GetTurretDirection();
        GetMoveDirection();
        FaceDirection(m_turretTransform, m_turretDirection, m_turretRotationSpeed);
        FaceDirection(m_bodyTransform, m_moveDirection, m_bodyRotationSpeed);
        m_PlayerShoot.Tick();
    }



    private void FixedUpdate()
    {
        if (!isLocalPlayer || m_PlayerHealth.m_IsDead)
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

    public void Disable()
    {
        StartCoroutine(RespawnCO());       
    }

    IEnumerator RespawnCO()
    {

        transform.position = GetSpawnPosition() ;
        m_RB.velocity = Vector3.zero;
        yield return new WaitForSeconds(2f);
        if (m_SpawnEffectPrefab != null)
        {
            GameObject effectinstance = Instantiate(m_SpawnEffectPrefab, transform.position = Vector3.up * .5f, Quaternion.identity);
            Destroy(effectinstance, 3f);
        }
        yield return new WaitForSeconds(.5f);
        m_PlayerHealth.ResetOnSpawn();
        
    }

    Vector3 GetSpawnPosition()
    {
        if (m_spawnPosArray != null && m_spawnPosArray.Length > 0)
        {
            NetworkStartPosition pos = m_spawnPosArray[UnityEngine.Random.Range(0, m_spawnPosArray.Length)];
            return pos.transform.position;
        }
        return m_originalSpawnPos;
    }

    public string GetName()
    {
        return m_PlayerSetup.m_PlayernameText.text;
    }
}
