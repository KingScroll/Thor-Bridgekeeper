using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
    private BoxCollider mainCollider;
    public GameObject StruckEmitter;
    public GameObject hammer;
    private GameObject enemyRig;
    private Animator enemyAnimator;
    private GameObject target;
    private NavMeshAgent agent;
    bool isRunning;
    bool shouldAttack = false;
    float ragdollForce;
    float enemyHP;
    bool callDie = true;
    private GameObject enemySpawner;
    private BattleManager battleManager;
    public GameObject deathParticlePrefab;
    public GameObject bodyMesh;
    public float hammerVelocity;
    AudioSource audioSource;
    public AudioClip hitAudioClip;
    public AudioClip attack1Audio;
    public AudioClip attack2Audio;
    public AudioClip attack3Audio;
    public AudioClip attack4Audio;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        target = GameObject.Find("Target");
        enemySpawner = GameObject.Find("EnemySpawner");
        hammer = GameObject.Find("Mjolnir");
        battleManager = enemySpawner.GetComponent<BattleManager>();
        enemyHP = 10;
        mainCollider = GetComponent<BoxCollider>();
        enemyRig = gameObject;
        enemyAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(target.transform.position); // chase target
        agent.speed = Random.Range(3f, 8f);
        agent.acceleration = Random.Range(6f, 10f);
        enemyAnimator.SetBool("isRunning", true);
        getRagdollRigidbodies();
        RagDollModeOff();
        FaceTarget();
        StruckEmitter.GetComponent<ParticleSystem>().Stop();
    }

    void Update()
    {
        hammerVelocity = hammer.GetComponent<Mjolnir_V2>().velocityNum;

        float distance = Vector3.Distance(target.transform.position, transform.position);
        ragdollForce = hammerVelocity * 10f;

        if (distance <= agent.stoppingDistance)
        {
            GetComponent<NavMeshAgent>().enabled = false;
            enemyAnimator.SetBool("isRunning", false);
            enemyAnimator.SetBool("shouldAttack", true);
            int AttackType = Random.Range(1, 5);
            enemyAnimator.SetInteger("AttackType", AttackType);
        }

        if (enemyHP <= 0 && callDie)
        {
            StartCoroutine(Die());
        }
    }

    private void OnParticleCollision(GameObject other)
    {

        if (other.name == "StrikeLightningPoint" || other.name == "UltimateStrike" || other.name == "UltimateLightning")
        {
            StruckEmitter.GetComponent<ParticleSystem>().Play();
            enemyHP -= 10;
            if (enemyHP <= 0)
            {
                RagDollModeOn(ragdollForce * 2f);
            }
        }

        if (other.name == "Lightning")
        {
            StruckEmitter.GetComponent<ParticleSystem>().Play();
            enemyHP -= 5f;
            if (enemyHP <= 0)
            {
                RagDollModeOn(ragdollForce * 1.5f);
            }
        }

        if (other.name == "EmitterLightning")
        {
            StruckEmitter.GetComponent<ParticleSystem>().Play();
            enemyHP -= 10f;
            if (enemyHP <= 0)
            {
                RagDollModeOn(ragdollForce * 3f);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "damage" && collision.gameObject.name != "Shockwave")
        {
            enemyHP -= 10;
            if (enemyHP <= 0)
            {
                RagDollModeOn(ragdollForce);
            }
            StartCoroutine(Die());
        }

        if (collision.gameObject.name == "Shockwave")
        {
            enemyHP -= 10;
            if (enemyHP <= 0)
            {
                RagDollModeOn(ragdollForce * 3f);
            }
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
    }

    Collider[] ragdollColliders;
    Rigidbody[] ragdollRigidbodies;

    void getRagdollRigidbodies()
    {
        ragdollColliders = enemyRig.GetComponentsInChildren<Collider>();
        ragdollRigidbodies = enemyRig.GetComponentsInChildren<Rigidbody>();
    }

    void RagDollModeOn(float force)
    {
        enemyAnimator.enabled = false;
        foreach (Collider col in ragdollColliders)
        {
            col.enabled = true;
            col.tag = "damage";
        }

        foreach (Rigidbody rigid in ragdollRigidbodies)
        {
            rigid.isKinematic = false;
           rigid.AddExplosionForce(force * 1.67f, rigid.transform.position, 7f, 0.146f, ForceMode.Impulse);
        }

        mainCollider.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

    }

    void RagDollModeOff()
    {
        enemyAnimator.enabled = true;
        foreach (Collider col in ragdollColliders)
        {
            if (col.gameObject.name != "club")
                col.enabled = false;
        }

        foreach (Rigidbody rigid in ragdollRigidbodies)
        {
            rigid.isKinematic = true;
        }

        mainCollider.enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }

   void Attack1Audio()
    {
        audioSource.PlayOneShot(attack1Audio);
    }

    void Attack2Audio()
    {
        audioSource.PlayOneShot(attack2Audio);
    }

    void Attack3Audio()
    {
        audioSource.PlayOneShot(attack3Audio);
    }


    void Attack4Audio()
    {
        audioSource.PlayOneShot(attack4Audio);
    }

    IEnumerator Die()
    {
        audioSource.PlayOneShot(hitAudioClip);
        battleManager.score += 10;
        callDie = false;
        GetComponent<NavMeshAgent>().enabled = false;
        yield return new WaitForSeconds(0.65f);
        gameObject.layer = 10;
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            child.gameObject.tag = "ignoreEnemyCollide";
            //if (child.gameObject.GetComponent<Collider>() != null){ child.gameObject.GetComponent<Collider>().enabled = false; }
            child.gameObject.layer = 10;
        }
        yield return new WaitForSeconds(3.2f);
        var deathParticle = Instantiate(deathParticlePrefab, bodyMesh.transform.position, bodyMesh.transform.rotation) as GameObject;
        Destroy(gameObject);
        yield return null;
    }
}
