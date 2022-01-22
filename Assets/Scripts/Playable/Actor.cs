using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

// Actor 추상 클래스
// 플레이가 가능한 캐릭터는 이 클래스로부터 상속받습니다.
public abstract class Actor : MonoBehaviour
{
    [SerializeField]
    public static readonly NotifierClass<Actor> LocalPlayer = new NotifierClass<Actor>();

    public float MoveSpeed;
    public float JumpPower;

    protected int mJumpCount = 0;
    public int MaxJumpCount = 2;

    public Level CurrentLevel;

    protected Transform ActorTransform;

    protected Rigidbody mRigidbody3D;
    protected Rigidbody Rigidbody3D
    {
        get { return mRigidbody3D; }
    }

    public GameObject RigidGameObject;

    public Camera MainCamera;

    [SerializeField]
    protected GameObject mMesh;
    public GameObject Mesh
    {
        get { return mMesh; }
    }

    [SerializeField]
    int mActorIndex = -1;

    [Header("Network")]

    [SerializeField]
    private TestSendPosition mSendPosition;
    public TestSendPosition SendPosition { get => mSendPosition; }

    [SerializeField]
    private TestReceivePosition mReceivePosition;
    public TestReceivePosition ReceivePosition { get => mReceivePosition; }

    protected bool IsLocalPlayer { get { return LocalPlayer.CurrentData.gameObject == gameObject; } }

    protected virtual void Awake()
    {
        //LocalPlayerIndex is thisCharacter
        if (mActorIndex == LocalPlayerData.Instance.characterIndex.CurrentData)
        {
            SendPosition.enabled = true;
            LocalPlayer.CurrentData = this;
        }
        else // otherWise
        {
            ReceivePosition.enabled = true;

            
        }
    }

    // 조작 순수 가상함수
    protected abstract void Controller();

    protected void Attack()
    {
        //if(Input.GetKeyDown(KeyCode.))
        //{

        //}
    }

    static readonly Quaternion Rotation2D = Quaternion.Euler(90.0f, 0.0f, 0.0f);

    // 2D, 3D 상태에 따른 모드 스위칭
    public void SwitchMode(Level.eLevelStatus status)
    {
        if(status == Level.eLevelStatus.LEVEL_2D)
        {
            Rigidbody3D.rotation = Rotation2D;
        }
        else
        {
            Rigidbody3D.rotation = Quaternion.identity;
        }

    }
}
