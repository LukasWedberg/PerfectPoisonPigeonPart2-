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

            case BossAttackType.Madness:
                onCurrentAttack = FloodingFunction;
                onCurrentAttack += CirclingEyesFunction;
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

        SetEyesTargetPositions(Vector3.right * 100, Vector3.right * -100);

        rightEyeController.currentEyeState = StinkEyeEnemy.EyeState.Aiming;

        leftEyeController.currentEyeState = StinkEyeEnemy.EyeState.Aiming;



        //At the bottom we should also reset the timers!

        waveAttackTimer = 0;

        camAttackTimer = 0;


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
        else if (playerFocusTimer != 8)
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

   
    


    void Start()
    {
        onBeginBossAttack += BeginAttacking;

        onEndBossAttack += EndAttacking;


        player = GameObject.Find("PPPlayer").transform;

        boss = GameObject.Find("Chefil").transform;

        cam = Camera.main;


        leftEyeController = leftEye.GetComponent<StinkEyeEnemy>();
        rightEyeController = rightEye.GetComponent<StinkEyeEnemy>();
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

    void SetEyesSentryPosition()
    {



    }
}
