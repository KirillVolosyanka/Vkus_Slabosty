using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class PlayerSpellSystem : MonoBehaviour
{
    [Header("General")]
    public Camera cam;
    [Space(5)]

    [Header("Mana")]
    public float maxMana;
    public float currentMana;
    [Space(5)]

    [Header("CatBall")]
    public CatBall catBall;
    public KeyCode catBallKey;
    public GameObject catBallCastPoint;
    public float catBallCooldown;
    public float catBallManaCost;
    [Space(5)]
    
    [Header("Teleport")]
    public KeyCode teleportKey;
    public float teleportCooldown;
    public float teleportManaCost;
    public float teleportExplosionDamage;
    public float teleportExplosionRadius;
    public float teleportExplosionKnockoutForce;
    [Space(5)]

    [Header("Blevotina")]
    public float blevotinaManaCost;
    public KeyCode blevotinaKey;
    public GameObject blevotinaCastPoint;
    public float blevotinaCooldown;
    public Blevotina blevotina;
    public int blevotinaAmount;
    [Space(5)]

    [Header("EarthSlam")]
    public GameObject earthChecker;
    public Vector2 earthCheckerSize;
    public GameObject earthSlamPoint;
    public float earthSlamForce;

    #region GeneralPrivates
    internal Vector2 mousePos;
    private Vector2 playerPos;
    #endregion

    #region CatBallPrivates
    private float catBallTimer;
    private bool isCatBallCooldown = false;
    private Vector2 catBallCastPos;
    #endregion

    #region TeleportPrivates
    private float teleportTimer;
    private bool isTeleportCooldown = false;
    #endregion    

    #region BlevotinaPrivates
    private float blevotinaTimer;
    private bool isBlevotinaCooldown = false;
    private Vector2 blevotinaCastPos;
    #endregion

    void Start()
    {
        currentMana = maxMana;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) CastEarthSlam();

        #region BlevotinaChecks
        if (Input.GetKeyDown(blevotinaKey)) CastBlevotina();

        if (isBlevotinaCooldown) blevotinaTimer -= Time.deltaTime;

        if (blevotinaTimer < 0f) isBlevotinaCooldown = false;
        #endregion
        
        #region CatBallChecks
        if (Input.GetKeyDown(catBallKey) && !isCatBallCooldown && currentMana >= catBallManaCost) CastCatBall();

        if (isCatBallCooldown) catBallTimer -= Time.deltaTime;

        if (catBallTimer < 0f) isCatBallCooldown = false;
        #endregion

        #region TeleportChecks
        if (Input.GetKeyDown(teleportKey) && !isTeleportCooldown && currentMana >= teleportManaCost) CastTeleport();

        if (isTeleportCooldown) teleportTimer -= Time.deltaTime;

        if (teleportTimer < 0f) isTeleportCooldown = false;
        #endregion
    }

    public void CastCatBall()
    {
        currentMana -= catBallManaCost;
        isCatBallCooldown = true;
        catBallTimer = catBallCooldown;
        playerPos = cam.WorldToScreenPoint(GetComponent<Transform>().position);
        catBallCastPos = cam.WorldToScreenPoint(catBallCastPoint.transform.position);
        mousePos = Input.mousePosition;
        playerPos = catBallCastPos - playerPos;
        mousePos -= catBallCastPos;
        if (mousePos.x * playerPos.x <= 0)
        {
            GetComponent<PlayerMovement>().Turn();
            playerPos = cam.WorldToScreenPoint(GetComponent<Transform>().position);
            catBallCastPos = cam.WorldToScreenPoint(catBallCastPoint.transform.position);
            mousePos = Input.mousePosition;
            playerPos = catBallCastPos - playerPos;
            mousePos -= catBallCastPos;
        }

        Instantiate(catBall, catBallCastPoint.transform.position, catBallCastPoint.transform.rotation);
    }

    public void CastTeleport()
    {
        currentMana -= teleportManaCost;
        isTeleportCooldown = true;
        teleportTimer = teleportCooldown;
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        GetComponent<Transform>().position = mousePos;
    }   

    public void CastBlevotina()
    {
        currentMana -= blevotinaManaCost;
        isBlevotinaCooldown = true;
        blevotinaTimer = blevotinaCooldown;
        playerPos = cam.WorldToScreenPoint(GetComponent<Transform>().position);
        blevotinaCastPos = cam.WorldToScreenPoint(blevotinaCastPoint.transform.position);
        mousePos = Input.mousePosition;
        playerPos = blevotinaCastPos - playerPos;
        mousePos -= blevotinaCastPos;
        if (mousePos.x * playerPos.x <= 0)
        {
            GetComponent<PlayerMovement>().Turn();
            playerPos = cam.WorldToScreenPoint(GetComponent<Transform>().position);
            blevotinaCastPos = cam.WorldToScreenPoint(blevotinaCastPoint.transform.position);
            mousePos = Input.mousePosition;
            playerPos = blevotinaCastPos - playerPos;
            mousePos -= blevotinaCastPos;
        }

        for (int i = 0; i < blevotinaAmount; i++)
        {
            Instantiate(blevotina, blevotinaCastPoint.transform.position, blevotinaCastPoint.transform.rotation);
        }
    }

    public void CastEarthSlam()
    {
        Collider2D earth = Physics2D.OverlapBox(earthChecker.transform.position, earthCheckerSize, 0);
        Vector2 slamSize = earth.transform.localScale;
        Collider2D[] damaged = Physics2D.OverlapBoxAll(earthSlamPoint.transform.position, new Vector2(slamSize.x, 1), 0);

        foreach (var c in damaged)
        {
            if (c.CompareTag("Enemy"))
            {
                //Damage enemy
                c.GetComponent<Rigidbody2D>().AddForce(Vector2.up * earthSlamForce, ForceMode2D.Impulse);
            }
        }
    }
}