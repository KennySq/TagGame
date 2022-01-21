using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 현재 레벨을 관리하는 클래스
// 카메라, 2D,3D 상태 관리
public class Level : MonoBehaviour
{
    struct CameraSetup
    {
        public CameraSetup(Vector3 pos, Quaternion rot, float fov)
        {
            PositionOffset = pos;
            RotationOffset = rot;
            FOV = fov;
        }
        public Vector3 PositionOffset;
        public Quaternion RotationOffset;
        public float FOV;
    }

    public enum eLevelStatus
    {
        LEVEL_2D = 0,
        LEVEL_3D = 1,
    }

    [SerializeField]
    private eLevelStatus mLevelStatus;
    public eLevelStatus LevelStatus
    {
        get { return mLevelStatus; }
    }

    private Camera mMainCamera;
    public Camera MainCamera
    {
        get { return mMainCamera; }
        set { mMainCamera = value; }
    }

    // pre-set 카메라 옵션
    CameraSetup CamSetup2D = new CameraSetup(new Vector3(0, 0, -5.5f), Quaternion.Euler(0, 0, 0), 0.0f);
    CameraSetup CamSetup3D = new CameraSetup(new Vector3(0, 4.5f, -5.5f), Quaternion.Euler(30, 0, 0), 60.0f);

    private GameObject mLocalPlayer;
    public GameObject LocalPlayer
    {
        get { return mLocalPlayer; }
        set { mLocalPlayer = value; }
    }

    // 레벨 상태 스위칭 (2D <-> 3D)
    public void SwitchLevelStatus()
    {
        if (mLevelStatus == eLevelStatus.LEVEL_2D)
        {
            mLevelStatus = eLevelStatus.LEVEL_3D;

            MainCamera.transform.position = CamSetup3D.PositionOffset;
            MainCamera.transform.rotation = CamSetup3D.RotationOffset;
            MainCamera.fieldOfView = CamSetup3D.FOV;
            MainCamera.orthographic = false;
        }
        else
        {
            mLevelStatus = eLevelStatus.LEVEL_2D;

            MainCamera.transform.position = CamSetup2D.PositionOffset;
            MainCamera.transform.rotation = CamSetup2D.RotationOffset;
            MainCamera.orthographic = true;
        }

        Actor localActor = mLocalPlayer.GetComponent<Actor>();
        localActor.SwitchMode(mLevelStatus);
    }

    private void Awake()
    {
        mMainCamera = Camera.main;
    }


    private void Update()
    {

    }

}
