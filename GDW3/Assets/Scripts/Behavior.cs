﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Globalization;
using System;
public class Behavior : MonoBehaviour
{
    public GameObject rect;
    public GameObject rect2;

    public Vector3 playerDirection = Vector3.right;
    public int playerHealth = 100;

    private float cooldown = 0;

    private float cooldownB = 0;

    private static bool isDashing = false;

    private static float dashTimer = 0.0f;

    [Range(0, 3)]
    public int playerType = 0;

    [Range(0, 1000)]
    public int jumpModifyer = 175;

    [Range(0, 5)]
    public float cooldownDuration = 1;

    [Range(0, 5)]
    public float cooldownBDuration = 0.5f; //Basic slash

    private float heldPower = 0;

    [Range(0, 100)]
    public float chargePowerModifyer = 1;

    public Transform chargePoint;
    public Transform basicPoint;

    [Range(0,10)]
    public float chargeRangeHeight = 1;

    [Range(0, 10)]
    public float chargeRangeWidth = 1;

    [Range(0, 10)]
    public float basicRangeHeight = 1;

    [Range(0, 10)]
    public float basicRangeWidth = 1;

    Vector3 chargeDimensions = Vector3.up;
    Vector3 basicDimensions = Vector3.up;

    Quaternion orientationMaybe = new Quaternion(0, 0, 0, 1);

    public LayerMask enemyLayers;
    public LayerMask bulletLayers;

    public followMouse please;
    public followMouse please2;
    public followMouse please3;

    private int direction = 2;
    private int particleID = -1;
    private float particleDelay = 0;

    public float chargeLimit = 1000;

    public bool locked = false; //This is only for the server, used to lock code that should not run here, I didn't delete in case something happens with the client code and I need to look at this for reference

    // Start is called before the first frame update
    void Start()
    {
        soundsManager.soundsSingleton.startBackgroundSong("firstSong");
        switch (playerType)
        {
            case 0: //Ninja character
                playerHealth = 100;
                jumpModifyer = 250;
                cooldownDuration = 1.25f;
                cooldownBDuration = 0.25f;
                chargePowerModifyer = 10;
                chargeLimit = 900;
                break;
            ///case 1: //HammerMan
            ///    playerHealth = 200;
            ///    jumpModifyer = 0;
            ///    cooldownDuration = 0.0f;
            ///    cooldownBDuration = 0.45f;
            ///    chargePowerModifyer = 30;
            ///    chargeLimit = 1500;
            ///    break;
            ///case 0: //Ninja character
            ///    playerHealth = 100;
            ///    jumpModifyer = 12;
            ///    cooldownDuration = 1.25f;
            ///    cooldownBDuration = 0.25f;
            ///    chargePowerModifyer = 10;
            ///    chargeLimit = 900;
            ///    break;
            ///case 0: //Ninja character
            ///    playerHealth = 100;
            ///    jumpModifyer = 12;
            ///    cooldownDuration = 1.25f;
            ///    cooldownBDuration = 0.25f;
            ///    chargePowerModifyer = 10;
            ///    chargeLimit = 900;
            ///    break;
        }
            
        if (this.gameObject.name != "player 1 (1)")
        {
            locked = true;
        }
    }

    int mouse1Buffer = 0;
    private Vector3 lastUpPos = new Vector3(0, 0, 0);
    private Vector3 lastTwoUpPos = new Vector3(0, 0, 0);

    public void lastUpdatePos(Vector3 newPos)
    {
        lastTwoUpPos = lastUpPos;
        lastUpPos = newPos;
    }

    // Update is called once per frame
    
