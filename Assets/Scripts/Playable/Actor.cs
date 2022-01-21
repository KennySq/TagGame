using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Actor 추상 클래스
// 플레이가 가능한 캐릭터는 이 클래스로부터 상속받습니다.
public abstract class Actor : MonoBehaviour
{
    public float MoveSpeed;

    public Level CurrentLevel;

    protected Rigidbody mRigidbody3D;
    protected Rigidbody Rigidbody3D
    {
        get { return mRigidbody3D; }
    }

    protected Rigidbody2D mRigidbody2D;
    protected Rigidbody2D Rigidbody2D
    {
        get { return mRigidbody2D; }
    }

    public GameObject RigidGameObject3D; // rigidbody 3d 그룹
    public GameObject RigidGameObject2D; // rigidbody 2d 그룹

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

    // 조작 순수 가상함수
    protected abstract void Controller();

    // 2D, 3D 상태에 따른 모드 스위칭
    public void SwitchMode(Level.eLevelStatus status)
    {
        if (status == Level.eLevelStatus.LEVEL_3D)
        {
            RigidGameObject3D.SetActive(true);
            RigidGameObject2D.SetActive(false);

            MainCamera.transform.SetParent(RigidGameObject3D.transform);
            mMesh.transform.SetParent(RigidGameObject3D.transform);

        }
        else
        {
            RigidGameObject3D.SetActive(false);
            RigidGameObject2D.SetActive(true);

            MainCamera.transform.SetParent(RigidGameObject2D.transform);
            mMesh.transform.SetParent(RigidGameObject2D.transform);
        }
    }
}
