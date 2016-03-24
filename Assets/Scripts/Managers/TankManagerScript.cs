using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 1.让坦克管理类成为可序列化，可在监视器中显示公共变量
/// 2.在与设定的诞生位置创建不同颜色的坦克
/// 3.使能/禁止坦克运动
/// </summary>


[Serializable]
public class TankManagerScript {

    public Color m_PlayerColor;
    public Transform m_BirthPoint;

    [HideInInspector]
    public int m_PlayerNo;
    [HideInInspector]
    public string m_ColoredPlayerText;
    [HideInInspector]
    public GameObject m_InstanceTank;
    [HideInInspector]
    public int m_WinCounts;

    private TankMovement m_MovementScript;
    private TankShootScript m_ShootingScript;
    private GameObject m_CanvasGameobj;

    public void setUp()
    {
        m_MovementScript = m_InstanceTank.GetComponent<TankMovement>();
        m_ShootingScript = m_InstanceTank.GetComponent<TankShootScript>();
        m_CanvasGameobj = m_InstanceTank.GetComponentInChildren<Canvas>().gameObject;

        m_MovementScript.m_PlayerNo = m_PlayerNo;
        m_ShootingScript.m_PlayerNo = m_PlayerNo;

        // Create a string using the correct color that says 'PLAYER 1' etc based on the tank's color and the player's number.
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNo + "</color>";

        //change the tank color
        MeshRenderer[] renders = m_InstanceTank.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renders.Length; i++)
        {
           
            renders[i].material.color = m_PlayerColor;
        }
    }

    //make the tank uncontrollable,and disvisible the healthUI
    public void disableTank()
    {
        m_MovementScript.enabled = false;
        m_ShootingScript.enabled = false;
        m_CanvasGameobj.SetActive(false);
    }

    public void enableTank()
    {
        m_MovementScript.enabled = true;
        m_ShootingScript.enabled = true;
        m_CanvasGameobj.SetActive(true);
    }

    public void reset()
    {
        m_InstanceTank.transform.position = m_BirthPoint.position;
        m_InstanceTank.transform.rotation = m_BirthPoint.rotation;
        m_InstanceTank.SetActive(false);
        m_InstanceTank.SetActive(true);
    }


}
