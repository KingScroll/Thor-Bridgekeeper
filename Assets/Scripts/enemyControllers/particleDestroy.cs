using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CallDestroy());
    }

    IEnumerator CallDestroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
        yield return null;
    }

}
