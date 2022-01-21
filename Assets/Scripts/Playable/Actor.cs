using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Actor �߻� Ŭ����
// �÷��̰� ������ ĳ���ʹ� �� Ŭ�����κ��� ��ӹ޽��ϴ�.
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

    public GameObject RigidGameObject3D; // rigidbody 3d �׷�
    public GameObject RigidGameObject2D; // rigidbody 2d �׷�

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

    // ���� ���� �����Լ�
    protected abstract void Controller();

    // 2D, 3D ���¿� ���� ��� ����Ī
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
