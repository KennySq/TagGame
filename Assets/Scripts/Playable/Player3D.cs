using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3D : Actor
{
    private CapsuleCollider mCapsule3D;
    private CapsuleCollider2D mCapsule2D;

    // 기본 컨트롤
    protected override void Controller()
    {
        float yDelta = Input.GetAxis("Vertical");
        float xDelta = Input.GetAxis("Horizontal");

        if(CurrentLevel.LevelStatus == Level.eLevelStatus.LEVEL_3D)
        {
            Rigidbody3D.AddRelativeForce(new Vector3(xDelta * MoveSpeed, 0.0f, yDelta * MoveSpeed));
        }
        else
        {
            Rigidbody3D.AddRelativeForce(new Vector3(xDelta * MoveSpeed, 0.0f, 0.0f));

            if(Input.GetKeyDown(KeyCode.Space) && mbAir == false)
            {
                Rigidbody3D.AddForce(new Vector3(0.0f, 0.0f, JumpPower), ForceMode.Impulse);

                mbAir = true;
            }
        }


        return;
    }

    private void Awake()
    {
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
            Ray r = new Ray(ActorTransform.position, -Vector3.forward + ActorTransform.position);
            RaycastHit hitResult;
            Debug.DrawLine(r.origin, ActorTransform.position - (Vector3.forward * 10), Color.red, 2.0f, false);

            if (Physics.Raycast(ActorTransform.position, ActorTransform.position - (Vector3.forward * 10), out hitResult, Mathf.Infinity))
            {
                Debug.Log(hitResult.distance);
                if(hitResult.distance < (mCapsule3D.radius * 2.0f))
                {
                    mbAir = false;
                }
            }
        }

    }
}
