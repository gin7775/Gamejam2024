using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLife : MonoBehaviour
{
    [SerializeField] int Life = 5;
    void Start()
    {
        SetObjectLifeTime();
    }

    void SetObjectLifeTime()
    {
        StartCoroutine(ObjectLife());
    }
    IEnumerator ObjectLife()
    {
        yield return new WaitForSeconds(Life);
        Destroy(gameObject);
    }
}
