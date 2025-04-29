using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstGameObjectOptions : MonoBehaviour
{
    public GameObject firstGameObject;
    public GameObject firstGameObject2;
    public GameObject firstGameObject3;
    public GameObject firstGameObject4;
    public void FirstGame()
    {

        EventSystem.current.SetSelectedGameObject(firstGameObject);
    }

    public void FirstGame2()
    {

        EventSystem.current.SetSelectedGameObject(firstGameObject2);
    }
    public void FirstGame3()
    {

        EventSystem.current.SetSelectedGameObject(firstGameObject3);
    }
    public void FirstGame4()
    {

        EventSystem.current.SetSelectedGameObject(firstGameObject4);
    }
}
