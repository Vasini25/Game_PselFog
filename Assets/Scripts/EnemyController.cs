using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public AudioSource deathSound;
    private BoxCollider2D[] colliders;
    private Rigidbody2D rb;
    private float horizontal = 1f;
    private float movementSpeed = 0.7f;
    public float isAlive = 1f;
    private bool playerCloseEnough = false;
    private bool playerSameHigh = false;
    private bool isFacingRight = true;
    private bool isWalking = false;
    public bool attack = false;
    private float relativePos = 0f;
    private float relativeHeigh = 0f;

    public GameObject player;
    public Movimento cs;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    Animator anim;

    private void Start()
    {
        deathSound = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        colliders = GetComponents<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        player = GameObject.Find("Player");
        cs = player.GetComponent<Movimento>();
    }

    private void Update()
    {
        if(isAlive >= 1f)
        {
            Flip();
        }
        
        relativePos = player.transform.position.x - transform.position.x;
        relativeHeigh = player.transform.position.y - transform.position.y;

        //checa altura relativa 
        if(relativeHeigh <= 3.5f && relativeHeigh >= -3.5f)
        {
            playerSameHigh = true;
            horizontal = 0f;
        }
        else
        {
            playerSameHigh = false;
        }

        //Ccheca distancia para o player
        if(relativePos >= 1.8f && relativePos <= 10f && playerSameHigh)
        {
            horizontal = 1f;
            isWalking = true;
            playerCloseEnough = false;
        }
        else if(relativePos <= -1.8f && relativePos >= -10f && playerSameHigh)
        {
            horizontal = -1f;
            isWalking = true;
            playerCloseEnough = false;
        }
        else if(relativePos <= 1.8f && relativePos >= -1.8f)
        {
            horizontal = 0f;
            isWalking = false;
            playerCloseEnough = true;
            anim.SetTrigger("inRange");
        }
        else
        {
            horizontal = 0f;
            isWalking = false;
            playerCloseEnough = false;
        }

        //checa se deve atacar baseado na posição do player
        if(playerSameHigh && playerCloseEnough)
        {
            horizontal = 0f;
            attack = true;
        }
        else
        {
            attack = false;
        }

        if(!anim.GetBool("inRange"))
        {
            horizontal *= movementSpeed * isAlive;
        }
        else
        {
            horizontal = 0f;
        }

        if(IsGrounded())
        {
            rb.velocity = new Vector2(horizontal, 0f);
        }

        anim.SetBool("walk", isWalking);
    }

    public void Kill()
    {
        deathSound.Play();
        /*Wait(() =>
        {
            Destroy(gameObject);
        }, 0.15f);*/

        ComplexDeath();
    }

    private void ComplexDeath()
    {
        isAlive = 0f;
        anim.SetBool("dead", true);

        /*
        foreach (var boxCollider2D in colliders)
        {
            boxCollider2D.enabled = true;
        }*/

        colliders[1].enabled = false;
        rb.mass = 0.5f;
        
        Wait(() =>
        {
            Destroy(gameObject);
        }, 3f);
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

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && relativePos < 0f || !isFacingRight && relativePos > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}