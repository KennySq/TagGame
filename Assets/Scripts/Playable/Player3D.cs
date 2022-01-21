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

        Rigidbody3D.AddRelativeForce(new Vector3(xDelta * MoveSpeed, 0.0f, yDelta * MoveSpeed));

        return;
    }

    private void Awake()
    {
        // Rigidbody, Collision 초기화.
        mRigidbody3D = GetComponentInChildren<Rigidbody>();
        mRigidbody2D = GetComponentInChildren<Rigidbody2D>();

        mCapsule3D = GetComponent<CapsuleCollider>();
        mCapsule2D = GetComponent<CapsuleCollider2D>();

        mMainCamera = Camera.main;
        mMesh = mRigidbody3D.transform.Find("Mesh").gameObject;
    }

    void Start()
    {

    }

    void Update()
    {
        Controller();

    }
}
