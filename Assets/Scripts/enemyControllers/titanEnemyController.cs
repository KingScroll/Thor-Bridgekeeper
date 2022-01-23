using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLSpace;

public class titanEnemyController : MonoBehaviour
{
    private HammerHit hammerHit;
    private GameObject hammer;
    private titanOffBridge offBridgeTitan;
    public RagdollManagerHum ragdollManager;
    private GameObject target;
    private Animator enemyAnimator;
    public float enemyHP;
    public bool hitbyRock = false;
    public bool hitbyBolt = false;
    public int[] bodyParts;
    public float rockMass;
    public float boltMass;
    public Vector3 boltVelocity;
    public Vector3 rockVelocity;
    private GameObject enemySpawner;
    private BattleManager battleManager;
    public bool callDie = true;
    public bool isHit = false;
    public GameObject deathParticlePrefab;
    public GameObject bodyMesh;
    private GameObject titanParentObject;
    public Vector3 whichSpawnPoint;
    int titanSpawnCount;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Target");
        enemySpawner = GameObject.Find("EnemySpawner");
        battleManager = enemySpawner.GetComponent<BattleManager>();
        titanParentObject = transform.parent.gameObject;
        Debug.Log(titanParentObject);
        enemyHP = 60;
        hammer = GameObject.Find("Mjolnir");
        enemyAnimator = GetComponent<Animator>();
        enemyAnimator.SetBool("shouldThrow", true);
        hammerHit = hammer.GetComponent<HammerHit>();
        offBridgeTitan = GetComponentInChildren<titanOffBridge>();
        ragdollManager = GetComponent<RagdollManagerHum>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHP <= 0)
        {
            if (ragdollManager.isFullRagdoll == false)
            {
                ragdollManager.startRagdoll(bodyParts, new Vector3(0f, 1f, 1f));
            }

            if (callDie == true)
            {
                enemyAnimator.SetBool("shouldThrow", false);
                StartCoroutine(Die());
            }
        }

        if (isHit == true)
        {
            enemyHP -= enemyHP;
            Debug.Log("Hit! HP: " + enemyHP);
            ragdollManager.startRagdoll(hammerHit.indices, (hammerHit.rb.velocity * hammerHit.rb.mass) / 10000);
            isHit = false;
        }

        if (offBridgeTitan.shouldKill == true)
        {
            offBridgeTitan.shouldCheckBridge = false;
            battleManager.score += 100;
            Destroy(transform.parent.gameObject, 2f);
        }

        if (hitbyRock)
        {
            Debug.Log("hitbyRock");
            enemyHP -= 70;
            ragdollManager.startRagdoll(bodyParts, (rockMass * rockVelocity) / 500);
            hitbyRock = false;
        }

        if (hitbyBolt)
        {
            Debug.Log("hitbyBolt");
            enemyHP -= 30;
            ragdollManager.startRagdoll(bodyParts, (boltMass * boltVelocity) / 1000);
            hitbyRock = false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.name == "Shockwave")
        {
            enemyHP -= 10f;
        }
    }

    IEnumerator Die()
    {
        callDie = false;
        offBridgeTitan.shouldCheckBridge = false;
        offBridgeTitan.shouldKill = false;
        battleManager.score += 100;
        battleManager.titansOnField--;
        if (whichSpawnPoint == battleManager.titanSpawnPoint1) { battleManager.titanSpawnPoint1Counter--; }
        if (whichSpawnPoint == battleManager.titanSpawnPoint2) { battleManager.titanSpawnPoint2Counter--; }
        if (whichSpawnPoint == battleManager.titanSpawnPoint3) { battleManager.titanSpawnPoint3Counter--; }
        if (whichSpawnPoint == battleManager.titanSpawnPoint4) { battleManager.titanSpawnPoint4Counter--; }
        yield return new WaitForSeconds(3.2f);
        var deathParticle = Instantiate(deathParticlePrefab, bodyMesh.transform.position, bodyMesh.transform.rotation) as GameObject;
        deathParticle.transform.localScale += new Vector3(9, 9, 9);
        Destroy(transform.parent.gameObject);
        yield return null;
    }

}
