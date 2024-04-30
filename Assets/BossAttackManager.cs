using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackManager : MonoBehaviour
{
    public enum BossAttackType
    {
        Flooding,
        CameraEyes,
        SentryEyes,
        CirclingEyes,
        Madness

    }

    private Transform boss;

    private Transform player; 


    public delegate void BeginBossAttack(BossAttackType attackToDo);
    public static BeginBossAttack onBeginBossAttack;




    public delegate void EndBossAttack();
    public static EndBossAttack onEndBossAttack;



    public delegate void CurrentAttack();
    public static CurrentAttack onCurrentAttack;

    public static bool currentlyAttacking;

    private bool firstFunctionCall = true;

    void BeginAttacking(BossAttackType attackToDo)
    {
        currentlyAttacking = true;

        switch(attackToDo){

            case BossAttackType.Flooding:
                onCurrentAttack = FloodingFunction;
               
                break;

            case BossAttackType.CirclingEyes:
                onCurrentAttack = CirclingEyesFunction;

                break;

            case BossAttackType.CameraEyes: 
                onCurrentAttack = CameraEyesFunction; 
                
                break;

            case BossAttackType.SentryEyes:
                onCurrentAttack = SentryEyesFunction; 
                break;

            case BossAttackType.Madness:
                onCurrentAttack = FloodingFunction;
                onCurrentAttack += SentryEyesFunction;
                onCurrentAttack += CameraEyesFunction;

                break;

        }

    }

    void EndAttacking()
    {
        currentlyAttacking = false;

        onCurrentAttack = null;

        firstFunctionCall = true;

        WaterManager.instance.targetWaterLevel = -59;

        WaterManager.instance.targetAmplitude = 1;

        SetEyesTargetPositions(new Vector3(20, 100,0), new Vector3(20, 100, 0));

        SetEyesSentryPositions(new Vector3(20, 100, 0), new Vector3(20, 100, 0));

        rightEyeController.currentEyeState = StinkEyeEnemy.EyeState.Aiming;

        leftEyeController.currentEyeState = StinkEyeEnemy.EyeState.Aiming;



        //At the bottom we should also reset the timers!

        waveAttackTimer = 0;

        camAttackTimer = 0;

        sentryAttackTimer = 0;


    }




    void FloodingFunction()
    {
        Debug.Log("You should really clean the streets!");

        if (firstFunctionCall)
        {
            WaterManager.instance.targetWaterLevel = -4;

            WaterManager.instance.targetAmplitude = 5;

            firstFunctionCall = false; 
            
            Debug.Log("This is the first frame of an attack!");
        }


        float playerBossXDist = boss.position.x - player.position.x;

        if ( Mathf.Abs(playerBossXDist) > 10)
        {

            targetWaveSpeed = 1*Mathf.Sign(playerBossXDist);


           
            

        }

        currentWaveSpeed = Mathf.Lerp(currentWaveSpeed, targetWaveSpeed, 1 * Time.deltaTime);

        WaterManager.instance.speed = currentWaveSpeed;

        if (waveAttackTimer < waveAttackTime)
        {
            waveAttackTimer += Time.deltaTime;
        }
        else
        {
            onEndBossAttack.Invoke();

            waveAttackTimer = 0;
        }


    }

    void CirclingEyesFunction()
    {
        Debug.Log("Torn-eye-do!");
    }

    void CameraEyesFunction()
    {

        //Debug.Log("We *look* the same, but you won't for much longer!");

        if (firstFunctionCall)
        {
            
            firstFunctionCall = false;

            Debug.Log("This is the first frame of an attack!");
            rightEyeController.target = cam.transform;
            leftEyeController.target = cam.transform;

            playerFocusTimer = 0;

            leftEyeController.aimTimer = 0;

            rightEyeController.aimTimer = 0;


            if (leftEyeController.currentEyeState != StinkEyeEnemy.EyeState.Bitten)
            {
                leftEyeController.currentEyeState = StinkEyeEnemy.EyeState.Aiming;
                leftEyeController.aimTimer = -3;
            }

            leftEyeController.chargeTimer = 0;


            if (rightEyeController.currentEyeState != StinkEyeEnemy.EyeState.Bitten)
            {
                rightEyeController.currentEyeState = StinkEyeEnemy.EyeState.Aiming;
                rightEyeController.aimTimer = -3;
            }

            rightEyeController.chargeTimer = 0;

        }

        SetEyesTargetPositions(
            cam.transform.position + cam.transform.forward * eyeForwardOffset + cam.transform.right * eyeSidewaysOffset
            , cam.transform.position + cam.transform.forward * eyeForwardOffset - cam.transform.right * eyeSidewaysOffset


            );

        if (playerFocusTimer < playerFocusTime)
        {
            playerFocusTimer += Time.deltaTime;
        }
        else if (rightEyeController.target != player.transform && leftEyeController.target != player.transform)
        {
            if (leftEyeController.currentEyeState != StinkEyeEnemy.EyeState.Bitten)
            {
                leftEyeController.currentEyeState = StinkEyeEnemy.EyeState.ChargingBlast;
                leftEyeController.aimTimer = 0;
            }


            if (rightEyeController.currentEyeState != StinkEyeEnemy.EyeState.Bitten)
            {
                rightEyeController.currentEyeState = StinkEyeEnemy.EyeState.Aiming;
                rightEyeController.aimTimer = 0;
            }

            rightEyeController.target = player.transform;
            leftEyeController.target = player.transform;

            playerFocusTimer = 8;

        }
        else
        {
            
           
        }


        if (camAttackTimer < camAttackTime)
        {
            camAttackTimer += Time.deltaTime;
        }
        else
        {
            onEndBossAttack.Invoke();

            camAttackTimer = 0;

        }

    }

    void SentryEyesFunction()
    {
        if (firstFunctionCall)
        {

            firstFunctionCall = false;

            Debug.Log("This is the first frame of an attack!");
            rightSentryEyeController.target = cam.transform;
            leftSentryEyeController.target = cam.transform;

            playerFocusTimer = 0;

            leftSentryEyeController.aimTimer = 0;

            rightSentryEyeController.aimTimer = 0;


            SetEyesSentryPositions(
            new Vector3(0, 1f, 0)
            , new Vector3(38.5f, 1f, 0)
            );

        }


        if (playerFocusTimer < playerFocusTime)
        {
            playerFocusTimer += Time.deltaTime;
        }
        else if (rightSentryEyeController.target != player.transform && leftSentryEyeController.target != player.transform)
        {
            if (leftSentryEyeController.currentEyeState != StinkEyeEnemy.EyeState.Bitten)
            {
                leftSentryEyeController.currentEyeState = StinkEyeEnemy.EyeState.Aiming;
                leftSentryEyeController.aimTimer = 0;
            }


            if (rightSentryEyeController.currentEyeState != StinkEyeEnemy.EyeState.Bitten)
            {
                rightSentryEyeController.currentEyeState = StinkEyeEnemy.EyeState.Aiming;
                rightSentryEyeController.aimTimer = 0;
            }

            rightSentryEyeController.target = player.transform;
            leftSentryEyeController.target = player.transform;

            playerFocusTimer = 8;

        }
        else
        {


        }


        if (sentryAttackTimer < sentryAttackTime)
        {
            sentryAttackTimer += Time.deltaTime;
        }
        else
        {
            onEndBossAttack.Invoke();

            sentryAttackTimer = 0;

        }

    }



    //Now we'll be listing stats for fine-tuning attack performance. Up above is all the delegate stuff

    //Wave attack stats
    public float currentWaveSpeed = 1;
    public float targetWaveSpeed = 1;
    public float waveAttackTime = 20;
    public float waveAttackTimer = 0;


    //Camera attack stats
    public float camAttackTime = 20;
    public float camAttackTimer = 0;
    
    public Transform leftEye;
    public Transform rightEye;
    StinkEyeEnemy leftEyeController;
    StinkEyeEnemy rightEyeController;

    Camera cam;
    

    public float eyeForwardOffset = 3;
    public float eyeSidewaysOffset = 3;
    public float playerFocusTime = 3;
    public float playerFocusTimer = 0;


    //Sentry attack stats!

    public float sentryAttackTime = 10;
    public float sentryAttackTimer = 0;

    public Transform leftSentryEye;
    public Transform rightSentryEye;
    StinkEyeEnemy leftSentryEyeController;
    StinkEyeEnemy rightSentryEyeController;




    void Start()
    {
        onBeginBossAttack += BeginAttacking;

        onEndBossAttack += EndAttacking;


        player = GameObject.Find("PPPlayer").transform;

        boss = GameObject.Find("Chefil").transform;

        cam = Camera.main;


        leftEyeController = leftEye.GetComponent<StinkEyeEnemy>();
        rightEyeController = rightEye.GetComponent<StinkEyeEnemy>();


        leftSentryEyeController = leftSentryEye.GetComponent<StinkEyeEnemy>();
        rightSentryEyeController = rightSentryEye.GetComponent<StinkEyeEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyAttacking)
        {
            onCurrentAttack.Invoke();

        }
    }


    void SetEyesTargetPositions(Vector3 leftPos, Vector3 rightPos)
    {
        leftEyeController.targetPos = leftPos;
        rightEyeController.targetPos = rightPos;


    }

    void SetEyesSentryPositions(Vector3 leftPos, Vector3 rightPos)
    {
        leftSentryEyeController.targetPos = leftPos;
        rightSentryEyeController.targetPos = rightPos;


    }
}
