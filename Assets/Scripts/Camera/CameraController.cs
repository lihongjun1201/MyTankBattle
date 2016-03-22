using UnityEngine;
using System.Collections;

/// <summary>
/// 父节点CamerRig计算出两坦克间的中点
/// 子节点maincamera 不断变化Size
/// 1.控制镜头跟踪坦克移动
/// 2.当两架坦克距离变化时，要拉近或拉远镜头（利用camera size参数）
/// 根据两架坦克的距离，计算出中点，让相机保持在该中点
/// 并计算出相对应的zoom size
/// 
/// 相机的size = 屏幕中点Y半轴距离
/// 屏幕中点X半轴距离 = size * aspect（如aspect = 16 / 9） 
/// </summary>

public class CameraController : MonoBehaviour {

    public float m_SmoothTime = 0.2f;   //the zoom time of camera
    public float m_MinSize = 6.5f;    //the camera's minest zoomsize 
    public float m_ScreenEdgeBuffer = 4f;    // Space between the top/bottom most target and the screen edge.

    public Transform[] m_TrackTargets ;

    private Camera m_Camera;
    private Vector3 m_MiddlePosOfTanks;
    private Vector3 m_MoveSpeed;
    private float m_ZoomSpeed;


    // Use this for initialization
    void Awake () {
        //the main camera is a child of the cameraRig
        m_Camera = GetComponentInChildren<Camera>();
	}
	

    //usually use with camera，update when all other things Finnish
    void LateUpdate()
    {
        moveCamera();
        zoomCamerSize();
    }

    private void moveCamera() 
    {
        getMiddlePosition();
        transform.position = Vector3.SmoothDamp(transform.position, m_MiddlePosOfTanks, ref m_MoveSpeed, m_SmoothTime);
    }

    private void getMiddlePosition()
    {
        Vector3 midpos = new Vector3();
        int tankcount = 0;
        foreach (var tank in m_TrackTargets)
        {
            tankcount++;
            if (!tank.gameObject.activeSelf)
                continue;

            midpos += tank.position;
        }
        
        if (tankcount > 0)
        {
            midpos /= tankcount;
        }
        midpos.y = this.transform.position.y;
        m_MiddlePosOfTanks = midpos;

        //Debug.Log(midpos);
    }


    private void zoomCamerSize()
    {
        float suitableZoomSize = getSuitableZoomSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, suitableZoomSize, ref m_ZoomSpeed, m_SmoothTime);
    }

    //Notes：localPosition   Position of the transform relative to the parent transform.
    private float getSuitableZoomSize()
    {
        // Find the position the camera rig is moving towards in its local space.
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_MiddlePosOfTanks);

        float zoomSize = 0;
        for (int i = 0; i < m_TrackTargets.Length; i++)
        {
            // ... and if they aren't active continue on to the next target.
            if (!m_TrackTargets[i].gameObject.activeSelf)
                continue;

            // get the local position of the tank
            Vector3 tankLocalPos = transform.InverseTransformPoint(m_TrackTargets[i].position);

            // Find the position of the target from the desired position of the camera's local space.
            Vector3 desiredPosToTarget = tankLocalPos - desiredLocalPos;

            // Choose the largest out of the current size and the distance of the tank 'up' or 'down' from the camera.
            zoomSize = Mathf.Max(zoomSize, Mathf.Abs(desiredPosToTarget.y));

            // Choose the largest out of the current size and the calculated size based on the tank being to the left or right of the camera.
            zoomSize = Mathf.Max(zoomSize, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);

        }

        // Add the edge buffer to the size., if not plus it ,the two tank will only at the edge of both oppsite sides;
        zoomSize += m_ScreenEdgeBuffer;
        // Make sure the camera's size isn't below the minimum.
        zoomSize = Mathf.Max(zoomSize, m_MinSize);
        return zoomSize;
    }
     
    public void SetOriginalPositionAndSize()
    {
        // Find the desired position.
        getMiddlePosition();

        // Set the camera's position to the desired position without damping.
        transform.position = m_MiddlePosOfTanks;

        // Find and set the required size of the camera.
        m_Camera.orthographicSize = getSuitableZoomSize();
    }


}
