using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEditor.Experimental.GraphView;

public class ChickenLouncher : MonoBehaviour
{
    public int chickenType = 0;
    [SerializeField] GameObject[] proyectiles = null;
    [SerializeField] float proyectileForce = 10;
    [SerializeField] int chickenCurrentUses = 0;
    [SerializeField] int chickenMaxUses = 3;
    [SerializeField] GameObject headBox;
    [SerializeField] GameObject swingBox;
    // Gestor de Vidas Player
    [SerializeField] int health = 3;
    [SerializeField] int maxHealth = 3;
    [SerializeField] GameObject[] eggs;
    [SerializeField] Collider[] shickensDetected;
    [SerializeField] int pickUpRange = 3;
    [SerializeField] float distanciaComparativa,distanciaActual;
    [SerializeField] GameObject polloElegido;
    [SerializeField] MusicManager musicManager;
    [SerializeField] Transform handPosition;
    public List <GameObject> currentProyectile;
    public GameObject[] shildCounter;
    GameObject proyectile;
    Vector3 projectilePos;
    private Animator anim;
    //Esto es para la muerte
    bool muriendo;

    private void Start()
    {
        musicManager = FindAnyObjectByType<MusicManager>();
        muriendo = false;
        distanciaComparativa = 1000;
        anim = GetComponent<Animator>();
    }
    private void Update()
    {

    }

    public void OnShoot(InputValue value)
    {
        anim.SetBool("Carrying", false);
        anim.SetTrigger("Throw");
        musicManager.Play_FX_RecogerPollo();
        Shoot(chickenType);

        //Debug.Log("Dispara");
    }

    public void OnAttack(InputValue value)
    {
        anim.SetTrigger("Attack");
        Attack(chickenType);
        //Debug.Log("Ataca");
    }

    public void OnPick(InputValue value)
    {
        musicManager.Play_FX_RecogerPollo();
        RetrieveChicken(chickenType);
        //Debug.Log("Coge");
    }

    void RetrieveChicken(int chickenNumber)
    {
        anim.SetBool("Carrying", true);
        chickenType = chickenNumber;
        if (chickenType == 3)
        {

            LifeUp(1);
        }
        if (chickenType >= 5 || chickenType < 0)
        {
            chickenType = 1;
        }
    }


    void LifeUp(int extraLife)
    {
        health += extraLife;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    public void ReciveDamage(int damage)
    {

        //UpdateLifeUI();
        health -= damage;
        if (health <= 0)
        {
            PlayerDeath();
        }
        //Debug.Log(health);
    }


    private void UpdateLifeUI()
    {
        if (eggs != null)
        {
            eggs[health - 1].GetComponent<Animator>().SetTrigger("Break");
        }
    }

    void PlayerDeath()
    {
        if(!muriendo) 
        {
            //AQUI DEBERÍAN IR PARTICULAS DE PLUMAS TMB
            muriendo = true;
            StartCoroutine(TransicionMuerte());
            
        }
      
    }
    IEnumerator TransicionMuerte()
    {
        musicManager.Play_FX_Player_PolloMuerto();
        yield return new WaitForSeconds(2f);
        //Debug.Log("Ye dead!");
        Destroy(this.gameObject);
    }



    void UpdateWeapon()
    {
        chickenCurrentUses++;
        if (chickenCurrentUses == chickenMaxUses)
        {
            chickenType = 0;
        }
    }

    void Shoot(int AmmoType)
    {
        projectilePos = transform.position;
        projectilePos += transform.forward;
        projectilePos += transform.up;

        switch (AmmoType)
        {
            case 0:
                //Debug.Log("Got no chickens");
                break;

            case 1:
                //Debug.Log("Lounching Chicken");
                proyectile = Instantiate(proyectiles[0], projectilePos, Quaternion.identity);
                proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);

                chickenType = 0;
                break;

            case 2:
                projectilePos = transform.position;
                projectilePos += transform.forward;
                projectilePos += transform.up;
                //Debug.Log("Lounching Chicken");
                proyectile = Instantiate(proyectiles[1], projectilePos, Quaternion.identity);
                proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);

                chickenType = 0;
                break;

            case 3:
                projectilePos = transform.position;
                projectilePos += transform.forward;
                projectilePos += transform.up;
                //Debug.Log("Lounching Chicken");
                proyectile = Instantiate(proyectiles[2], projectilePos, Quaternion.identity);
                proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);

                chickenType = 0;
                break;

            case 4:
                projectilePos = transform.position;
                projectilePos += transform.forward;
                projectilePos += transform.up;
                //Debug.Log("Lounching Chicken");
                proyectile = Instantiate(proyectiles[3], projectilePos, Quaternion.identity);
                proyectile.GetComponent<Rigidbody>().AddForce(transform.forward * proyectileForce);

                chickenType = 0;
                break;
        }
        if (currentProyectile != null)
        {
            //shildCounter = handPosition.gameObject.GetComponentsInChildren<GameObject>();
            foreach (GameObject pollo in currentProyectile)
            {
                pollo.transform.SetParent(handPosition, false);
                Destroy(pollo);
            }
            currentProyectile.Clear();
        }
    }

    public void Attack(int AmmoType)
    {
        switch (AmmoType)
        {
            case 0:
                //Debug.Log("Got no chickens");
                HeadBut();
                break;
            case 1:
                ChickenSwing();
                UpdateWeapon();
                break;
            case 2:
                ChickenSwing();
                UpdateWeapon();
                break;
            case 3:
                ChickenSwing();
                UpdateWeapon();
                break;
            case 4:
                ChickenSwing();
                UpdateWeapon();
                break;
        }
    }

    public void HeadBut()
    {
        if (headBox != null)
        {
            //Debug.Log("Headbutting");
            StartCoroutine(ActivateCollider(headBox));
        }
        else
        {
            //Debug.Log("Headbut Collider is missing");
        }
    }
    public void ChickenSwing()
    {
        if (swingBox != null)
        {
            //Debug.Log("Swinging a chicken");
            StartCoroutine(ActivateCollider(swingBox));
        }
        else
        {
            //Debug.Log("Swing Collider is missing");
        }
    }
    IEnumerator ActivateCollider(GameObject collider)
    {
        yield return new WaitForSeconds(0.2f);
        collider.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        collider.SetActive(false);
    }

    /*public void DealDamage(GameObject objetive, int damage)
    {
        GameManager.Instance.chickenEnemyTakeDamage(objetive, damage);
    }*/

    
    private void OnTriggerEnter(Collider other)
    {
       // Debug.Log("He colisionado con " + other.gameObject.name);
        if (other.gameObject.CompareTag("Corpse"))
        {
            //Debug.Log("Detecta Corpse");
            if(other.gameObject.GetComponent<ChickenCorpse>()!=null)
            RetrieveChicken(other.gameObject.GetComponent<ChickenCorpse>().chickenType);
            GameObject cadaver= other.gameObject;
            cadaver.transform.position = handPosition.position;
            cadaver.transform.SetParent(handPosition, true);
            currentProyectile.Add(cadaver);

        }
    }
}
