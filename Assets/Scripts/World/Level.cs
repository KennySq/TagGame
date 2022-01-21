using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{

    public enum eLevelStatus
    {
        LEVEL_2D = 0,
        LEVEL_3D = 1,
    }
   
    private eLevelStatus mLevelStatus;
    public eLevelStatus LevelStatus
    {
        get { return mLevelStatus; }
    }

    private Camera mMainCamera;
    public Camera MainCamera
    {
        get { return mMainCamera; }
    }

    void SwitchLevelStatus()
    {
        if (mLevelStatus == eLevelStatus.LEVEL_2D)
        {
            mLevelStatus = eLevelStatus.LEVEL_3D;
        }
        else
        {
            mLevelStatus = eLevelStatus.LEVEL_2D;
        }
    }

    private void Awake()
    {
        mMainCamera = Camera.main;
    }


    private void Update()
    {
        
    }

}
