using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TutorialControllerCommands : MonoBehaviour
{
    ActionBasedController controller;
    XRDirectInteractor directInteractor;

    GameObject tutorialManagerObject;
    TutorialManager tutorialManager;
    public InputActionProperty velocityProperty;
    public InputActionProperty stickProperty;
    public InputActionProperty primaryButtonProperty;

    private GameObject hammer;
    public GameObject handModelPrefab;
    private GameObject spawnedHandModel;
    private GameObject handTransform;
    private GameObject boltLauncher;
    public GameObject lightning;
    public GameObject lightningStrike;
    public GameObject boltObject;
    public GameObject ultimate;
    public GameObject ultimateLightning;
    public GameObject player;
    public GameObject shockwave;
    public GameObject EmitterLightningHammer;
    public ParticleSystem emitterPS;

    public GameObject lightningPowerIconActive;
    public GameObject lightningPowerIconBlocked;
    public GameObject hammerStrikeIconActive;
    public GameObject hammerStrikeIconBlocked;
    public GameObject lightningStrikeIconActive;
    public GameObject lightningStrikeIconBlocked;
    public GameObject lightningBoltIconActive;
    public GameObject lightningBoltIconBlocked;
    public GameObject UltimateIconActive;
    public GameObject UltimateIconBlocked;

    private Animator handAnimator;
    public Vector3 Velocity { get; private set; } = Vector3.zero;
    public Vector2 stickVector { get; private set; } = Vector2.zero;
    public float primaryButton { get; private set; } = 0;

    Vector3 strikeOrigin;
    int strikeCount = 0;
    bool canUltimate = true;
    bool ultimateEnabled = false;
    bool isUltimate = false;
    public float flySpeed;
    bool canLightningAttack = false;
    bool canLightningStrike = false;
    bool canLightningStrike2 = false;
    bool canUltimateLightningAttack = false;
    bool canPowerHit = false;
    bool shouldFall = false;
    bool startFly = false;
    bool shouldShockwave = false;
    XRDirectInteractor hammerHand;

    bool canLightningBolt = false; // checks if ability is unlocked
    bool enableBoltAction = false; // checks if ability is in use
    bool boltLoopAllowed = true; // manager for the state of the bolt Loop
    int boltCount = 100;
    int boltInHand = 0;
    GameObject bolt;
    GameObject newBolt;
    public AudioClip boltSpawnClip;
    public AudioClip boltShootClip;
    public AudioClip lightningPowerClip;
    public AudioClip ultimateStrikeClip;
    AudioSource audioSource;


    private void Start()
    {
        hammer = GameObject.Find("Mjolnir");
        emitterPS = EmitterLightningHammer.GetComponent<ParticleSystem>();
        emitterPS.Stop();
        ultimateLightning.GetComponent<ParticleSystem>().Stop();
        ultimate.GetComponent<ParticleSystem>().Stop();
        lightning.GetComponent<ParticleSystem>().Stop();
        lightningStrike.GetComponent<ParticleSystem>().Stop();
        directInteractor = gameObject.GetComponent<XRDirectInteractor>();
        controller = gameObject.GetComponent<ActionBasedController>();
        spawnedHandModel = Instantiate(handModelPrefab, transform);
        handTransform = spawnedHandModel.transform.GetChild(0).gameObject;
        boltLauncher = handTransform.transform.GetChild(1).gameObject;
        handAnimator = spawnedHandModel.GetComponent<Animator>();
        tutorialManagerObject = GameObject.Find("TutorialManager");
        tutorialManager = tutorialManagerObject.GetComponent<TutorialManager>();
        audioSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {

        Velocity = velocityProperty.action.ReadValue<Vector3>(); // Get Controller Velocity
        stickVector = stickProperty.action.ReadValue<Vector2>(); // Get StickVector
        primaryButton = primaryButtonProperty.action.ReadValue<float>(); //Get primaryButton
        float triggerClick = controller.activateAction.action.ReadValue<float>(); //Get Trigger

        // Min Lightning Attack DifModifier
        if (tutorialManager.tutorialAllowLightningAttack == true && tutorialManager.lightningAttackCount == 0)
        {
            lightningPowerIconBlocked.SetActive(false);
            lightningPowerIconActive.SetActive(true);
            canLightningAttack = true;
        }

        // Min Lightning Strike Attack DifModifier

        if (tutorialManager.tutorialAllowLightningStrike == true && strikeCount < 1)
        {
            lightningStrikeIconBlocked.SetActive(false);
            lightningStrikeIconActive.SetActive(true);
            canLightningStrike = true;
            canLightningStrike2 = true;
        }

        // Min Lightning Bolt Attack DifModifier

        if (tutorialManager.tutorialAllowLightningBolt == true && tutorialManager.lightningBoltCount == 0)
        {
            lightningBoltIconBlocked.SetActive(false);
            lightningBoltIconActive.SetActive(true);
            canLightningBolt = true;
        }

        // Min Power Hit DifModifier

        if (tutorialManager.tutorialAllowPowerHit == true && tutorialManager.powerHitCount == 0)
        {
            hammerStrikeIconBlocked.SetActive(false);
            hammerStrikeIconActive.SetActive(true);
            canPowerHit = true;
        }

        // here I check the ultimateCount on another script for simplicity. It makes sure that the right hand script and the left hand
        // script both reference a third party for the ultimate activation count so that they don't conflict
        //                      
        if (tutorialManager.tutorialAllowUltimate == true && tutorialManager.ultimateCount == 0)
        {
            UltimateIconBlocked.SetActive(false);
            UltimateIconActive.SetActive(true);
            ultimateEnabled = true;
        }


        float gripClick = controller.selectAction.action.ReadValue<float>();

        if (directInteractor.isPerformingManualInteraction == false) // check hand is not holding anything
        {

            if (triggerClick > 0) // if trigger is clicked then punch
            {
                spawnedHandModel.tag = "damage";
                spawnedHandModel.transform.tag = "damage";
                handAnimator.SetBool("punch", true);
                tutorialManager.isPunchDone = true;
            }
            else // if not, then don't punch
            {
                spawnedHandModel.tag = "Untagged";
                handAnimator.SetBool("punch", false);
            }

        }

        // IF Player pushes JOYSTICK DOWN, Lightning Strike
        if (stickVector.y <= -0.93 && canLightningStrike && directInteractor.selectTarget == null)
        {
            strikeCount++;
            strikeOrigin = new Vector3(hammer.transform.position.x, (hammer.transform.position.y + 60f), hammer.transform.position.z); // defines spawnPoint as where hammer is
            StartCoroutine(CallLightningStrikeCooldown());
        }

        //If Player presses trigger while holding hammer, start power hit
        if (canPowerHit && triggerClick > 0 && hammerHand)
        {
            if (EmitterLightningHammer.GetComponent<AudioSource>().isPlaying == false)
            {
                EmitterLightningHammer.GetComponent<AudioSource>().PlayOneShot(lightningPowerClip);
            }
            StartCoroutine(CallPowerHitCooldown());
        }

        // Lightning Bolt Ability goes here
        if (boltLoopAllowed && canLightningBolt && !hammerHand && gripClick > 0)
        {
            enableBoltAction = true;
            boltLoopAllowed = false;
        }

        if (enableBoltAction == true)
        {
            if (tutorialManager.areBoltsDone == true)
            {
                Debug.Log("Bolt Count is Zero & Cooldown Called");
                enableBoltAction = false;
                StartCoroutine(LightningBoltCooldown());
            }

            if (gripClick > 0 && !hammerHand && boltInHand == 0)
            {
                // instantiate bolt
                boltInHand++;
                newBolt = Instantiate(boltObject, new Vector3(transform.position.x, transform.position.y, (transform.position.z + 0.2f)), transform.rotation) as GameObject;
                audioSource.PlayOneShot(boltSpawnClip);
                newBolt.transform.SetParent(gameObject.transform);
                newBolt.GetComponent<bolt>().inHand = true;
                Debug.Log("Bolt in hand!");
            }

            if (boltInHand == 1 && gripClick == 0 && !hammerHand)
            {
                //release bolt
                Debug.Log("boltCount: " + boltCount);
                Debug.Log("bolt released!");
                newBolt.GetComponent<Rigidbody>().velocity = boltLauncher.transform.forward * 15f;
                audioSource.PlayOneShot(boltShootClip);
                newBolt.transform.SetParent(null, true); // set the bolt's parent transform to nothing
                newBolt.GetComponent<bolt>().inHand = false;
                bolt = newBolt;
                newBolt = null;
                boltInHand--;
                boltCount--;
            }

        }




        if (directInteractor.selectTarget)
        {
            //if gripping something called Mjolnir, then...
            spawnedHandModel.SetActive(false);
        }
        else
        {
            spawnedHandModel.SetActive(true);
        }

        //Check if directInteractor is holding something
        if (directInteractor.selectTarget)
        {
            if (directInteractor.selectTarget.name == "Mjolnir") // if that thing is Mjolnir...
            {
                hammerHand = directInteractor;

                //If Player pushes JOYSTICK UP && !isUltimate, Lightning Attack
                if (!isUltimate && stickVector.y >= 0.93 && canLightningAttack)
                {
                    lightning.GetComponent<ParticleSystem>().Play();
                    if (lightning.GetComponent<AudioSource>().isPlaying == false)
                    {
                        lightning.GetComponent<AudioSource>().PlayOneShot(lightningPowerClip);
                    }
                    StartCoroutine(CallLightningAttackCooldown());
                }
                else // cancels lightning attack if player lets go
                {
                    lightning.GetComponent<ParticleSystem>().Stop();
                    lightning.GetComponent<AudioSource>().Stop();
                }

                // IF Player pushes JOYSTICK DOWN, Lightning Strike
                if (stickVector.y <= -0.93 && canLightningStrike2 && hammerHand && strikeCount < 1)
                {
                    strikeCount++;
                    strikeOrigin = new Vector3(Random.Range(-4.4f, 4.4f), 61, Random.Range(55f, 75f)); // defines spawnPoint as where hammer is
                    StartCoroutine(CallLightningStrikeCooldown2());
                }

                //If Player pushes PRIMARY BUTTON, initiate Ultimate
                if (primaryButton == 1 && canUltimate && ultimateEnabled)
                {
                    StartCoroutine(CallUltimateCooldown());
                    canUltimate = false;
                }

                if (canUltimateLightningAttack && isUltimate && stickVector.y >= 0.93) // if canUltimateLightningAttack is true and joystick is up...
                {
                    ultimateLightning.GetComponent<ParticleSystem>().Play();
                    emitterPS.Play();
                    if (ultimateLightning.GetComponent<AudioSource>().isPlaying == false)
                    {
                        ultimateLightning.GetComponent<AudioSource>().PlayOneShot(lightningPowerClip);
                    }
                }
                else // cancels lightning attack if player lets go
                {
                    ultimateLightning.GetComponent<ParticleSystem>().Stop();
                    emitterPS.Stop();
                    ultimateLightning.GetComponent<AudioSource>().Stop();
                }
            }

        }



        if (startFly == true) // begin moving towards the sky
        {
            player.transform.Translate(Vector3.up * Time.deltaTime * flySpeed);
        }

        if (player.transform.position.y == 34) // if you reach appropriate height, stop
        {
            startFly = false;
        }

        if (shouldFall == true) // if shouldFall is set to true, fall
        {
            player.transform.Translate(Vector3.down * Time.deltaTime * (2 * flySpeed));
        }

        if (player.transform.position.y == 0 && shouldShockwave) // if you reach proper height, stop falling.
        {
            shouldFall = false;
            StartCoroutine(Shockwave());

        }

        if (player.transform.position.y < 0) // Just in case, if you go below 0 height, then teleport back
        {
            player.transform.position = new Vector3(0, 0, 0);
            shouldFall = false;
        }

        if (player.transform.position.y > 35) // Just in case, if player goes too far up, pause 
        {
            startFly = false;
        }
    }


    IEnumerator CallLightningAttackCooldown()
    {
        tutorialManager.lightningAttackCount++;
        yield return new WaitForSeconds(5);
        lightning.GetComponent<ParticleSystem>().Stop();
        lightning.GetComponent<AudioSource>().Stop();
        canLightningAttack = false;
        lightningPowerIconActive.SetActive(false);
        lightningPowerIconBlocked.SetActive(true);
        tutorialManager.isLightningDone = true;
        yield return null;
    }

    IEnumerator CallLightningStrikeCooldown()
    {
        tutorialManager.lightningStrikeCount++;
        lightningStrike.transform.position = strikeOrigin;
        lightningStrike.GetComponent<ParticleSystem>().Play();
        lightningStrike.GetComponent<AudioSource>().Play();
        lightningStrikeIconActive.SetActive(false);
        lightningStrikeIconBlocked.SetActive(true);
        yield return new WaitForSeconds(5);
        strikeCount--;
        lightningStrike.GetComponent<ParticleSystem>().Stop();
        lightningStrike.GetComponent<AudioSource>().Stop();
        canLightningStrike = false;
        tutorialManager.isStrikeDone = true;
        Debug.Log("lightningStrikeCount: " + tutorialManager.lightningStrikeCount);

        yield return null;
    }

    IEnumerator CallLightningStrikeCooldown2()
    {
        tutorialManager.lightningStrikeCount++;
        lightningStrike.transform.position = strikeOrigin;
        lightningStrike.GetComponent<ParticleSystem>().Play();
        lightningStrike.GetComponent<AudioSource>().Play();
        lightningStrikeIconActive.SetActive(false);
        lightningStrikeIconBlocked.SetActive(true);
        yield return new WaitForSeconds(5);
        strikeCount--;
        lightningStrike.GetComponent<ParticleSystem>().Stop();
        lightningStrike.GetComponent<AudioSource>().Stop();
        canLightningStrike2 = false;
        tutorialManager.isStrikeTwoDone = true;

        yield return null;
    }

    IEnumerator LightningBoltCooldown()
    {
        tutorialManager.lightningBoltCount++;
        Debug.Log("Bolt Cooldown Started");
        canLightningBolt = false;
        lightningBoltIconActive.SetActive(false);
        lightningBoltIconBlocked.SetActive(true);
        tutorialManager.areBoltsDone = true;
        yield return null;
    }

    IEnumerator CallPowerHitCooldown()
    {
        tutorialManager.powerHitCount++;
        emitterPS.Play();
        yield return new WaitForSeconds(5);

        canPowerHit = false;
        EmitterLightningHammer.GetComponent<AudioSource>().Stop();
        emitterPS.Stop();
        hammerStrikeIconActive.SetActive(false);
        hammerStrikeIconBlocked.SetActive(true);
        tutorialManager.isPowerHitDone = true;
        yield return null;
    }

    IEnumerator CallUltimateCooldown()
    {
        isUltimate = true; // sets status of isUltimate to true
        canUltimateLightningAttack = true; // allow lightning attack from hammer
        startFly = true; // begin flying
        yield return new WaitForSeconds(1);
        ultimate.GetComponent<ParticleSystem>().Play(); // set lightning feet true
        ultimate.GetComponent<AudioSource>().PlayOneShot(ultimateStrikeClip);

        yield return new WaitForSeconds(15); // wait 20 seconds so player can attack

        shouldFall = true; // begin falling
        startFly = false; // turn off flying
        ultimate.GetComponent<ParticleSystem>().Stop(); // turn off lightning feet
        ultimate.GetComponent<AudioSource>().Stop();
        canUltimateLightningAttack = false; // turn off lighting hammer bool
        ultimateLightning.GetComponent<ParticleSystem>().Stop(); // turn off lightning hammer
        emitterPS.Stop();
        shouldShockwave = true;
        UltimateIconActive.SetActive(false);
        UltimateIconBlocked.SetActive(true);
        tutorialManager.ultimateCount++;
        yield return null;
    }

    IEnumerator Shockwave()
    {
        shockwave.SetActive(true);
        shockwave.transform.localScale = Vector3.Lerp(shockwave.transform.localScale, new Vector3(10, 10, 10), 10 * Time.deltaTime);
        isUltimate = false;
        yield return new WaitForSeconds(1.5f);
        shockwave.transform.localScale = Vector3.Lerp(shockwave.transform.localScale, Vector3.zero, 10 * Time.deltaTime);
        shockwave.SetActive(false);
        shouldShockwave = false;
        tutorialManager.isUltimateDone = true;
        yield return null;
    }



}