    void Update()
    {
        switch (pauseGame.singleton.stateOfGame)
        {
            case pauseGame.generalState.PAUSED:
                break;
            case pauseGame.generalState.PLAYING:
                if (locked)
                {
                    //Prediction
                    Vector3 howShouldMove = Vector3.Normalize(new Vector3(lastUpPos.x - lastTwoUpPos.x, 0, lastUpPos.z - lastTwoUpPos.z));
                    transform.position = transform.position + howShouldMove * Time.deltaTime * 5;
                }

                if (!locked)
                    transform.Translate(playerDirection * 5 * Time.deltaTime);
                if (!locked)
                {
                    if (playerDirection != new Vector3(0, 0, 0) && particleDelay == 0)
                    {
                        playerAnimationScript.singleton.playRun();

                        if (particleID == -1)
                        {
                            particleID = particleManager.singleton.startParticles(gameObject.transform.position);
                            particleManager.singleton.setParent(particleID, gameObject);
                        }
                        switch (direction)
                        {
                            //-       +
                            // 1, 2, 3
                            // 4,  , 5
                            // 6, 7, 8
                            case 1:

                                particleManager.singleton.changeFacing(particleID, gameObject.transform.position + new Vector3(1, 0, -1));
                                break;
                            case 2:
                                particleManager.singleton.changeFacing(particleID, gameObject.transform.position + new Vector3(0, 0, -1));

                                break;
                            case 3:
                                particleManager.singleton.changeFacing(particleID, gameObject.transform.position + new Vector3(-1, 0, -1));

                                break;
                            case 4:
                                particleManager.singleton.changeFacing(particleID, gameObject.transform.position + new Vector3(1, 0, 0));

                                break;
                            case 5:
                                particleManager.singleton.changeFacing(particleID, gameObject.transform.position + new Vector3(-1, 0, 0));

                                break;
                            case 6:
                                particleManager.singleton.changeFacing(particleID, gameObject.transform.position + new Vector3(1, 0, 1));

                                break;
                            case 7:
                                particleManager.singleton.changeFacing(particleID, gameObject.transform.position + new Vector3(0, 0, 1));

                                break;
                            case 8:
                                particleManager.singleton.changeFacing(particleID, gameObject.transform.position + new Vector3(-1, 0, 1));

                                break;

                        }
                    }
                    else if (particleID != -1 && particleDelay == 0)
                    {
                        particleDelay = 0.5f;
                    }
                    else if (particleID != -1 && particleDelay > 0)
                    {
                        particleDelay -= Time.deltaTime;
                    }
                    else if (particleDelay < 0 && particleID != -1)
                    {
                        particleManager.singleton.deActivateParticle(particleID);
                        particleID = -1;
                        particleDelay = 0;
                    }
                    else if (playerDirection == new Vector3(0, 0, 0))
                    {
                        playerAnimationScript.singleton.playIddle();

                    }
                }

                if (!locked)
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        playerDirection.z = 1;
                    }
                    else if (Input.GetKey(KeyCode.S))
                    {
                        playerDirection.z = -1;
                    }
                    else
                    {
                        playerDirection.z = 0;
                    }
                    if (Input.GetKey(KeyCode.A))
                    {
                        playerDirection.x = -1;
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        playerDirection.x = 1;
                    }
                    else
                    {
                        playerDirection.x = 0;
                    }
                    switch (playerDirection.x)
                    {
                        case -1: //left
                            switch (playerDirection.z)
                            {
                                case -1: //down
                                    callUpdateDirection(6);
                                    break;
                                case 0:
                                    callUpdateDirection(4);
                                    break;
                                case 1:
                                    callUpdateDirection(1);
                                    break;
                            }
                            break;
                        case 0:
                            switch (playerDirection.z)
                            {
                                case -1:
                                    callUpdateDirection(7);
                                    break;
                                case 0:
                                    break;
                                case 1:
                                    callUpdateDirection(2);
                                    break;
                            }
                            break;
                        case 1:
                            switch (playerDirection.z)
                            {
                                case -1:
                                    callUpdateDirection(8);
                                    break;
                                case 0:
                                    callUpdateDirection(5);
                                    break;
                                case 1:
                                    callUpdateDirection(3);
                                    break;
                            }
                            break;
                    }


                    if (Input.GetKey(KeyCode.LeftShift) && cooldown == 0 && isDashing == false)
                    {
                        isDashing = true;
                    }

                    if (isDashing == true)
                    {
                        playerDirection *= jumpModifyer * Time.deltaTime;
                        dashTimer += Time.deltaTime;
                        cooldown = cooldownDuration;

                        if (dashTimer >= 0.1f)
                        {
                            isDashing = false;
                            dashTimer = 0.0f;
                        }
                    }

                    if (Input.GetKey(KeyCode.Mouse1))
                    {
                        chargeAttack();
                        mouse1Buffer = 1;
                    }
                    else if (mouse1Buffer == 1)
                    {
                        soundsManager.soundsSingleton.playSoundEffect(UnityEngine.Random.Range(11, 15));
                        mouse1Buffer = 0;
                        clientScript.singleton.doAttack(1, heldPower, direction);
                        releaseChargeAttack();
                    }


                    if (Input.GetKey(KeyCode.Mouse0) && cooldownB == 0)
                    {
                        soundsManager.soundsSingleton.playSoundEffect(UnityEngine.Random.Range(11, 15));
                        basicAttack();
                        cooldownB = cooldownBDuration;
                        clientScript.singleton.doAttack(0, 30, direction);
                    }
                }
                break;
        }
    }

    void callUpdateDirection(int num)
    {
        direction = num;
        please.updateDirection(num);
        please2.updateDirection(num);
        please3.updateDirection(num);
    }

    public void callBasicAttack()
    {
        basicAttack();
    }

    void basicAttack()
    {
        playerAnimationScript.singleton.playAttack();

        bool hitOneAtLeast = false;

        Collider[] enemiesHit = Physics.OverlapBox(basicPoint.position, new Vector3(basicDimensions.x + basicRangeHeight, 1, basicDimensions.z + basicRangeWidth), orientationMaybe, enemyLayers); //Change to just basicRange when we find the right numbers
        Collider[] bulletHit = Physics.OverlapBox(basicPoint.position, new Vector3(basicDimensions.x + basicRangeHeight, 1, basicDimensions.z + basicRangeWidth), orientationMaybe, bulletLayers); //Change to just basicRange when we find the right numbers

        foreach (Collider enemies in enemiesHit)
        {
            //enemies.GetComponent<enemyBehavior>().takeDmg(heldPower);
            enemies.GetComponent<enemyBehavior>().takeDmg(30);
            //soundsManager.soundsSingleton.playSoundEffect("");
            //Nothing yet because we don't have the sound for it
            hitOneAtLeast = true;
        }

        foreach (Collider bullets in bulletHit)
        {
            //enemies.GetComponent<enemyBehavior>().takeDmg(heldPower);
            bullets.GetComponent<bullet>().die();
            //soundsManager.soundsSingleton.playSoundEffect("");
            //Nothing yet because we don't have the sound for it
            hitOneAtLeast = true;
        }
        //if (hitOneAtLeast == true) { saveLoadingManager.singleton.changeHitAccuracy(1); } else { saveLoadingManager.singleton.changeHitAccuracy(0); }

    }

    void chargeAttack()
    {
        if (heldPower <= chargeLimit)
        {
            heldPower += 1.0f * chargePowerModifyer;
            rect2.GetComponent<UpdateLength>().updateLength((chargeLimit - heldPower) / chargeLimit);

        }
    }

    public void callHeldPower(float heldValue, float directionServer)
    {
        heldPower = heldValue;
        direction = (int)directionServer;
        releaseChargeAttack();
    }

    void releaseChargeAttack()
    {
        playerAnimationScript.singleton.playAttack();

        //do the attack
        bool hitOneAtLeast = false;
        Collider[] enemiesHit = Physics.OverlapBox(chargePoint.position, new Vector3(chargeDimensions.x + chargeRangeHeight, 1, chargeDimensions.z + chargeRangeWidth), orientationMaybe, enemyLayers); //Change to just chargeRange when we find the right numbers
        
        foreach(Collider enemies in enemiesHit)
        {
            //enemies.GetComponent<enemyBehavior>().takeDmg(heldPower);
            enemies.GetComponent<enemyBehavior>().doKnockback(heldPower, direction);
            hitOneAtLeast = true;
        }
        //if (hitOneAtLeast == true) { saveLoadingManager.singleton.changeHitAccuracy(1); } else { saveLoadingManager.singleton.changeHitAccuracy(0); }
        heldPower = 0;
        rect2.GetComponent<UpdateLength>().updateLength((chargeLimit - heldPower) / chargeLimit);

    }

    private void OnDrawGizmosSelected()
    {


        Gizmos.DrawWireCube(chargePoint.position, new Vector3 (chargeDimensions.x + chargeRangeHeight, 1, chargeDimensions.z + chargeRangeWidth));
        Gizmos.DrawWireCube(basicPoint.position, new Vector3 (basicDimensions.x + basicRangeHeight, 1, basicDimensions.z + basicRangeWidth));
    }

    void LateUpdate() //Update that is called after all the other updates
    {
        cooldown = (cooldown <= 0 ? 0 : cooldown - 1 * Time.deltaTime);
        cooldownB = (cooldownB <= 0 ? 0 : cooldownB - 1 * Time.deltaTime);
    }

    public void takeDmg(int dmg)
    {
        soundsManager.soundsSingleton.playSoundEffect(UnityEngine.Random.Range(16, 18));
        playerAnimationScript.singleton.playHurt();
        //Nothing for now
        playerHealth -= dmg;
        rect.GetComponent<UpdateLength>().updateLength(playerHealth / 100.0f);
        if (playerHealth <= 0)
        {
            SceneManager.LoadScene("playerDeath");
            clientScript.singleton.playersDied();
            //SceneManager.LoadScene("playerStats");
        }
        //Debug.Log("Took " + dmg + " damage");

    }
}
