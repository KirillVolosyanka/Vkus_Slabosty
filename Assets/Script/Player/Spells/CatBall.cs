using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class CatBall : MonoBehaviour
{
    public float ballDamage;
    public float ballRadius;
    public float ballSpeed;
    public float ballKnockoutForce;

    private CircleCollider2D myCollider;
    private Rigidbody2D myRigidbody;
    
    void Awake()
    {
        PlayerSpellSystem player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSpellSystem>();

        myCollider = GetComponent<CircleCollider2D>();
        myCollider.isTrigger = true;
        myCollider.radius = ballRadius;

        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.isKinematic = true;

        myRigidbody.velocity = player.mousePos.normalized * ballSpeed;

        Destroy(gameObject, 2f);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            PlayerSpellSystem player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSpellSystem>();
            //Damage dealing
            other.GetComponent<Rigidbody2D>().AddForce(ballKnockoutForce * player.mousePos.normalized, ForceMode2D.Impulse);
        }
        Destroy(gameObject);
    }
}