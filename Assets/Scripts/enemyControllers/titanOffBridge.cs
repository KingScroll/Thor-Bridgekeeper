using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class titanOffBridge : MonoBehaviour
{

    public bool shouldKill;
    public bool shouldCheckBridge = true;
    // Update is called once per frame
    void Update()
    {
        if (shouldCheckBridge)
        {
            if (gameObject.transform.position.y < -2 || gameObject.transform.position.x > 10 || gameObject.transform.position.y < -10)
                shouldKill = true;
        }

    }
}
