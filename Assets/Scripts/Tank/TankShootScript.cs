using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 1.控制炮弹威力UI变化
/// 2.在发炮口实例化子弹，赋予速度
/// 3.检测用户的按钮时长，并赋予子弹不同的发射速度
/// 4.播放发射和蓄力的音效
/// </summary>


public class TankShootScript : MonoBehaviour {

    public int m_PlayerNo = 1;
    public Rigidbody m_ShellRigidBody;
    public Transform m_FirePosition;
    public Slider m_PowerSlider;
    public AudioSource m_ShootingAS;
    public AudioClip m_ShootingClip;
    public AudioClip m_chargingClip;

    public float m_MaxPower = 30f;
    public float m_MinPower = 15f;
    private float m_MaxChargeTime = 0.75f;

    private float m_ChargeSpeed;
    private float m_CurrentPower;

    private string m_FireButton;
    private bool m_IsFired;


    private void OnEnable()
    {
        m_CurrentPower = m_MinPower;
        m_PowerSlider.value = m_CurrentPower;
    }


	// Use this for initialization
	void Start () {
        m_FireButton = "Fire" + m_PlayerNo;
        m_ChargeSpeed = (m_MaxPower - m_MinPower) / m_MaxChargeTime;

	}
	
	// Update is called once per frame
    //check how does the user press the fire button ,short or long time; 
	void Update () {
        m_PowerSlider.value = m_MinPower;

        if (m_CurrentPower >= m_MaxPower && !m_IsFired)
        {
            m_CurrentPower = m_MaxPower;
            fire();
        }
        else if(Input.GetButtonDown(m_FireButton))
        {
            m_IsFired = false;
            m_CurrentPower = m_MinPower;

            m_ShootingAS.clip = m_chargingClip;
            m_ShootingAS.Play();

        }
        else if(Input.GetButton(m_FireButton) && !m_IsFired)   // if the user held the fire button to charge power
        {
            m_CurrentPower += m_ChargeSpeed * Time.deltaTime;
            m_PowerSlider.value = m_CurrentPower;

            m_ShootingAS.clip = m_chargingClip;
            m_ShootingAS.Play();
        }
        else if(Input.GetButtonUp(m_FireButton))
        {
            fire();
        }
	}

    private void fire()
    {
        m_IsFired = true;

        //get a shell at the fire point
        Rigidbody shellInstance = Instantiate(m_ShellRigidBody,
                                              m_FirePosition.position,
                                              m_FirePosition.rotation) as Rigidbody;

        shellInstance.velocity = m_CurrentPower * m_FirePosition.forward;
        m_ShootingAS.clip = m_ShootingClip;
        m_ShootingAS.Play();

        m_CurrentPower = m_MinPower;
    }

}
