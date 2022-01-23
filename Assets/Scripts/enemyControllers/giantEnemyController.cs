using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MLSpace;

public class giantEnemyController : MonoBehaviour
{
    private HammerHit hammerHit;
    public bool isHit = false;
    private enemyMeshOffBridge offBridge;
    private GameObject hammer;
    private BoxCollider boxCollider;
    private Rigidbody rb;
    private Animator enemyAnimator;
    private RagdollManagerHum ragdollManager;
    private GameObject target;
    private NavMeshAgent agent;
    public GameObject StruckEmitter;
    public float jumpAttackDistance;
    public float enemyHP;
    int jumpAttackCounter = 0;
    public bool hitbyRock = false;
    public bool hitbyBolt = false;
    public int[] bodyParts;
    public float rockMass;
    public float boltMass;
    public Vector3 rockVelocity;
    public Vector3 boltVelocity;
    bool callDie = true;
    int hammerHitCount = 0;
    public GameObject deathParticlePrefab;
    public GameObject bodyMesh;
    private GameObject enemySpawner;
    private BattleManager battleManager;

    public AudioClip giantAttackAudio;
    public AudioClip giantJumpAttackAudio;
    public AudioClip hitAudio;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Target");
        enemySpawner = GameObject.Find("EnemySpawner");
        battleManager = enemySpawner.GetComponent<BattleManager>();
        enemyHP = 30;
        hammer = GameObject.Find("Mjolnir");
        offBridge = GetComponentInChildren<enemyMeshOffBridge>();
        hammerHit = hammer.GetComponent<HammerHit>();
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        enemyAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ragdollManager = GetComponent<RagdollManagerHum>();
        agent.SetDestination(target.transform.position);
        FaceTarget();
        enemyAnimator.SetBool("isWalking", true);
        enemyAnimator.SetBool("shouldAttack", false);
        enemyAnimator.SetBool("shouldJumpAttack", false);
        StruckEmitter.GetComponent<ParticleSystem>().Stop();
    }

    // Update is called once per frame
    void Update()
    {

        float distance = Vector3.Distance(target.transform.position, transform.position);

        // if distance is at or past Jumping point but further than stopping point
        if (distance <= jumpAttackDistance && distance > agent.stoppingDistance)
        {
            agent.enabled = true;
            enemyAnimator.SetBool("isWalking", false); // stop walking
            FaceTarget();
            enemyAnimator.SetBool("shouldJumpAttack", true); // jump attack!
            StartCoroutine(WaitForJumpAttack(enemyAnimator.GetCurrentAnimatorStateInfo(0).length));
        }

       // if distance is further than stopping point and jump already happened, walk towards target
       if ((distance > agent.stoppingDistance || distance > jumpAttackDistance) && jumpAttackCounter > 0)
        {
            FaceTarget();
            agent.enabled = true;
            enemyAnimator.SetBool("shouldJumpAttack", false);
            enemyAnimator.SetBool("shouldAttack", false);
            enemyAnimator.SetBool("isWalking", true);
        }

       if (distance > agent.stoppingDistance && enemyAnimator.GetBool("isWalking") == true && transform.rotation.y > -150 || transform.rotation.y < -180)
        {
            FaceTarget();
        }

       // if distance is at or past the stoppingPoint, then do melee attack
       if (distance <= agent.stoppingDistance)
        {
            agent.enabled = false;
            FaceTarget();
            enemyAnimator.SetBool("isWalking", false);
            enemyAnimator.SetBool("shouldJumpAttack", false);
            enemyAnimator.SetBool("shouldAttack", true);
        }

        if (enemyHP <= 0)
        {

            if (callDie)
            {
                if (isHit == false)
                {
                    ragdollManager.startRagdoll();
                }
                agent.enabled = false;
                StartCoroutine(Die());
            }

        }


        if (isHit == true)
        {
            GetComponent<AudioSource>().PlayOneShot(hitAudio);
            hammerHitCount++;
            boxCollider.enabled = false;
            rb.isKinematic = true;
            enemyHP -= hammerHit.damage;
            Debug.Log("Hit! HP: " + enemyHP);
            agent.enabled = false;
            ragdollManager.startRagdoll(hammerHit.indices, (hammerHit.rb.velocity * hammerHit.rb.mass) / 1200);
            if (enemyHP > 0)
            {
                StartCoroutine(GetUp());
            }
            isHit = false;
        }

        if (hitbyRock)
        {
            Debug.Log("hitbyRock");
            GetComponent<AudioSource>().Play();
            enemyHP -= 70;
            agent.enabled = false;
            ragdollManager.startRagdoll(bodyParts, ((rockMass * rockVelocity) / 600));
            hitbyRock = false;
        }

        if (hitbyBolt)
        {
            Debug.Log("hitbyBolt");
            GetComponent<AudioSource>().Play();
            enemyHP -= 30;
            agent.enabled = false;
            ragdollManager.startRagdoll(bodyParts, (boltMass * boltVelocity));
            hitbyBolt = false;
        }

        if (offBridge.shouldKill == true)
        {
            Destroy(gameObject);
        }

    }

    IEnumerator WaitForJumpAttack(float _delay = 0)
    {
        yield return new WaitForSeconds(_delay);
        enemyAnimator.SetBool("shouldJumpAttack", false);
        jumpAttackCounter++;
    }

    IEnumerator GetUp()
    {
        boxCollider.enabled = true;
        rb.isKinematic = false;
        yield return new WaitForSeconds(4f);
       ragdollManager.blendToMecanim();
        agent.enabled = true;
                FaceTarget();
       yield return null;
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.name == "StrikeLightningPoint" || other.name == "UltimateStrike" || other.name == "UltimateLightning")
        {
            enemyHP -= 2f;
            StruckEmitter.GetComponent<ParticleSystem>().Play();
        }

        if (other.name == "Lightning")
        {
            enemyHP -= 0.5f;
            StruckEmitter.GetComponent<ParticleSystem>().Play();
        }

        if (other.name == "EmitterLightning")
        {
            enemyHP -= 30f;
            StruckEmitter.GetComponent<ParticleSystem>().Play();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.name == "Shockwave")
        {
            enemyHP -= 10f;
        }

    }

    void JumpAttackAudio()
    {
        GetComponent<AudioSource>().PlayOneShot(giantJumpAttackAudio);
    }

    void MeleeAttackAudio()
    {
        GetComponent<AudioSource>().PlayOneShot(giantAttackAudio);
    }

    IEnumerator Die()
    {
        callDie = false;
        Debug.Log("giant Die called!");
        battleManager.score += 30;
        GetComponent<NavMeshAgent>().enabled = false;
        yield return new WaitForSeconds(3.2f);
        var deathParticle = Instantiate(deathParticlePrefab, bodyMesh.transform.position, bodyMesh.transform.rotation) as GameObject;
        deathParticle.transform.localScale += new Vector3(3, 3, 3);
        Destroy(gameObject);
        yield return null;
    }
}
