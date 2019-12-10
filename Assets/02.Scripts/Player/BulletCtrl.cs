using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float damage = 20.0f;

    public float speed = 1000.0f;

    private Transform tr;
    private Rigidbody rb;
    private TrailRenderer trail;

    void Awake()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
    }

    void OnEnable()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }
    void OnDisable()
    {
        trail.Clear();
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }

}
