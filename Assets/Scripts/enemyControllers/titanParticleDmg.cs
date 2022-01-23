using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class titanParticleDmg : MonoBehaviour
{
    private titanEnemyController enemyTitanController;
    public GameObject StruckEmitter;

    private void Start()
    {
        enemyTitanController = GetComponentInParent<titanEnemyController>();
        StruckEmitter.GetComponent<ParticleSystem>().Stop();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.name == "StrikeLightningPoint" || other.name == "UltimateStrike" || other.name == "UltimateLightning")
        {
            enemyTitanController.enemyHP -= 2f;
            StruckEmitter.GetComponent<ParticleSystem>().Play();
        }

        if (other.name == "Lightning")
        {
            enemyTitanController.enemyHP -= 0.5f;
            StruckEmitter.GetComponent<ParticleSystem>().Play();
        }

    }

    private void Update()
    {
        if (enemyTitanController.enemyHP <= 0)
        {
            StruckEmitter.GetComponent<ParticleSystem>().Stop();
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
