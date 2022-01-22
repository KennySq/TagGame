using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Player3D : Actor
{
    private readonly Notifier<bool> inputMovement = new Notifier<bool>();

    private CapsuleCollider mCapsule3D;
    public CapsuleCollider Capsule3D
    {
        get { return mCapsule3D; }
    }

    [SerializeField]
    private Animator animator;

    public float GravityScalar = -2.45f;
    Vector3 Gravity2D;
    Vector3 Gravity3D;

    private int mLastFacing = 0;
    private Vector3 mFacingDirection;

    // 기본 컨트롤
    protected override void Controller()
    {
        float yDelta = Input.GetAxis("Vertical");
        float xDelta = Input.GetAxis("Horizontal");

        if(CurrentLevel.LevelStatus == Level.eLevelStatus.LEVEL_3D)
        {
            Rigidbody3D.velocity += (new Vector3(xDelta * MoveSpeed, 0.0f, yDelta * MoveSpeed ));

            if(Mathf.Abs(xDelta) >= Mathf.Epsilon)
            {
                mFacingDirection = new Vector3(Mathf.Sign(xDelta), 0, 0);

            }
            if (Mathf.Abs(yDelta) >= Mathf.Epsilon)
            {
                mFacingDirection = new Vector3(0, 0, Mathf.Sign(yDelta));
            }
        }
        else
        {
            mLastFacing = (int)Mathf.Sign(xDelta);

            Rigidbody3D.velocity += new Vector3(xDelta * MoveSpeed, 0.0f, 0.0f);

            if(Input.GetKeyDown(KeyCode.Space) && mJumpCount < MaxJumpCount)
            {
                Rigidbody3D.velocity += new Vector3(0.0f, 0.0f, JumpPower);

                mJumpCount++;
            }
        }

        

        Attack();

        inputMovement.CurrentData = new Vector2(xDelta, yDelta).magnitude > 0.2f;

        return;
    }

    protected override void Awake()
    {
        base.Awake();

        // Rigidbody, Collision 초기화.
        mRigidbody3D = GetComponentInChildren<Rigidbody>();
        mCapsule3D = GetComponentInChildren<CapsuleCollider>();

        RigidGameObject = transform.Find("Rigid3D").gameObject;
        mMesh = mRigidbody3D.transform.Find("Mesh").gameObject;

        ActorTransform = mRigidbody3D.transform;
        inputMovement.OnDataChanged += Movement_OnDataChanged;

        Gravity2D = new Vector3(0.0f, 0.0f, GravityScalar);
        Gravity3D = new Vector3(0.0f, GravityScalar, 0.0f);
    }

    private void Movement_OnDataChanged(bool isMove)
    {
        //공격시 아래의 애니메이션 코드를 실행해 주세요
        //animator.SetTrigger("attack");

        if (isMove)
        {
            animator.SetTrigger("walk_start");
        }
        else
        {
            animator.SetTrigger("walk_end");
        }
    }

    private void Attack()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetTrigger("attack");

            Vector3 origin = ActorTransform.position;
            Vector3 direction = mFacingDirection * 3.0f;

            RaycastHit hitResult;
            Ray r = new Ray(origin, direction);
            Debug.DrawRay(origin, direction, Color.blue, 1.0f);

            if (Physics.Raycast(r, out hitResult, 3.0f))
            {
                Debug.Log("remote player : " + CurrentLevel.RemotePlayer);
                Debug.Log("collider : " + hitResult.collider.transform.parent);
                if (hitResult.collider.transform.parent.gameObject == CurrentLevel.RemotePlayer)
                {
                    Rigidbody RemoteRigid = CurrentLevel.RemotePlayer.GetComponentInChildren<Rigidbody>();
                    RemoteRigid.AddExplosionForce(10.0f, direction, 1.0f);
                }
            }

        }
    }

    void Update()
    {
        if(IsLocalPlayer == true)
        {
            Debug.Log("Local");
            Controller();
        }

        if(mActorIndex == 1)
        {
            mMesh.transform.LookAt(MainCamera.transform);
        }

        Debug.Log(CurrentLevel);

        if (CurrentLevel.LevelStatus == Level.eLevelStatus.LEVEL_2D)
        {
            RaycastHit hitResult;

            Vector3 rayStart = new Vector3(ActorTransform.position.x, ActorTransform.position.y, ActorTransform.position.z - (Capsule3D.height / 2.0f));
            Ray r = new Ray(ActorTransform.position, -Vector3.forward + ActorTransform.position);
            Debug.DrawLine(rayStart, ActorTransform.position - (Vector3.forward * 10), Color.red, 2.0f, false);

            if (Physics.Raycast(rayStart, ActorTransform.position - (Vector3.forward * 10), out hitResult, Mathf.Infinity))
            {
                GameObject gameObject = hitResult.collider.gameObject;
                if(hitResult.distance < (mCapsule3D.radius * 2.0f))
                {
                    mJumpCount = 0;
                }
            }

            Rigidbody3D.velocity += Gravity2D * Time.deltaTime;
        }
        else
        {
            Rigidbody3D.velocity += Gravity3D * Time.deltaTime;

        }





    }
}
