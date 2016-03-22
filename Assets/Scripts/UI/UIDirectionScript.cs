using UnityEngine;
using System.Collections;

/// <summary>
/// 控制血条UI 跟随父节点坦克一起旋转
/// 
/// </summary>


public class UIDirectionScript : MonoBehaviour {

    public bool m_IsRotateWithTank = true;

    private Quaternion m_RotationOfTank;


	// Use this for initialization
	void Start () {
        m_RotationOfTank = transform.parent.rotation;

    }
	
	// Update is called once per frame,usally use with UI or Sounds
	void Update () {
        if (m_IsRotateWithTank)
        {
            transform.rotation = m_RotationOfTank;
        }
	}
}
