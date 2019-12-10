using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "BULLET")
        {
            ShowEffect(collision);
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);

        }
    }

    void ShowEffect(Collision coll)
    {
        ContactPoint contact = coll.contacts[0];

        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

        GameObject spark = Instantiate(sparkEffect, contact.point, rot);
        spark.transform.SetParent(transform);
    }
}
