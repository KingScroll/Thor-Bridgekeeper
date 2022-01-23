using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLSpace;

public class rock : MonoBehaviour
{
    private rockGenerator rockGen;
    private titanEnemyController enemyTitanController;
    private giantEnemyController enemyGiantController;
    float rockHP;
    public Rigidbody rockRB;
    public GameObject struckEmitter;
    public bool shouldDie = false;
    public bool shouldRockHarmEnemies = false;
    public bool checkBridge = false;
    private RagdollManagerHum ragdollManager;
    public float hitInterval = 1.0f;        // do mot hit every frame. give it a little buffer.
    private float hitTimer = 0.0f;
    public int[] indices;

    // Start is called before the first frame update
    void Start()
    {
        rockHP = 20f;
        rockRB = GetComponent<Rigidbody>();
        hitTimer = hitInterval;
    }

    // Update is called once per frame
    void Update()
    {
        hitTimer += Time.deltaTime;

        if (rockHP <= 0)
        {
            shouldDie = true;
        }

        if (checkBridge == true)
        {
            // if off bridge
            if (gameObject.transform.position.x >= 12 || gameObject.transform.position.x <= -12
                || gameObject.transform.position.y <= -5)
            {
                shouldDie = true;
            }
        }

        if (shouldDie)
            Destroy(gameObject, 2f);

    }

    private void OnParticleCollision(GameObject other)
    {

        rockRB.constraints = RigidbodyConstraints.None;

        if (other.name == "StrikeLightningPoint")
        {
            rockHP -= 2f;
            struckEmitter.SetActive(true);
            rockRB.AddExplosionForce(4f, gameObject.transform.position, 10f, 2f, ForceMode.Impulse);
        }

        if (other.name == "Lightning")
        {
            rockHP -= 0.5f;
            struckEmitter.SetActive(true);
            rockRB.AddExplosionForce(4f, gameObject.transform.position, 10f, 2f, ForceMode.Impulse);
        }

        if (other.name == "UltimateStrike" || other.name == "UltimateLightning")
        {
            rockHP -= 5f;
            struckEmitter.SetActive(true);
            rockRB.AddExplosionForce(4f, gameObject.transform.position, 10f, 2f, ForceMode.Impulse);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Shockwave")
        {
            rockHP -= 10f;
            rockRB.constraints = RigidbodyConstraints.None;
            rockRB.AddForce(new Vector3(0, 4f, 15f), ForceMode.Impulse);
        }

        if (collision.gameObject.name == "Mjolnir")
        {
            GetComponent<AudioSource>().Play();
            Debug.Log("collision with hammer!");
            rockRB.AddForce(new Vector3(0, 25f, 65f), ForceMode.Impulse);
            rockRB.constraints = RigidbodyConstraints.None;
            rockHP -= 10f;
            Debug.Log("rockHP: " + rockHP);
        }

        if (collision.gameObject.name == "vfxGraphBolt")
        {
            GetComponent<AudioSource>().Play();
            Debug.Log("collision with hammer!");
            rockRB.AddForce(new Vector3(0, 25f, 65f), ForceMode.Impulse);
            rockRB.constraints = RigidbodyConstraints.None;
            rockHP -= 20f;
            Debug.Log("rockHP: " + rockHP);
        }

        if (shouldRockHarmEnemies)
        {

            if (collision.gameObject.layer == 11 || collision.gameObject.layer == 12)
            {
                ragdollManager = collision.gameObject.transform.parent.GetComponentInParent<RagdollManagerHum>();
                enemyTitanController = collision.gameObject.transform.parent.GetComponentInParent<titanEnemyController>();
                enemyGiantController = collision.gameObject.transform.parent.GetComponentInParent<giantEnemyController>();
                rockGen = collision.gameObject.transform.parent.GetComponentInChildren<rockGenerator>();

                if (hitTimer > hitInterval)
                {
                    BodyColliderScript bcs = collision.collider.GetComponent<BodyColliderScript>();
                    if (bcs && rockRB)
                    {
                        indices = new int[] { bcs.index };
                        if (enemyTitanController)
                        {
                            rockGen.shouldLetGo = true;
                            rockGen.disableCol = true;
                            enemyTitanController.bodyParts = indices;
                            enemyTitanController.rockMass = rockRB.mass;
                            enemyTitanController.rockVelocity = rockRB.velocity;
                            enemyTitanController.hitbyRock = true;
                        }

                        if (enemyGiantController)
                        {
                            enemyGiantController.bodyParts = indices;
                            enemyGiantController.rockMass = rockRB.mass;
                            enemyGiantController.rockVelocity = rockRB.velocity;
                            enemyGiantController.hitbyRock = true;
                        }

                    }
                    hitTimer = 0.0f;
                }

            }
        }

    }
}
