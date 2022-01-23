using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public TextMesh scoreUI;
    public GameObject spawnParticlesPrefab;
    private float timeTracker;
    public GameObject baseEnemy;
    public GameObject giantEnemy;
    public GameObject titanEnemy;
    public float baseEnemyConstant;
    public float titanConstant;
    public float giantConstant;

    List<GameObject> spawnEnemies = new List<GameObject>();

    public Quaternion spawnRotation;
    public float difficultyConstant;
    public int initEnemies;
    public float spawnWait;

    Vector3 spawnPoint;
    public float difficultyModifier = 0;

    bool initDone = false;
    int spawnCall;
    public int enemySpawnCount;
    public int activeEnemies;

    public int score = 0;
    public int highScore;

    [SerializeField] public int lightningAttackCount = 0;
    [SerializeField] public int lightningStrikeCount = 0;
    [SerializeField] public int powerHitCount = 0;
    [SerializeField] public int lightningBoltCount = 0;
    [SerializeField] public int ultimateCount = 0;

    public int titansOnField = 0;

    public Vector3 titanSpawnPoint3 = new Vector3(3.8f, 0, 103);
    public Vector3 titanSpawnPoint1 = new Vector3(-2.8f, 0, 109);
    public Vector3 titanSpawnPoint2 = new Vector3(4, 0, 118);
    public Vector3 titanSpawnPoint4 = new Vector3(-2.8f, 0, 125);



    public int titanSpawnPoint1Counter = 0;
    public int titanSpawnPoint2Counter = 0;
    public int titanSpawnPoint3Counter = 0;
    public int titanSpawnPoint4Counter = 0;

    int titanCheckCounter = 1;

    private void Start()
    {
        timeTracker = Time.timeSinceLevelLoad;
        StartCoroutine(InitialSpawn(initEnemies)); // start by spawning enemies
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        difficultyModifier = 0;
    }

    private void Update()
    {
        timeTracker = Time.timeSinceLevelLoad;
        difficultyModifier = (timeTracker + 100) / difficultyConstant; // set the difficulty modifier at new frame
        scoreUI.text = score.ToString();

        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        
        if (initDone)
        {
            if (transform.childCount > 0) { activeEnemies = transform.childCount; } else { activeEnemies = 0; }
            if (activeEnemies <= enemySpawnCount * 0.25 && spawnCall < 1)
            {
                StartCoroutine(Spawn());
                spawnCall++;
            }
        }

    }

    //Make enemies spawn between a range of positions so they don't knock each other over
    IEnumerator InitialSpawn(int initEnemyCount)
    {
        enemySpawnCount = initEnemyCount;
        for (int counter = 0; counter < initEnemyCount; counter++)
        {
            spawnPoint = new Vector3(Random.Range(-4.4f, 4.4f), 0, Random.Range(70f, 115f)); // defines spawnPoint at Random
            var newEnemy = Instantiate(baseEnemy, spawnPoint, spawnRotation) as GameObject; // Instantiates new Enemy
            var spawnParticle = Instantiate(spawnParticlesPrefab, spawnPoint, spawnRotation) as GameObject;
            newEnemy.transform.parent = gameObject.transform; // Sets new enemy as going under 'Enemies' gameObject
            yield return new WaitForSeconds(spawnWait);
            activeEnemies++;
        }
        Debug.Log("Initial Enemies spawned");
        initDone = true;
        yield return null;

    }

    IEnumerator Spawn()
    {
        Debug.Log("Spawn activated!");
        int baseSpawn;
        int giantSpawn;
        int titanSpawn;
        
        titanSpawn = Mathf.RoundToInt((difficultyModifier - 700) / 100f);
        if (titansOnField >= 2)
        {
            titanSpawn = 0;
        }
        else
        {
            if (titanSpawn < 0) { titanSpawn = 0; }
            if (titanSpawn >= 2) { titanSpawn = 2; }
        }


        giantSpawn = Mathf.RoundToInt(((difficultyModifier - 450) / 45f) - (titanSpawn * titanConstant));
        if (giantSpawn < 0) { giantSpawn = 0; }

        // number of enemies
        baseSpawn = Mathf.RoundToInt(((difficultyModifier * difficultyConstant) / baseEnemyConstant) - (giantSpawn * giantConstant));
        if (baseSpawn <= 0) { baseSpawn = Mathf.RoundToInt(difficultyModifier / 100); }

        int totalEnemyCount = baseSpawn + giantSpawn + titanSpawn;
        enemySpawnCount = totalEnemyCount;
        Debug.Log("Difficulty Modifier: " + difficultyModifier);
        Debug.Log("titans: " + titanSpawn);
        Debug.Log("giants: " + giantSpawn);
        Debug.Log("base enemies: " + baseSpawn);


        for (int i = 0; i < baseSpawn; i++)
        {
            spawnEnemies.Add(baseEnemy);
        }

        for (int i = 0; i < giantSpawn; i++)
        {
            spawnEnemies.Add(giantEnemy);
        }

        for (int i = 0; i < titanSpawn; i++)
        {
            spawnEnemies.Add(titanEnemy);
        }

        for (int counter = 0; counter < spawnEnemies.Count; counter++)
        {
            spawnWait = Random.Range(1.1f, 1.94f);

            if (spawnEnemies[counter] == titanEnemy)
            {
                 int titanSpawnChosen = 1;
                 Vector3 titanSpawnPoint = Vector3.zero;
                int i = 1;

                while (i != 0)
                {
                    if (titanSpawnPoint1Counter > 0 && titanSpawnPoint2Counter > 0 && titanSpawnPoint3Counter > 0 && titanSpawnPoint4Counter > 0)
                    {
                        titanSpawnPoint = new Vector3(Random.Range(-4.4f, 4.4f), 0, Random.Range(129, 139));
                        i = 0;
                    }

                    if (titanSpawnChosen == 1 && titanSpawnPoint1Counter < 1) { titanSpawnPoint = titanSpawnPoint1; titanSpawnPoint1Counter++; i = 0; } else { titanSpawnChosen++; }
                    if (titanSpawnChosen == 2 && titanSpawnPoint2Counter < 1) { titanSpawnPoint = titanSpawnPoint2; titanSpawnPoint2Counter++; i = 0; } else { titanSpawnChosen++; }
                    if (titanSpawnChosen == 3 && titanSpawnPoint3Counter < 1) { titanSpawnPoint = titanSpawnPoint3; titanSpawnPoint3Counter++; i = 0; } else { titanSpawnChosen++; }
                    if (titanSpawnChosen == 4 && titanSpawnPoint4Counter < 1) { titanSpawnPoint = titanSpawnPoint4; titanSpawnPoint4Counter++; i = 0; } else { titanSpawnChosen++; }
                }

                Debug.Log("Titan Spawn Point: " + titanSpawnPoint);
                spawnPoint = titanSpawnPoint;
                titansOnField++;

            }
            else
            {
                spawnPoint = new Vector3(Random.Range(-4.4f, 4.4f), 0, Random.Range(70f, 130f)); // defines spawnPoint at Random
            }

            var newEnemy = Instantiate(spawnEnemies[counter], spawnPoint, spawnRotation) as GameObject; // Instantiates new Enemy
            

            if (spawnEnemies[counter] == titanEnemy) { var spawnParticle = Instantiate(spawnParticlesPrefab, spawnPoint, spawnRotation) as GameObject;
                spawnEnemies[counter].GetComponentInChildren<titanEnemyController>().whichSpawnPoint = spawnPoint;
                spawnParticle.transform.localScale += new Vector3(9, 9, 9);
            }
            else if (spawnEnemies[counter] == giantEnemy)
            {
                var spawnParticle = Instantiate(spawnParticlesPrefab, spawnPoint, spawnRotation) as GameObject;
                spawnParticle.transform.localScale += new Vector3(4, 4, 4);
            }
            else
            {
                var spawnParticle = Instantiate(spawnParticlesPrefab, spawnPoint, spawnRotation) as GameObject;
            }

            newEnemy.transform.parent = gameObject.transform; // Sets new enemy as going under 'Enemies' gameObject
            activeEnemies++;
            yield return new WaitForSeconds(spawnWait);
        }
        spawnEnemies.Clear();
        spawnCall--;
        yield return null;

    }

}
