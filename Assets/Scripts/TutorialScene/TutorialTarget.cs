using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTarget : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip targetBreak;

    private void OnTriggerEnter(Collider other)
    {


            if (other.name == "Mjolnir")
            {
                audioSource.PlayOneShot(targetBreak);
                Destroy(gameObject);
            }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "damage")
        {
            audioSource.PlayOneShot(targetBreak);
            Destroy(gameObject);
        }
    }
}
