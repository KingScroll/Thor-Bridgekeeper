using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundStop : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
                Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                transform.position = transform.position;
                transform.rotation = transform.rotation;
        }
    }
}
