using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3D : Actor
{
    protected override void Controller()
    {
        float yDelta = Input.GetAxis("Vertical");
        float xDelta = Input.GetAxis("Horizontal");

        Rigidbody.AddRelativeForce(new Vector3(xDelta * MoveSpeed, 0.0f, yDelta * MoveSpeed));

        return;
    }

    // Start is called before the first frame update

    private void Awake()
    {
        mRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Controller();

    }
}
