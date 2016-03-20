using System;
using UnityEngine;

/// <summary>
///1.获取用户输入
///2.初始化音乐资源 
///3.控制前后移动
///4.控制左右转向 
///5.控制坦克音效转变
/// </summary>

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNo = 1;
    public float m_MoveSpeed = 12f;
    public float m_TurnSpeed = 180f;

    //sound sources
    public AudioSource m_AudioSource;
    public AudioClip m_EngineIding;
    public AudioClip m_EngineDriving;
    public float m_PitchRange = 0.2f;
    

    private Rigidbody m_RigidBodyTank;

    private float m_MoveInputValue;
    private float m_TurnInputValue;
    private string m_MoveAxixOfPlayer;
    private string m_TurnAxixOfPlayer;
    private float m_originPitch;

    /// <summary>
    ///recall before start ,basic init
    /// </summary>
    void Awake()
    {
        m_RigidBodyTank = GetComponent<Rigidbody>();
        m_MoveInputValue = 0f;
        m_TurnInputValue = 0f;
    }

    private void OnEnable()
    {
        m_RigidBodyTank.isKinematic = false;
    }

    private void Disable()
    {
        m_RigidBodyTank.isKinematic = true;
    }

    private void Start()
    {
        m_MoveAxixOfPlayer = "Vertical" + m_PlayerNo;
        m_TurnAxixOfPlayer = "Horizontal" + m_PlayerNo;
        m_originPitch = m_AudioSource.pitch;
    }

    void Update()
    {
        m_MoveInputValue = Input.GetAxis(m_MoveAxixOfPlayer);     // press w / s
        m_TurnInputValue = Input.GetAxis(m_TurnAxixOfPlayer);   // press a /a

        changeEngineSound();
    }

    //change the sound of engine with pitch
    private void changeEngineSound()
    {
        // when no user's input,the tank is idle
        if (Mathf.Abs(m_MoveInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            //if now is playing driving sound, change to play idlesound
            if (m_AudioSource.clip == m_EngineDriving)
            {
                m_AudioSource.clip = m_EngineIding;
                //change the pitch
                m_AudioSource.pitch = UnityEngine.Random.Range(m_originPitch - m_PitchRange, m_originPitch + m_PitchRange);
                //play drivingsound
                m_AudioSource.Play();
            }
        }
        else
        {
            if (m_AudioSource.clip == m_EngineIding)
            {
                m_AudioSource.clip = m_EngineDriving;
                //change the pitch
                m_AudioSource.pitch = UnityEngine.Random.Range(m_originPitch - m_PitchRange, m_originPitch + m_PitchRange);
                //play drivingsound
                m_AudioSource.Play();
            }
        }
    }

    //always updates physic things in fixed physic time
    void FixedUpdate()
    {
        //move forward or backward
        m_RigidBodyTank.MovePosition(m_RigidBodyTank.position + (transform.forward * m_MoveSpeed * m_MoveInputValue * Time.deltaTime));

        //turn around
        float turnValue = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turn = Quaternion.Euler(0f, turnValue, 0f);
        m_RigidBodyTank.MoveRotation(m_RigidBodyTank.rotation * turn);
       
    
    }
}



