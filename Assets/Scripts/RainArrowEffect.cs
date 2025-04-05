using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainArrowEffect : MonoBehaviour
{
    public float fallSpeed = 25f;
    public float destroyAfterSeconds = 3f;

    void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }
}
