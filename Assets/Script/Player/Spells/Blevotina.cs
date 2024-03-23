using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Blevotina : MonoBehaviour
{
    public float blevotinaDamage;
    public float blevotinaRadius;
    public float blevotinaSpeed;
    public float blevotinaKnockoutForce;
    public int blevotinaAngle;

    private CircleCollider2D myCollider;
    private Rigidbody2D myRigidbody;
    private float limit;
    private float multiplier;
    private float speedRandom;
    private Vector2 blevotinaResultPos;
    private System.Random random = new System.Random();
    
    void Awake()
    {
        PlayerSpellSystem player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSpellSystem>();

        myCollider = GetComponent<CircleCollider2D>();
        myCollider.isTrigger = true;
        myCollider.radius = blevotinaRadius;

        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.isKinematic = true;

        limit = Mathf.Tan(Convert.ToSingle(blevotinaAngle*Math.PI/360));
        Vector2 addition = Vector2.up;
        multiplier = random.Next(-100, 100);
        multiplier /= 100f;
        speedRandom = random.Next(70, 100);
        speedRandom /= 100f;
        addition.y = addition.y * limit * multiplier;
        blevotinaResultPos = player.mousePos.normalized + addition;

        myRigidbody.velocity = blevotinaResultPos.normalized * blevotinaSpeed * speedRandom;

        Destroy(gameObject, 10f);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            PlayerSpellSystem player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSpellSystem>();
            //Damage dealing
            other.GetComponent<Rigidbody2D>().AddForce(blevotinaKnockoutForce * GetComponent<Rigidbody2D>().velocity.normalized, ForceMode2D.Impulse);
            Destroy(gameObject);
        }
    }
}