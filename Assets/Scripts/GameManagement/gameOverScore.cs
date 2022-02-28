using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameOverScore : MonoBehaviour
{
    BattleManager battleManager;
    public Text gameOverText;
    public GameObject highScoreText;
    public GameObject scoreText;

    // Start is called before the first frame update
    void Start()
    {
        battleManager = GameObject.Find("EnemySpawner").GetComponent<BattleManager>();
        gameOverText.text = battleManager.score.ToString();
        if (battleManager.score >= battleManager.highScore)
        {
            scoreText.SetActive(false);
            highScoreText.SetActive(true);
        }
        GameObject.Find("EnemySpawner").SetActive(false);
    }

}
