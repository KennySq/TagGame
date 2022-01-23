using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using Utils;

public class Player3D : Actor
{
    private readonly Notifier<bool> inputMovement = new Notifier<bool>();
    private readonly Notifier<bool> GroundState = new Notifier<bool>();



    [SerializeField]
    public Transform Root;

    [SerializeField]
    private List<Collider> MapCollider;

    [SerializeField]
    private PlatformMovement platforms;

    private CapsuleCollider mCapsule3D;
    public CapsuleCollider Capsule3D
    {
        get { return mCapsule3D; }
    }

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private AnimationCurve JumpCurve;


    public float GravityScalar = -2.45f;
    Vector3 Gravity2D;
    Vector3 Gravity3D;

    private int mLastFacing = 0;
    private Vector3 mFacingDirection;
    private CoroutineWrapper wrapper;



    // 기본 컨트롤
    protected override void Controller()
    {
        float yDelta = Input.GetAxis("Vertical");
        float xDelta = Input.GetAxis("Horizontal");

        if (Mathf.Abs(yDelta) <= Mathf.Epsilon && Mathf.Abs(xDelta) <= Mathf.Epsilon)
        {
            if (mActorIndex == 1 && mbMoving == true)
            {
                mFmodEventInstances["event:/CoolCat/Cat_Move"].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }

            mbMoving = false;

        }
        else
        {
            if (mActorIndex == 1 && mbMoving == false)
            {
                FMOD.Studio.EventInstance inst = mFmodEventInstances["event:/CoolCat/Cat_Move"];
                inst.start();
            }

            mbMoving = true;

        }
        if (CurrentLevel.LevelStatus == Level.eLevelStatus.LEVEL_3D)
        {
            var velocity = (new Vector3(xDelta * MoveSpeed * Time.deltaTime, 0.0f, yDelta * MoveSpeed * Time.deltaTime));

            if (CheckPenetration(Vector3.ClampMagnitude(velocity, 0.1f), out var decomp))
                velocity += decomp.normalized * velocity.magnitude;

            Rigidbody3D.MovePosition(Rigidbody3D.position + velocity);

            if (Mathf.Abs(xDelta) >= Mathf.Epsilon)
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

            var velocity = new Vector3(xDelta * MoveSpeed * Time.deltaTime, 0.0f, 0.0f);

            if (CheckPenetration(Vector3.ClampMagnitude(velocity, 0.1f), out var decomp))
                velocity += decomp.normalized * velocity.magnitude;

            Rigidbody3D.MovePosition(Rigidbody3D.position + velocity);

            if (Input.GetKeyDown(KeyCode.Space) && mJumpCount < MaxJumpCount)
            {
                wrapper.StartSingleton(Jump(new Vector3(0.0f, 0.0f, JumpPower)));
                //Rigidbody3D.velocity += new Vector3(0.0f, 0.0f, JumpPower * Time.deltaTime);

                mJumpCount++;
            }
        }

        Attack();

        if(Input.GetKeyDown(KeyCode.X))
        {
            if(ActorIndex == 0)
            {
                FMOD.Studio.EventInstance dogTaunt = mFmodEventInstances["event:/HotDog/Dog_Taunt"];

                dogTaunt.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform));
                dogTaunt.start();
            }
            else
            {
                FMOD.Studio.EventInstance catTaunt = mFmodEventInstances["event:/CoolCat/Cat_Taunt"];

                catTaunt.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform));
                catTaunt.start();
            }
        }

        LookAt(new Vector2(xDelta, yDelta));

        inputMovement.CurrentData = new Vector2(xDelta, yDelta).magnitude > 0.2f;

        return;
    }

    IEnumerator Jump(Vector3 jumpVelocity)
    {
        float t = 0;

        FMOD.Studio.EventInstance jumpSound = mFmodEventInstances["event:/CoolCat/Cat_Jump"];
        jumpSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform));

        jumpSound.start();

        while (t < 0.25f)
        {
            var delta = jumpVelocity * JumpCurve.Evaluate(t * 4);
            Rigidbody3D.MovePosition(Rigidbody3D.position + delta);
            jumpVelocity -= delta;

            t += Time.deltaTime;

            if (mActorIndex == 1)
            {
                mbMoving = false;
            }

            yield return null;
        }
    }


    public void InvokeAnimationTrigger(int index)
    {
        switch (index)
        {
            case 1:
                animator.SetTrigger("walk_start");
                break;
            case 2:
                animator.SetTrigger("walk_end");
                break;
            case 3:
                animator.SetTrigger("attack");
                break;
        }
    }

    public bool CheckPenetration(in Vector3 offset, out Vector3 decomposition)
    {
        decomposition = Vector3.zero;
        foreach (var collider in MapCollider)
        {
            if (collider.ComputePenetration(Capsule3D, Capsule3D.transform.position + offset, out var dir, out var dis))
            {
                decomposition += dir * dis;
            }
        }

        foreach(var collider in platforms.colliders)
        {
            if (collider.ComputePenetration(Capsule3D, Capsule3D.transform.position + offset, out var dir, out var dis))
            {
                decomposition += dir * dis;
            }
        }

        return !Mathf.Approximately(decomposition.magnitude, 0);
    }

    protected override void Awake()
    {
        base.Awake();

        mFmodEvents.Add("event:/BGM/Depeat");
        mFmodEvents.Add("event:/BGM/Main");
        mFmodEvents.Add("event:/BGM/Victory");

        mFmodEvents.Add("event:/CoolCat/Cat_Attack");
        mFmodEvents.Add("event:/CoolCat/Cat_Dash");
        mFmodEvents.Add("event:/CoolCat/Cat_Hit");
        mFmodEvents.Add("event:/CoolCat/Cat_Hurt");

        mFmodEvents.Add("event:/CoolCat/Cat_Jump");
        mFmodEvents.Add("event:/CoolCat/Cat_Move");
        mFmodEvents.Add("event:/CoolCat/Cat_Taunt");

        mFmodEvents.Add("event:/HotDog/Dog_Attack");
        mFmodEvents.Add("event:/HotDog/Dog_Dash");
        mFmodEvents.Add("event:/HotDog/Dog_Hit");
        mFmodEvents.Add("event:/HotDog/Dog_Hurt");
        mFmodEvents.Add("event:/HotDog/Dog_Idle");
        mFmodEvents.Add("event:/HotDog/Dog_Jump");
        mFmodEvents.Add("event:/HotDog/Dog_Taunt");

        mFmodEvents.Add("event:/SFX/SwitchToCat");
        mFmodEvents.Add("event:/SFX/SwitchToDog");

        wrapper = new CoroutineWrapper(this);

        // Rigidbody, Collision 초기화.
        mRigidbody3D = GetComponentInChildren<Rigidbody>();
        mCapsule3D = GetComponentInChildren<CapsuleCollider>();

        RigidGameObject = transform.Find("Rigid3D").gameObject;
        mMesh = mRigidbody3D.transform.Find("Mesh").gameObject;

        MapCollider = Root.GetComponentsInChildren<Collider>(false).ToList();

        ActorTransform = mRigidbody3D.transform;
        inputMovement.OnDataChanged += Movement_OnDataChanged;
        GroundState.OnDataChanged += GroundState_OnDataChanged;

        Gravity2D = new Vector3(0.0f, 0.0f, GravityScalar);
        Gravity3D = new Vector3(0.0f, GravityScalar, 0.0f);

        TagGame.Photon.PhotonManager.Instance.OnTagReceive += Instance_OnTagReceive;
    }

    private void Start()
    {
        foreach (var e in mFmodEvents)
        {
            FMOD.Studio.EventInstance inst = FMODUnity.RuntimeManager.CreateInstance(e);
            mFmodEventInstances.Add(e, inst);
            inst.setVolume(mMasterVolume);
        }

        if (IsLocalPlayer)
        {
            FMOD.Studio.EventInstance mainMusic = mFmodEventInstances["event:/BGM/Main"];
            mainMusic.start();
        }

        if (mActorIndex == 0)
        {
            FMOD.Studio.EventInstance dogIdleSound = mFmodEventInstances["event:/HotDog/Dog_Idle"];
            dogIdleSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform));
            dogIdleSound.start();
        }

    }


    private void Instance_OnTagReceive(TagGame.Photon.TagPacket obj)
    {
        if (obj.actorIndex == mActorIndex) // 공격자 데이터가 공격자 캐릭터로 갔다면
            return;

        Debug.Log("ReceiveTag");
        //Rigidbody3D.AddExplosionForce(10.0f, obj.contactDirection, 1.0f);
        Rigidbody3D.transform.name = "Hit by " + obj.actorIndex;
    }

    private void GroundState_OnDataChanged(bool isGrounded)
    {
        if (isGrounded)
        {
            mJumpCount = 0;
        }
        else
        {

        }
    }

    private void Movement_OnDataChanged(bool isMove)
    {
        //공격시 아래의 애니메이션 코드를 실행해 주세요
        //animator.SetTrigger("attack");

        if (isMove)
        {
            animator.SetTrigger("walk_start");
            TagGame.Photon.PhotonManager.SendAnimationData(new TagGame.Photon.AnimationPacket() { index = 1 });
        }
        else
        {
            animator.SetTrigger("walk_end");
            TagGame.Photon.PhotonManager.SendAnimationData(new TagGame.Photon.AnimationPacket() { index = 2 });
        }
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(CurrentLevel.TagPlayer != gameObject)
            {
                return;
            }

            if(ActorIndex == 0)
            {
                FMOD.Studio.EventInstance dogAttackSound = mFmodEventInstances["event:/HotDog/Dog_Attack"];
                dogAttackSound.start();
            }
            else
            {
                FMOD.Studio.EventInstance catAttackSound = mFmodEventInstances["event:/CoolCat/Cat_Attack"];
                catAttackSound.start();
            }

            animator.SetTrigger("attack");
            TagGame.Photon.PhotonManager.SendAnimationData(new TagGame.Photon.AnimationPacket() { index = 3 });

            Vector3 origin = ActorTransform.position;
            Vector3 direction = mFacingDirection * 3.0f;

            RaycastHit hitResult;
            Ray r = new Ray(origin, direction);
            Debug.DrawRay(origin, direction, Color.blue, 1.0f);

            if (Physics.Raycast(r, out hitResult, 3.0f))
            {
                if (hitResult.collider.transform.parent.gameObject == CurrentLevel.RemotePlayer)
                {
                    Debug.Log("Attack Hit.");

                    if (ActorIndex == 1)
                    {
                        FMOD.Studio.EventInstance catAttackSound = mFmodEventInstances["event:/CoolCat/Cat_Hit"];
                        FMOD.Studio.EventInstance dogHurtSound = mFmodEventInstances["event:/HotDog/Dog_Hurt"];

                        dogHurtSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(CurrentLevel.RemotePlayer.transform));
                        catAttackSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform));
                        dogHurtSound.start();
                        catAttackSound.start();

                    }
                    else
                    {
                        FMOD.Studio.EventInstance dogAttackSound = mFmodEventInstances["event:/HotDog/Dog_Hit"];
                        FMOD.Studio.EventInstance catHurtSound = mFmodEventInstances["event:/CoolCat/Cat_Hurt"];

                        catHurtSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(CurrentLevel.RemotePlayer.transform));
                        dogAttackSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform));
                        dogAttackSound.start();
                        catHurtSound.start();
                    }

                    //.call Instance_OnTagReceive
                    TagGame.Photon.PhotonManager.SendTagPacketData(new TagGame.Photon.TagPacket()
                    {
                        actorIndex = this.mActorIndex,
                        contactPosition = hitResult.point,
                        //contactDirection = hitResult.collider.transform.position - Rigidbody3D.position
                        contactDirection = direction
                    });

                    if(CurrentLevel.MatchTimer >= CurrentLevel.SuddenDeathTime)
                    {
                        if(this.gameObject.name == "Player2D")
                        {
                            CurrentLevel.WinnerText.text = "Cat Wins!";
                        }
                        else if (this.gameObject.name == "Player3D")
                        {
                            CurrentLevel.WinnerText.text = "Dog Wins!";
                        }
                    }

                    Debug.Log("SendTag");
                    //Rigidbody RemoteRigid = CurrentLevel.RemotePlayer.GetComponentInChildren<Rigidbody>();
                    //RemoteRigid.AddExplosionForce(10.0f, direction, 1.0f);
                }
            }

        }
    }

    private void LookAt(Vector2 input)
    {
        if (input.magnitude < 0.2)
            return;

        Vector3 Upward = CurrentLevel.LevelStatus == Level.eLevelStatus.LEVEL_2D ? Vector3.forward : Vector3.up;

        if (CurrentLevel.LevelStatus == Level.eLevelStatus.LEVEL_2D) // not working?
            input = new Vector2(input.x, 0);

        if (Mathf.Approximately(input.magnitude, 0))
            return;

        var targetRotation = Quaternion.Slerp(Rigidbody3D.rotation, Quaternion.LookRotation(input.ToVector3FromXZ(), Upward), 0.25f);
        Rigidbody3D.MoveRotation(targetRotation);
    }

    void Update()
    {
        if (IsLocalPlayer == true)
        {
            Debug.Log("Local");
            Controller();
        }

        if (mActorIndex == 1)
        {
            mMesh.transform.LookAt(MainCamera.transform);
        }

        if (CurrentLevel.LocalActor.gameObject == gameObject)
        {
            FMOD.Studio.EventInstance moveSound = mFmodEventInstances["event:/CoolCat/Cat_Move"];

            moveSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform));
        }
        else
        {
            FMOD.Studio.EventInstance moveSound = mFmodEventInstances["event:/CoolCat/Cat_Move"];
            moveSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(CurrentLevel.RemotePlayer.transform));
        }


        if (IsLocalPlayer == false)
        {
            return;
        }


        if (CurrentLevel.LevelStatus == Level.eLevelStatus.LEVEL_2D)
        {
            GroundState.CurrentData = CheckGround();

            if (GroundState.CurrentData == false && !wrapper.IsPlaying)
                Rigidbody3D.velocity += Gravity2D * Time.deltaTime;
        }
        else
        {
            Rigidbody3D.velocity += Gravity3D * Time.deltaTime;
        }

        Rigidbody3D.transform.position = new Vector3(Rigidbody3D.transform.position.x, 0.3f, Rigidbody3D.transform.position.z);
    }

    private bool CheckGround()
    {
        Vector3 rayStart = new Vector3(ActorTransform.position.x, ActorTransform.position.y, ActorTransform.position.z - (Capsule3D.height / 2.0f) + 0.1f);
        Debug.DrawLine(rayStart, ActorTransform.position - (Vector3.forward * 10), Color.red, 2.0f, false);

        var hits = Physics.RaycastAll(rayStart, Vector3.back, 0.2f);
        if (hits != null && hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDestroy()
    {
        if (TagGame.Photon.PhotonManager.TryGetInstance(out var instance))
        {
            instance.OnTagReceive -= Instance_OnTagReceive;
        }

    }
}
