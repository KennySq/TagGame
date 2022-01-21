using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    protected abstract void Controller();
    public float MoveSpeed;

    protected Rigidbody mRigidbody;
    protected Rigidbody Rigidbody
    {
        get { return mRigidbody; }
    }

    void Update()
    {
    }
}
