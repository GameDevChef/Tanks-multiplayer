using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour {

    [Header("References")]
    
    [SerializeField]
    GameObject m_SpawnEffectPrefab;

    [SerializeField]
    Transform m_turretTransform;

    [SerializeField]
    Transform m_bodyTransform;

    PlayerSetup m_PlayerSetup;

    PlayerHealth m_PlayerHealth;

    PlayerShoot m_PlayerShoot;

    NetworkStartPosition[] m_spawnPosArray;

    LayerMask m_groundMask;

    Rigidbody m_RB;

    [Header("Variables")]

    [SerializeField]
    float m_moveSpeed;

    [SerializeField]
    float m_bodyRotationSpeed;

    [SerializeField]
    float m_turretRotationSpeed;

    Vector3 m_moveDirection;

    Vector3 m_turretDirection;

    Vector3 m_originalSpawnPos;

    bool m_canControl;

    public int m_Score;


    void Awake()
    {
        GetReferences();      
        m_groundMask = LayerMask.GetMask("Ground");
    }

    void GetReferences()
    {
        m_PlayerHealth = GetComponent<PlayerHealth>();
        m_PlayerSetup = GetComponent<PlayerSetup>();
        m_PlayerShoot = GetComponent<PlayerShoot>();
        m_RB = GetComponent<Rigidbody>();
    }

    public override void OnStartLocalPlayer()
    {
        m_spawnPosArray = FindObjectsOfType<NetworkStartPosition>();
        m_originalSpawnPos = transform.position;
    }


    void OnDestroy()
    {
        GameManager.m_AllPlayersList.Remove(this);
    }

    void Update()
    {
        if (!isLocalPlayer || m_PlayerHealth.m_IsDead || !m_canControl)
            return;

        
        m_PlayerShoot.Tick();
    }

    public void SetState(bool _state)
    {
        m_canControl = _state;
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer || m_PlayerHealth.m_IsDead || !m_canControl)
            return;

        GetMoveDirection();
        GetTurretDirection();

        FaceDirection(m_turretTransform, m_turretDirection, m_turretRotationSpeed);
        FaceDirection(m_bodyTransform, m_moveDirection, m_bodyRotationSpeed);
        
        MovePlayer(m_bodyTransform.forward * m_moveDirection.magnitude);
    }

    void GetMoveDirection()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        m_moveDirection = new Vector3(h, 0, v);
    }

    void GetTurretDirection()
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
        _transform.rotation = Quaternion.Lerp(_transform.rotation, direction, _speed * Time.deltaTime);
    }

    void FaceDirection(Rigidbody _rb, Vector3 _dir, float _speed)
    {
        if (_dir == Vector3.zero)
            return;
        Quaternion direction = Quaternion.LookRotation(_dir);
        _rb.MoveRotation(Quaternion.Lerp(_rb.rotation, direction, _speed * Time.deltaTime));
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
        return m_PlayerSetup.m_PlayerName;
    }
}
