using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class BackToMenu : MonoBehaviour
{
    public GameObject player;
    private GameObject end;
    
    void Start()
    {
        player = GameObject.Find("Player");
        end = GameObject.Find("LevelEnd");
    }

    void Update()
    {
        if(!player.GetComponent<Movimento>().isAlive)
        {
             Wait(() =>
            {
                CallMenuWhenDead();
            }, 3f);
        }

        else if(player.transform.position.x >= end.transform.position.x)
        {
            CallMenuWhenDead();
        }

        if(Input.GetButtonDown("Cancel"))
        {
            CallMenuWhenDead();
        }
    }

    public void CallMenuWhenDead()
    {
        SceneManager.LoadScene(0);
    }
    
    public void Wait(Action action, float delay)
    {
        StartCoroutine(WaitCoroutine(action, delay));
    }

    IEnumerator WaitCoroutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);

        action();
    }
}
