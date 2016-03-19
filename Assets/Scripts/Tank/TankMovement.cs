using System;
using UnityEngine;

/// <summary>
///1.获取用户输入
///2.初始化音乐资源 
///3.控制前后移动
///4.控制左右转向 
/// </summary>

public class TankMovement : MonoBehaviour
{
    public int mPlayerNumber = 1;     //user no.
    public float mMoveSpeed = 12f;     // tank move speed   
    public float mTurnSpeed = 180f;       //tank turn speed
    public float mPitchRange = 0.2f;      //the different pitch of the sound of the engine 

    public AudioSource mMovementAudio;
    public AudioClip mEngineIdling;         // when a tank is stationary 
    public AudioClip mEngineDriving;         // when a tank is moving

    private string mMovementAxisName;      // the name of the vertical Axis
    private string mTurnAxisName;     // the name of the horizontal Axis
    private Rigidbody mRigidBodyTank;           //the rigidbody of tank

    private float mMovementInputValue;     // the current value of forward of backward move
    private float mTurnInputValue;
    private float mOriginalPitch;       //the pitch of sound at the start


    void Awake()
    {
        mRigidBodyTank = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        mRigidBodyTank.isKinematic = false;
        mMovementInputValue = 0f;
        mTurnInputValue = 0f;
    }

    private void Disable()
    {
        mRigidBodyTank.isKinematic = true;
    }

    private void Start()
    {
        mMovementAxisName = "Vertical" + mPlayerNumber;
        mTurnAxisName = "Horizontal" + mPlayerNumber;
        mOriginalPitch = mMovementAudio.pitch;            //get the original pitch of the sound
    }

    //every frame ,usually updates none physic things
    void Update()
    {
        //get user input
        mMovementInputValue = Input.GetAxis(mMovementAxisName);
        mTurnInputValue = Input.GetAxis(mTurnAxisName);
 
        EngineAudio();
    }

    private void EngineAudio()
    {
        //if no input, the tank is static
        if (Mathf.Abs(mMovementInputValue) < 0.1f && Mathf.Abs(mTurnInputValue) < 0.1f)
        {
            //if now is playing driving sound
            if (mMovementAudio.clip == mEngineDriving)
            {
                mMovementAudio.clip = mEngineIdling;
                mMovementAudio.pitch = UnityEngine.Random.Range(mOriginalPitch - mPitchRange, mOriginalPitch + mPitchRange);
                mMovementAudio.Play();
            }
        }
        else
        {
            if (mMovementAudio.clip == mEngineIdling)
            {
                mMovementAudio.clip = mEngineDriving;
                mMovementAudio.pitch = UnityEngine.Random.Range(mOriginalPitch - mPitchRange, mOriginalPitch + mPitchRange);
                mMovementAudio.Play();
            }
        }
    }

    void FixedUpdate()
    {
 
        move();
        turn();

    }

    private void move()
    {
        Vector3 movement = transform.forward * mMovementInputValue * mMoveSpeed* Time.deltaTime;
        mRigidBodyTank.MovePosition(mRigidBodyTank.position + movement);
    }

    private void turn()
    {
        float turn = mTurnInputValue * mTurnSpeed * Time.deltaTime;
        
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        mRigidBodyTank.MoveRotation(mRigidBodyTank.rotation * turnRotation);
    }

  
}
