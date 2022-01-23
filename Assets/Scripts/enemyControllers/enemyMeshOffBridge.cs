using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLSpace;
using UnityEngine.AI;

public class enemyMeshOffBridge : MonoBehaviour
{

    private GameObject enemyMesh;
    private Animator enemyAnimator;
    private RagdollManagerHum ragdollManager;
    private NavMeshAgent agent;
    public bool shouldKill;
    private GameObject enemySpawner;
    BattleManager battleManager;

    private void Start()
    {
        enemyMesh = gameObject;
        enemyAnimator = GetComponentInParent<Animator>();
        ragdollManager = GetComponentInParent<RagdollManagerHum>();
        enemySpawner = GameObject.Find("EnemySpawner");
        battleManager = enemySpawner.GetComponent<BattleManager>();

        if (GetComponentInParent<NavMeshAgent>()) { agent = GetComponentInParent<NavMeshAgent>(); } else
        {
            agent = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if off bridge
        if (enemyMesh.transform.position.x >= 6 || enemyMesh.transform.position.x <= -6
            || enemyMesh.transform.position.y <= -0.7)
        {
            battleManager.score += 30;
            Debug.Log("Off bridge!");
            enemyAnimator.enabled = false;
            ragdollManager.enableGetUpAnimation = false;
            ragdollManager.startRagdoll();
            agent.enabled = false;
            shouldKill = true;
        }
    }
}
