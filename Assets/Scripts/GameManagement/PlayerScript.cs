using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour

{

    public float playerHealth = 230f;
    public float currentPlayerHealth;
    public GameObject HealthBar;
    public GameObject damageOverlay;
    float alphaModifier = 0f;
    public GameObject leftHandController;
    public GameObject rightHandController;
    public GameObject gameOverMenu;
    public GameObject abilitiesBar;
    public GameObject bigScore;
    public GameObject sceneSelectLeft;
    public GameObject sceneSelectRight;
    int gameOverCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentPlayerHealth = playerHealth;
    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.GetComponent<Image>().fillAmount = currentPlayerHealth / playerHealth;

        if (currentPlayerHealth <= 30f)
        {
            alphaModifier = 0.55f;
        }

        if (currentPlayerHealth <= 60f && currentPlayerHealth > 30f)
        {
            alphaModifier = 0.45f;
        }

        if (currentPlayerHealth <= 90f && currentPlayerHealth > 60f)
        {
            alphaModifier = 0.35f;
        }

        if (currentPlayerHealth > 90f)
        {
            alphaModifier = 0f;
        }

        if (currentPlayerHealth <= 0f && gameOverCount == 0)
        {
            alphaModifier = 0.66f;
            Time.timeScale = 0;
            GameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "club")
        {
            currentPlayerHealth -= 7f;
            DamageEffects();
        }

        if (other.gameObject.name == "giantClub")
        {
            currentPlayerHealth -= 15f;
            DamageEffects();
        }

        if (other.gameObject.name == "rock(Clone)")
        {
            currentPlayerHealth -= 25f;
            DamageEffects();
        }
    }


    private void DamageEffects()
    {
        GetComponent<AudioSource>().Play();
        damageOverlay.GetComponent<Image>().color = new Color(damageOverlay.GetComponent<Image>().color.r, damageOverlay.GetComponent<Image>().color.g, damageOverlay.GetComponent<Image>().color.b, alphaModifier);
    }

    private void GameOver()
    {
        GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag("damage");
        foreach (GameObject go in gameObjectArray)
        {

            go.SetActive(false);
        }
            gameOverCount++;
        gameOverMenu.SetActive(true);
        abilitiesBar.SetActive(false);
        bigScore.SetActive(false);
        leftHandController.SetActive(false);
        rightHandController.SetActive(false);
        sceneSelectLeft.SetActive(true);
        sceneSelectRight.SetActive(true);
        // turn on controller ray interactor
    }
}
