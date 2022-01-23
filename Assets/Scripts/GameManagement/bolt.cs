using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLSpace;

public class bolt : MonoBehaviour
{
    public Rigidbody boltRB;
    public bool inHand = true;
    private RagdollManagerHum ragdollManager;
    public float hitInterval = 1.0f;        // do mot hit every frame. give it a little buffer.
    private float hitTimer = 0.0f;
    public int[] indices;

    private titanEnemyController enemyTitanController;
    private giantEnemyController enemyGiantController;


    private void Start()
    {
        boltRB = GetComponent<Rigidbody>();
        hitTimer = hitInterval;
    }

    private void Update()
    {
        hitTimer += Time.deltaTime;


        if (gameObject.transform.position.x > 90 || gameObject.transform.position.x < -90)
        {
            StartCoroutine(BoltDestroy());
        }

        if (gameObject.transform.position.y > 90)
        {
            StartCoroutine(BoltDestroy());
        }

        if (gameObject.transform.position.z > 350)
        {
            StartCoroutine(BoltDestroy());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == 11 || collision.gameObject.layer == 12)
        {
            ragdollManager = collision.gameObject.transform.parent.GetComponentInParent<RagdollManagerHum>();
            enemyTitanController = collision.gameObject.transform.parent.GetComponentInParent<titanEnemyController>();
            enemyGiantController = collision.gameObject.transform.parent.GetComponentInParent<giantEnemyController>();

            if (hitTimer > hitInterval)
            {
                BodyColliderScript bcs = collision.collider.GetComponent<BodyColliderScript>();
                if (bcs && boltRB)
                {
                    indices = new int[] { bcs.index };
                    if (enemyTitanController)
                    {
                        enemyTitanController.bodyParts = indices;
                        enemyTitanController.rockMass = boltRB.mass;
                        enemyTitanController.rockVelocity = boltRB.velocity;
                        enemyTitanController.hitbyBolt = true;
                    }

                    if (enemyGiantController)
                    {
                        enemyGiantController.bodyParts = indices;
                        enemyGiantController.rockMass = boltRB.mass;
                        enemyGiantController.rockVelocity = boltRB.velocity;
                        enemyGiantController.hitbyBolt = true;
                    }

                }
                hitTimer = 0.0f;
            }

        }

        if (inHand == false)
        {
            if (collision.gameObject.tag == "enemy" || collision.gameObject.tag == "ground" || collision.gameObject.name == "rock")
            {
                StartCoroutine(BoltDestroy());
            }
        }

    }

    IEnumerator BoltDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
        yield return null;
    }

}
