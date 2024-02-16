using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expire : MonoBehaviour
{
    public float duration;

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0f)
            Destroy(gameObject);
    }
}
