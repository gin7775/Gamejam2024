using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{

    [SerializeField] AudioSource[] sonidoPolloEnemigo;
    [SerializeField] AudioSource polloMuerto;

    // Start is called before the first frame update
    void Start()
    {
        sonidoPolloEnemigo[Random.Range(0, sonidoPolloEnemigo.Length)].Play();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Sound_PolloMuerto()
    {
        polloMuerto.Play();
    }

}