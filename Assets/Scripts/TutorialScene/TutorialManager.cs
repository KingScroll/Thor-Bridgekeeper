using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    // TutorialHammer Variable
    TutorialHammer tutHammer;
    public GameObject tutorialCanvas;

    // Module Bools for Tutorial
    public bool tutorialAllowLightningAttack = false;
    public bool tutorialAllowLightningStrike = false;
    public bool tutorialAllowLightningBolt = false;
    public bool tutorialAllowPowerHit = false;
    public bool tutorialAllowUltimate = false;

    public bool isPunchDone = false;
    public bool isLightningDone = false;
    public bool isStrikeDone = false;
    public bool isStrikeTwoDone = false;
    public bool areBoltsDone = false;
    public bool isPowerHitDone = false;
    public bool isUltimateDone = false;


    public GameObject leftHandController;
    public GameObject rightHandController;
    public GameObject leftSelect;
    public GameObject rightSelect;
    public GameObject hammerHitTargets;
    public GameObject punchTarget;
    public GameObject hammerThrowTargets;
    public GameObject lightningAttackTarget;
    public GameObject lightningBoltTargets;

    // Tutorial Text
    public GameObject hammerGrabText;
    public GameObject hammerHitTargetsText;
    public GameObject hammerThrowText;
    public GameObject punchText;
    public GameObject lightningAttackText;
    public GameObject lightningStrikeText;
    public GameObject lightningStrikeText2;
    public GameObject lightningBoltText;
    public GameObject powerHitText;
    public GameObject UltimateText;
    public GameObject ultimateAttackText;

    bool shouldHammerGrab = false;
    bool shouldHammerHit = false;
    bool shouldHammerThrow = false;
    bool shouldPunch = false;
    bool shouldLightningAttack = false;
    bool shouldLightningStrike = false;
    bool shouldLightningStrike2 = false;
    bool shouldLightningBolt = false;
    bool shouldPowerHit = false;
    bool shouldUltimate = false;
    bool shouldTutorialOver = false;

    public int lightningAttackCount = 0;
    public int lightningStrikeCount = 0;
    public int lightningBoltCount = 0;
    public int powerHitCount = 0;
    public int ultimateCount = 0;

    private void Start()
    {
        shouldHammerGrab = true;
        tutHammer = GameObject.Find("Mjolnir").GetComponent<TutorialHammer>();
    }

    private void Update()
    {
        if (shouldHammerGrab == true)
            StartCoroutine(HammerGrab());

        if (shouldHammerHit == true)
            StartCoroutine(HammerHit());

        if (shouldPunch == true)
            StartCoroutine(Punch());

        if (shouldHammerThrow == true)
            StartCoroutine(HammerThrow());

        if (shouldLightningAttack == true)
            StartCoroutine(LightningAttack());

        if (shouldLightningStrike == true)
            StartCoroutine(LightningStrike());

        if (shouldLightningStrike2 == true)
            StartCoroutine(LightningStrikePartTwo());

        if (shouldLightningBolt == true)
            StartCoroutine(LightningBolt());

        if (shouldPowerHit == true)
            StartCoroutine(PowerHit());

        if (shouldUltimate == true)
            StartCoroutine(Ultimate());

        if (shouldTutorialOver == true)
            StartCoroutine(TutorialOver());

    }


   IEnumerator HammerGrab()
    {
        Debug.Log("Hammer Grab Initiated");
        hammerGrabText.SetActive(true);
        if (tutHammer.isGrabbed == true)
        {
            hammerGrabText.SetActive(false);
            shouldHammerGrab = false;
            shouldPunch = true;
        }
        yield return null;
    }

    IEnumerator HammerHit()
    {
        Debug.Log("Hammer Hit Initiated");
        hammerHitTargetsText.SetActive(true);
        hammerHitTargets.SetActive(true);

        if (hammerHitTargets.transform.childCount <= 0)
        {
            hammerHitTargetsText.SetActive(false);
            hammerHitTargets.SetActive(false);
            shouldHammerHit = false;
            shouldHammerThrow = true;
        }

        yield return null;
    }


    IEnumerator HammerThrow()
    {
        Debug.Log("Hammer Throw Initiated");
        hammerThrowText.SetActive(true);
        hammerThrowTargets.SetActive(true);

        if (hammerThrowTargets.transform.childCount <= 0)
        {
            hammerThrowTargets.SetActive(false);
            hammerThrowText.SetActive(false);
            shouldHammerThrow = false;
            shouldLightningAttack = true;
        }

        yield return null;
    }


    IEnumerator Punch()
    {
        punchText.SetActive(true);
        if (isPunchDone)
        {
            punchText.SetActive(false);
            shouldPunch = false;
            shouldHammerHit = true;
        }
        yield return null;
    }

    IEnumerator LightningAttack()
    {
        Debug.Log("Lightning Attack Initiated");
        tutorialAllowLightningAttack = true;
        lightningAttackText.SetActive(true);
        lightningAttackTarget.SetActive(true);

        if (isLightningDone)
        {
            lightningAttackText.SetActive(false);
            tutorialAllowLightningAttack = false;
            shouldLightningStrike = true;
            shouldLightningAttack = false;
        }
        yield return null;
    }

    IEnumerator LightningStrike()
    {
        Debug.Log("Lightning Strike Initiated");
        tutorialAllowLightningStrike = true;
        lightningStrikeText.SetActive(true);
        if (isStrikeDone)
        {
            lightningStrikeText.SetActive(false);
            shouldLightningStrike2 = true;
        }

        yield return null;
    }

    IEnumerator LightningStrikePartTwo()
    {
        lightningStrikeText2.SetActive(true);
        if (isStrikeTwoDone)
        {
            tutorialAllowLightningStrike = false;
            lightningStrikeText2.SetActive(false);
            shouldLightningBolt = true;
            shouldLightningStrike2 = false;
        }

        yield return null;
    }

    IEnumerator LightningBolt()
    {
        Debug.Log("Lightning Bolt Initiated");
        tutorialAllowLightningBolt = true;
        lightningBoltText.SetActive(true);
        lightningBoltTargets.SetActive(true);
        if (lightningBoltTargets.transform.childCount <= 0)
        {
            tutorialAllowLightningBolt = false;
            lightningBoltText.SetActive(false);
            lightningBoltTargets.SetActive(false);
            areBoltsDone = true;
            shouldPowerHit = true;
            shouldLightningBolt = false;
        }
        yield return null;
    }

    IEnumerator PowerHit()
    {
        Debug.Log("Power Hit Initiated");
        tutorialAllowPowerHit = true;
        powerHitText.SetActive(true);
        if (isPowerHitDone)
        {
            tutorialAllowPowerHit = false;
            powerHitText.SetActive(false);
            shouldUltimate = true;
            shouldPowerHit = false;
        }
        yield return null;
    }

    IEnumerator Ultimate()
    {
        Debug.Log("Ultimate Initiated");
        tutorialAllowUltimate = true;
        UltimateText.SetActive(true);
        ultimateAttackText.SetActive(true);
        if (isUltimateDone)
        {
            tutorialAllowUltimate = false;
            UltimateText.SetActive(false);
            shouldTutorialOver = true;
            shouldUltimate = false;
        }

        yield return null;
    }

    IEnumerator TutorialOver()
    {
        Debug.Log("Tutorial Over Initiated");
        tutorialCanvas.SetActive(true);
        leftHandController.SetActive(false);
        rightHandController.SetActive(false);
        leftSelect.SetActive(true);
        rightSelect.SetActive(true);
        shouldTutorialOver = false;
        yield return null;
    }
}
