using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLSpace;

public class HammerHit : MonoBehaviour
{
    public Rigidbody rb;   // reference to rigidbody
    public float hitInterval = 1.0f;        // do mot hit every frame. give it a little buffer.
    private float hitTimer = 0.0f;
    public bool isHit = false;
    public int[] indices;
    public float damage;

    private void Start()
    {
        // get rigidbody component
        if (!rb) { Debug.LogWarning("Cannot find Rigidbody component."); }
        hitTimer = hitInterval;
    }

    private void Update()
    {
        hitTimer += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == 11 || collision.gameObject.layer == 12)
        {
            titanEnemyController enemyTitanController = collision.gameObject.transform.parent.GetComponentInParent<titanEnemyController>();
            giantEnemyController enemyGiantController = collision.gameObject.transform.parent.GetComponentInParent<giantEnemyController>();
            if (hitTimer > hitInterval)
            {
                BodyColliderScript bcs = collision.collider.GetComponent<BodyColliderScript>();
                if (bcs && rb)
                {
                    indices = new int[] { bcs.index };
                    if (enemyTitanController)
                    {
                        enemyTitanController.isHit = true;
                    }

                    if (enemyGiantController)
                    {
                        enemyGiantController.isHit = true;
                    }
                }
                hitTimer = 0.0f;
            }
        }
        
    }
}
