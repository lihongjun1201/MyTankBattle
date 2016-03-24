using UnityEngine;
using System.Collections;
using UnityEngine.UI;


/// <summary>
/// 添加两个不同的坦克诞生位置，空对象+gizmos
/// 调用tankManager进行控制坦克操作
/// 
/// 利用协程实现游戏主循环
/// </summary>



public class GameManagerScript : MonoBehaviour 
{
    public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game.
    public float m_StartDelayTime = 3f;           
    public float m_EndDelayTime = 3f;               

    public CameraController m_CameraControl;     //Reference to the camera inorder to set right size and position
    public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
    public GameObject m_TankPrefab;             // Reference to the prefab the players will control.
    public TankManagerScript[] m_Tanks;               // An array of managers for enabling and disabling different aspects of the tanks.


    private int m_RoundNo;                  // Which round the game is currently on.
    private WaitForSeconds m_StartWaitTime;         
    private WaitForSeconds m_EndWaitTime;          
    private TankManagerScript m_RoundWinner;         
    private TankManagerScript m_GameWinner;          

    private void Start()
    {
        m_StartWaitTime = new WaitForSeconds(m_StartDelayTime);
        m_EndWaitTime = new WaitForSeconds(m_EndDelayTime);

        createTanks();
        setCameraTargets();

        StartCoroutine(gameLoop());
    }

    //call tankManager to create tank
    private void createTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_InstanceTank =
                Instantiate(m_TankPrefab,
                m_Tanks[i].m_BirthPoint.position,
                m_Tanks[i].m_BirthPoint.rotation) as GameObject;

            m_Tanks[i].m_PlayerNo = i + 1; 
            m_Tanks[i].setUp();
        }
    }

    private void setCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_InstanceTank.transform;
        }
        m_CameraControl.m_TrackTargets = targets;
    }


    private IEnumerator gameLoop()
    {
        yield return StartCoroutine(startingRound());
        yield return StartCoroutine(playingRound());
        yield return StartCoroutine(endingRound());

        //if one player dies ，start a new round
        if (m_GameWinner != null)
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        else
        {
            StartCoroutine(gameLoop());
        }

    }

    //the logic of starting a new round
    private IEnumerator startingRound()
    {
        Debug.Log("start a new round!");

        resetTanks();
        DisableTankControl();
        m_CameraControl.SetOriginalPositionAndSize();

        m_RoundNo++;
        m_MessageText.text = "ROUND : " + m_RoundNo;


        yield return m_StartWaitTime;
    }

    //the logic when playing a new round
    private IEnumerator playingRound()
    {
        Debug.Log("playing a new round!");

        EnableTankControl();
        m_MessageText.text = string.Empty;

        while (!HasOneTankLeft())
        {
            yield return null;
        }
           

    }

    //the logic when ending a new round
    private IEnumerator endingRound()
    {
        Debug.Log("ending a new round!");

        DisableTankControl();
        m_RoundWinner = null;
        m_RoundWinner = getRoundWinner();

        if (m_RoundWinner != null)
            m_RoundWinner.m_WinCounts++;

        m_GameWinner = getGameWinner();

        string message = EndMessage();
        m_MessageText.text = message;

        yield return m_EndWaitTime;
    }

    //check if there is only one tank left in the scene
    private bool HasOneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_InstanceTank.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }

    private TankManagerScript getRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_InstanceTank.activeSelf)
                return m_Tanks[i];
        }
        
        //an even round
        return null;
    }

    private TankManagerScript getGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_WinCounts >= m_NumRoundsToWin)
            {
                return m_Tanks[i];
            }
        }

        return null;
    }

    private string EndMessage()
    {
        // By default when a round ends there are no winners so the default end message is a draw.
        string message = "EVEN ROUND!";

        // If there is a winner then change the message to reflect that.
        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        // Add some line breaks after the initial message.
        message += "\n\n\n\n";

        // Go through all the tanks and add each of their scores to the message.
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_WinCounts + " WINS\n";
        }

        // If there is a game winner, change the entire message to reflect that.
        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }

    private void resetTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].reset();
        }
    }

    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].enableTank();
        }
    }

    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].disableTank();
        }
    }


}



