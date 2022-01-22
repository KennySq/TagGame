using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Actor 추상 클래스
// 플레이가 가능한 캐릭터는 이 클래스로부터 상속받습니다.
public abstract class Actor : MonoBehaviour
{
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

    protected Rigidbody2D mRigidbody2D;
    public GameObject RigidGameObject;

    protected Camera mMainCamera;
    public Camera MainCamera
    {
        get { return mMainCamera; }
    }

    [SerializeField]
    protected GameObject mMesh;
    public GameObject Mesh
    {
        get { return mMesh; }
    }

    [SerializeField]
    int mActorIndex = -1;

    protected virtual void Awake()
    {
        
    }

    // 조작 순수 가상함수
    protected abstract void Controller();

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
