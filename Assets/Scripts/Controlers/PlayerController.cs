using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float speed = 75f;
    public Rigidbody rigid;
    public GameObject cam;


    // Update is called once per frame
    public void Move(float inputH, float inputV)
    {


        Vector3 force = new Vector3(inputH, 0, inputV);
        force = transform.TransformDirection(force);
        rigid.AddForce(force * speed);
        Vector3 euler = cam.transform.eulerAngles;
        transform.rotation = Quaternion.AngleAxis(euler.y, Vector3.up);

    }
}

