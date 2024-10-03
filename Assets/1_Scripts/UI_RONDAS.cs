using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RONDAS : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play("<link>RoundANimacion</link>", 0, 0f);
    }
}