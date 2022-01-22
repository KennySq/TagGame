using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3D : Actor
{
    private CapsuleCollider mCapsule3D;
    public CapsuleCollider Capsule3D
    {
        get { return mCapsule3D; }
    }

    // 기본 컨트롤
    protected override void Controller()
    {
        float yDelta = Input.GetAxis("Vertical");
        float xDelta = Input.GetAxis("Horizontal");

        if(CurrentLevel.LevelStatus == Level.eLevelStatus.LEVEL_3D)
        {
            Rigidbody3D.velocity += (new Vector3(xDelta * MoveSpeed, 0.0f, yDelta * MoveSpeed));
        }
        else
        {
            Rigidbody3D.velocity += new Vector3(xDelta * MoveSpeed, 0.0f, 0.0f);

            if(Input.GetKeyDown(KeyCode.Space) && mJumpCount < MaxJumpCount)
            {
                Rigidbody3D.velocity += new Vector3(0.0f, 0.0f, JumpPower);

                mJumpCount++;
            }
        }

        return;
    }

    protected override void Awake()
    {
        base.Awake();

        // Rigidbody, Collision 초기화.
        mRigidbody3D = GetComponentInChildren<Rigidbody>();
        mRigidbody2D = GetComponentInChildren<Rigidbody2D>();

        mCapsule3D = GetComponentInChildren<CapsuleCollider>();

        mMainCamera = Camera.main;
        mMesh = mRigidbody3D.transform.Find("Mesh").gameObject;

        ActorTransform = mRigidbody3D.transform;
    }

   void Start()
    {
        
    }

    void Update()
    {
        Controller();

        if(CurrentLevel.LevelStatus == Level.eLevelStatus.LEVEL_2D)
        {
            RaycastHit hitResult;

            Vector3 rayStart = new Vector3(ActorTransform.position.x, ActorTransform.position.y, ActorTransform.position.z - (Capsule3D.height / 2.0f));
            Ray r = new Ray(ActorTransform.position, -Vector3.forward + ActorTransform.position);
            Debug.DrawLine(rayStart, ActorTransform.position - (Vector3.forward * 10), Color.red, 2.0f, false);

            if (Physics.Raycast(rayStart, ActorTransform.position - (Vector3.forward * 10), out hitResult, Mathf.Infinity))
            {
                Debug.Log(hitResult.collider.name);

                GameObject gameObject = hitResult.collider.gameObject;
                if(hitResult.distance < (mCapsule3D.radius * 2.0f))
                {
                    mJumpCount = 0;
                }
            }
        }



    }
}
