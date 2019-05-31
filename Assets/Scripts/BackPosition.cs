using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPosition : MonoBehaviour
{
    [HideInInspector] public bool back = false;

    Vector3 original;
    Quaternion originalQua;

    void Start()
    {
        original = transform.position;
        originalQua = transform.rotation;
    }

    void Update()
    {
        if (back)
        {
            transform.position = Vector3.Lerp(transform.position, original, 2f * Time.deltaTime);
            transform.rotation = originalQua;
        }
    }
}
