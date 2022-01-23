using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rockGenerator : MonoBehaviour
{
    BoxCollider rockGeneratorCollider;
    GameObject spawnedRock;
    GameObject newRock;
    Rigidbody rockRB;
    Rigidbody spawnedRockRB;
    public GameObject rock;
    public GameObject target;
    Quaternion rockRotation = new Quaternion(0, 0, 0, 0);
    Vector3 targetPosition;
    Vector3 rockPosition;
    public float firingAngle;
    public float gravity = 9.8f;
    public bool shouldLetGo = false;
    public bool disableCol = false;
    

    private void Start()
    {
        target = GameObject.Find("Target");
        targetPosition = target.transform.position;
    }

    private void Update()
    {
        if (disableCol)
            GetComponent<SphereCollider>().enabled = false;

        if (shouldLetGo == true)
        {
            spawnedRock.transform.SetParent(null, true);
            spawnedRockRB.useGravity = true;
            spawnedRockRB.velocity = Vector3.zero;
            target = newRock.gameObject;
            targetPosition = newRock.transform.position;
        }

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "rockGenerator")
        {
            rockGeneratorCollider = collision.gameObject.GetComponent<BoxCollider>();
            rockGeneratorCollider.enabled = false;

            StartCoroutine(PickUp());

            Debug.Log("Rock Picked Up");

            StartCoroutine(ThrowRock());
        }
    }

    IEnumerator PickUp()
    {
        yield return new WaitForSeconds(0.7f);
        rockPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 2, gameObject.transform.position.z);
        spawnedRock = Instantiate(rock, rockPosition, rockRotation) as GameObject;
        spawnedRock.transform.SetParent(gameObject.transform);
        spawnedRockRB = spawnedRock.GetComponent<Rigidbody>();
        yield return null;
    }

    IEnumerator ThrowRock()
    {
        yield return new WaitForSeconds(4f);
        Debug.Log("Rock Thrown");
        rockGeneratorCollider.enabled = true; // turn on the generator collider again
        newRock = spawnedRock;
        rockRB = spawnedRockRB;
        spawnedRock = null;
        spawnedRockRB = null;
        newRock.transform.SetParent(null, true); // set the rocks' parent transform to nothing
        newRock.GetComponent<rock>().checkBridge = true; // at this point, check if rock is off bridge


        // Throw!
        Vector3 startPosition = newRock.gameObject.transform.position;

        // Calculate distance to target
        float target_Distance = Vector3.Distance(startPosition, targetPosition);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X & Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        newRock.transform.rotation = Quaternion.LookRotation(targetPosition - startPosition);
        float elapse_time = 0;
        while (elapse_time < flightDuration)
        {
            newRock.transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
            elapse_time += Time.deltaTime;
            yield return null;
        }

        newRock.GetComponent<rock>().shouldRockHarmEnemies = true;
        rockRB.constraints = RigidbodyConstraints.None;
        yield return new WaitForSeconds(1f);
        rockRB.useGravity = true;

        yield return null;
    }
}
