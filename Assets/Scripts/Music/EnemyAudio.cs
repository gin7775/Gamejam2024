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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Play_Enemy_Sound_PolloMuerto();
        }
    }

    public void Play_Enemy_Sound_PolloMuerto()
    {
        polloMuerto.Play();
    }

}
