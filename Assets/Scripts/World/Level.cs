using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private float mSwitchTimer = 0.0f;
    public float SwitchTimer
    {
        get { return mSwitchTimer; }
    }

    public float SwitchTime = 60.0f;

    public TextMeshProUGUI TimerText;
    
    // pre-set 카메라 옵션
    CameraSetup CamSetup2D = new CameraSetup(new Vector3(0, 30.0f, 0.0f), Quaternion.Euler(90, 0, 0), 0.0f);
    CameraSetup CamSetup3D = new CameraSetup(new Vector3(0, 4.5f, -10.0f), Quaternion.Euler(45, 0, 0), 60.0f);

    public const float GravityScalar = -64.0f;
    Vector3 Gravity2D = new Vector3(0.0f, 0.0f, GravityScalar * 2);
    Vector3 Gravity3D = new Vector3(0.0f, GravityScalar, 0.0f);

    private CameraSetup mCurrentCameraOption;

    [SerializeField]
    private GameObject mLocalPlayer;
    public GameObject LocalPlayer
    {
        get { return mLocalPlayer; }
        set
        {
            mLocalPlayer = value;
            mLocalActor = mLocalPlayer.GetComponent<Actor>();
        }
    }

    private GameObject mRemotePlayer;
    public GameObject RemotePlayer
    {
        get { return mRemotePlayer; }
        set
        {
            mRemotePlayer = value;
        }
    }

    private Actor mLocalActor;
    public Actor LocalActor
    {
        get { return mLocalActor; }
    }

    // 레벨 상태 스위칭 (2D <-> 3D)
    public void SwitchLevelStatus()
    {
        if (mLevelStatus == eLevelStatus.LEVEL_2D)
        {
            mLevelStatus = eLevelStatus.LEVEL_3D;
            Physics.gravity = Gravity3D;

            mCurrentCameraOption = CamSetup3D;
            MainCamera.orthographic = false;
        }
        else
        {
            mLevelStatus = eLevelStatus.LEVEL_2D;
            Physics.gravity = Gravity2D;

            mCurrentCameraOption = CamSetup2D;
            MainCamera.orthographic = true;
        }

        Actor localActor = mLocalPlayer.GetComponent<Actor>();
        localActor.SwitchMode(mLevelStatus);

        Actor remoteActor = mRemotePlayer.GetComponent<Actor>();
        remoteActor.SwitchMode(mLevelStatus);

    }

    private void Awake()
    {
        mCurrentCameraOption = CamSetup2D;

        // Temporal Code
        RemotePlayer = GameObject.Find("Player2D");
        // -------------
    }

    private void Update()
    {
        if(mSwitchTimer >= SwitchTime)
        {
            SwitchLevelStatus();
            mSwitchTimer = 0;
        }

        mSwitchTimer += Time.deltaTime;

        TimerText.text = (SwitchTime - mSwitchTimer).ToString("N2");

        GameObject rigidObject = mLocalActor.RigidGameObject;
        Vector3 remotePosition = RemotePlayer.transform.position;
        Vector3 localPosition = rigidObject.transform.position;
        Quaternion targetRotation = mCurrentCameraOption.RotationOffset;
        Vector3 distanceVector = (localPosition + remotePosition) * 0.5f;
        float distance = Vector3.Distance(localPosition, remotePosition);

        mMainCamera.transform.rotation = Quaternion.Slerp(mMainCamera.transform.rotation, targetRotation, 0.1f);

        if (LevelStatus == eLevelStatus.LEVEL_3D)
        {
            Vector3 targetPosition;

            targetPosition = distanceVector + CamSetup3D.PositionOffset;
            Debug.DrawLine(mMainCamera.transform.position, targetPosition, Color.red, 1.0f, false);

            mMainCamera.transform.position = Vector3.Lerp(mMainCamera.transform.position, targetPosition, 0.1f);
            // mMainCamera.transform.position = new Vector3(mMainCamera.transform.position.x, mMainCamera.transform.position.y, length);
            mMainCamera.transform.position -= mMainCamera.transform.TransformDirection(mMainCamera.transform.forward * distance);

            mMainCamera.fieldOfView = Mathf.Lerp(mMainCamera.fieldOfView, mCurrentCameraOption.FOV, 0.1f);
        }
        else
        {
            Vector3 targetPosition = distanceVector + CamSetup2D.PositionOffset;

            mMainCamera.transform.position = Vector3.Lerp(mMainCamera.transform.position, targetPosition, 0.1f);
            mMainCamera.transform.position = new Vector3(mMainCamera.transform.position.x, 30.0f, mMainCamera.transform.position.z);
            mMainCamera.orthographicSize = Mathf.Lerp(mMainCamera.orthographicSize, distance / 1.414f, 0.1f);
        }



    }

}
