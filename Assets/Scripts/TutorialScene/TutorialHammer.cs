using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


//I Should NOT use OnTriggerEnter or Exit to make things happen, stick to listener events
// OnTrigger events happen to frequently

public class TutorialHammer: MonoBehaviour
{
    XRGrabInteractable m_GrabInteractable;
    ActionBasedController controller;
    XRDirectInteractor directInteractor;

    [Tooltip("The Transform that the object will return to")]
    [SerializeField] Vector3 returnToPosition;
    [SerializeField] XRInteractionManager interactionManager;
    protected bool shouldReturnHome { get; set; }
    bool isController;
    public bool isGrabbed;
    int selectCount = 0;
    string powerController;
    [SerializeField] float AttractorSpeed;
    bool isPressed;
    bool checkForReturn;
    float pressMeter;
    bool checkCollider = false;
    ControllerCommands controllerVelocity = null;
    public float velocityNum;
    float velocityX;
    float velocityY;
    float velocityZ;
    Rigidbody hammerRB;

    // Start is called before the first frame update
    void Awake()
    {
        m_GrabInteractable = GetComponent<XRGrabInteractable>();
        controller = GetComponent<ActionBasedController>();
        hammerRB = GetComponent<Rigidbody>();

        shouldReturnHome = true;
        checkForReturn = false;
        isGrabbed = false;
        //Debug.Log("Set Grab Interactable: " + m_GrabInteractable);
    }

    void Update()
    {
        if (checkForReturn == true)
        {
            isReturnClicked();
        }
        if (selectCount > 0 && !isGrabbed && isPressed && shouldReturnHome)
        {
            //Debug.Log("Hammer Return initiated!");
            returnToPosition = controller.transform.position;
            //Debug.Log("Return to Position: " + returnToPosition);
            StartCoroutine(ReturnHome());
        }

        if (isGrabbed)
            HammerVelocityCheck();

        if (selectCount < 1)
        {
            hammerRB.isKinematic = true;
        }
        else
        {
            hammerRB.isKinematic = false;
        }

    }

    private void OnEnable()
    {
        //Debug.Log("Interactable enabled");
        m_GrabInteractable.selectExited.AddListener(OnSelectExit);
        //Debug.Log("OnSelectExit listener added");
        m_GrabInteractable.selectEntered.AddListener(OnSelect);
        //Debug.Log("OnSelectEntered listener added");
    }

    private void OnDisable()
    {
        // Debug.Log("Interactable disabled");
        m_GrabInteractable.selectExited.RemoveListener(OnSelectExit);
        //Debug.Log("OnSelectExit listener removed");
        m_GrabInteractable.selectEntered.RemoveListener(OnSelect);
        //Debug.Log("OnSelectEntered listener removed");
    }

    private void OnSelect(SelectEnterEventArgs arg0)
    {
        selectCount++;
        controllerVelocity = arg0.interactor.GetComponent<ControllerCommands>();
        if (arg0.interactor.gameObject.name == "LeftHand Controller")
        {
            GameObject.Find("RightHand Controller").GetComponent<XRDirectInteractor>().enabled = false;
            powerController = "LeftHand Controller";
        }
        if (arg0.interactor.gameObject.name == "RightHand Controller")
        {
            GameObject.Find("LeftHand Controller").GetComponent<XRDirectInteractor>().enabled = false;
            powerController = "RightHand Controller";
        }
        isGrabbed = true;
        checkForReturn = false;
        isPressed = false;
        checkCollider = false;
        //Debug.Log("isGrabbed: " + isGrabbed);
        CancelInvoke("ReturnHome");
        // Debug.Log("ReturnHome Invoke canceled");
    }
    private void OnSelectExit(SelectExitEventArgs arg0)
    {
        controllerVelocity = null;
        isGrabbed = false;
        checkForReturn = true;
        gameObject.tag = "damage";
        //Debug.Log("isGrabbed: " + isGrabbed);
    }

    IEnumerator ReturnHome()
    {
        // Debug.Log("Returning...");
        checkForReturn = false;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        Quaternion returnRotation = Quaternion.Euler(-100, 0, -90);
        Vector3 returnVelocity = Vector3.zero;
        Vector3 returnAngularVelocity = Vector3.zero;
        Vector3 currentTransform = transform.position;
        Quaternion currentRotation = transform.rotation;

        transform.position = Vector3.MoveTowards(currentTransform, returnToPosition, AttractorSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(currentRotation, returnRotation, AttractorSpeed * Time.deltaTime / AttractorSpeed);
        rb.velocity = Vector3.MoveTowards(rb.velocity, returnVelocity, AttractorSpeed * Time.deltaTime);
        rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, returnAngularVelocity, AttractorSpeed * Time.deltaTime);

        float distance = Vector3.Distance(returnToPosition, transform.position);

        if (distance <= 30 && distance >= 4)
        {
            gameObject.GetComponent<AudioSource>().enabled = true;
        }

        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (ControllerCheck(other.gameObject) && checkCollider == true && (other.gameObject.name == powerController))
        {
            pressMeter = controller.selectAction.action.ReadValue<float>();
            if (pressMeter > 0 && other.gameObject.name == powerController)
            {
                interactionManager.ForceSelect(directInteractor, m_GrabInteractable);
                gameObject.GetComponent<AudioSource>().enabled = false;
            }

            return;
        }

        if (ControllerCheck(other.gameObject))
        {
            return;
        }

        var socketInteractor = other.gameObject.GetComponent<XRSocketInteractor>();

        if (socketInteractor == null)
            shouldReturnHome = true;

        else if (socketInteractor.CanSelect(m_GrabInteractable))
        {
            shouldReturnHome = false;
        }
        else
            shouldReturnHome = true; //The socket interactor exists and CANNOT select the Grab object
    }

    private void OnTriggerExit(Collider other)
    {
        if (ControllerCheck(other.gameObject))
            return;

        shouldReturnHome = true;
    }

    bool ControllerCheck(GameObject collidedObject) // this function checks if object is a controller
    {
        if (collidedObject.gameObject.GetComponent<XRBaseController>() != null && collidedObject.gameObject.name == powerController)
        {
            controller = collidedObject.GetComponent<ActionBasedController>(); // set the collided object to be controller
            directInteractor = collidedObject.GetComponent<XRDirectInteractor>(); // Set collided object to be directInteractor
            isController = true;
        }
        else
        {
            isController = false;
        }

        return isController;
    }

    private void isReturnClicked()
    {
        pressMeter = controller.selectAction.action.ReadValue<float>();
        if (pressMeter > 0)
        {
            isPressed = true;
            // Debug.Log("isPressed = true");
            checkCollider = true;
        }
    }

    private void HammerVelocityCheck()
    {
        Vector3 hammerVelocity = controllerVelocity ? controllerVelocity.Velocity : Vector3.zero;

        velocityX = hammerVelocity.x;
        velocityY = hammerVelocity.y;
        velocityZ = hammerVelocity.z;

        velocityNum = velocityX + velocityY + velocityZ;

        if (isGrabbed && (velocityNum > 0.5 || velocityNum < -0.5))
        {
            gameObject.tag = "damage";
        }
        else
        {
            gameObject.tag = "ignoreEnemyCollide";
        }
    }
}
