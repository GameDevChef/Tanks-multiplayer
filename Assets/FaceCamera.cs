using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

    public Camera m_Camera;

    private void Awake()
    {
        m_Camera = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
    }
}
