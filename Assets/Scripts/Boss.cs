using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lukas.Utilities;
using System;
using System.Linq;
using static UnityEngine.EventSystems.EventTrigger;

public class Boss : BitableObject
{
    [SerializeField]
    int staminaPoints = 3;

    [SerializeField]
    float flyAwayLerpSpeed = .1f;

    [SerializeField]
    float flyAwayDistance = 10f;

    Vector3 targetPos;


    Transform player;


    public enum BossState
    {
        vulnerable,
        attacking,
        cooldown,
        Bitten

    }

    [SerializeField]
    public BossState currentBossState = BossState.cooldown;

    [SerializeField]
    float cooldownTime = 5f;
    float cooldownTimer;

    [SerializeField]
    Transform[] warpPoints = new Transform[4];

    List<Transform> randomWarpsBag = new List<Transform>();


    protected override void Start()
    {
        base.Start();

        player = GameObject.Find("PPPlayer").transform;

        targetPos = transform.position;

        cooldownTimer = cooldownTime;

        MakerandomWarpsBag();


        BossAttackManager.onEndBossAttack += ActivateCooldown;

        //This next line is just testing to see if we can call the delegate!
        //BossAttackManager.onBeginBossAttack.Invoke(BossAttackManager.BossAttackType.Flooding);

    }

    void Update()
    {
        

        if (currentBossState != BossState.vulnerable)
        {
            if (Vector3.Distance(player.position, targetPos) < flyAwayDistance)
            {
                Transform randomWarpPointPicked = randomWarpsBag[0];

                randomWarpsBag.Remove(randomWarpPointPicked);

                if (randomWarpsBag.Count == 0)
                {
                    MakerandomWarpsBag();
                }

                targetPos = randomWarpPointPicked.position;
                
            }

            transform.position = Vector3.Lerp(transform.position, targetPos, flyAwayLerpSpeed * Time.deltaTime);
        }

        switch (currentBossState)
        {
            case BossState.cooldown:

                if (Vector3.Distance(player.position, transform.position) < 40)
                {
                    transform.LookAt(player.position);

                    

                }

                if (cooldownTimer > 0)
                {
                    cooldownTimer -= Time.deltaTime;
                }
                else
                {
                    cooldownTimer = cooldownTime;

                    currentBossState = BossState.attacking;
                }



                break;


            case BossState.attacking:

                if (Vector3.Distance(player.position, transform.position) < 100)
                {
                    if (!BossAttackManager.currentlyAttacking)
                    {
                        BossAttackManager.BossAttackType randomAttack = (BossAttackManager.BossAttackType)UnityEngine.Random.Range(0, 2); //System.Enum.GetValues(typeof(BossAttackManager.BossAttackType)).Length);

                        BossAttackManager.onBeginBossAttack.Invoke(randomAttack);

                        Debug.Log("Take that!");

                    }

                    transform.LookAt(player.position);

                    
                    //currentBossState = BossState.cooldown;

                }

                break;


            case BossState.vulnerable:
                //This is when the boss will be too tired to run away!
                
                break;


            case BossState.Bitten:
                //This is where the boss will blow up!

                SceneManagement.instance.SceneTransition(2);





                break;

        }


    }

    public void GetTired()
    {
        staminaPoints--;

        if (staminaPoints <= 0 && currentBossState != BossState.Bitten)
        {
            BossAttackManager.onEndBossAttack -= ActivateCooldown;

            currentBossState = BossState.vulnerable;



        }
        else
        {

            Transform randomWarpPointPicked = randomWarpsBag[0];

            randomWarpsBag.Remove(randomWarpPointPicked);

            if (randomWarpsBag.Count == 0)
            {
                MakerandomWarpsBag();
            }

            targetPos = randomWarpPointPicked.position;

           
        }

        BossAttackManager.onEndBossAttack.Invoke();
    }

    void MakerandomWarpsBag()
    {
        for (int i = 0; i < warpPoints.Length; i++)
        {
            randomWarpsBag.Add(warpPoints[i]);
        }

        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < randomWarpsBag.Count; t++)
        {
            Transform temporary = randomWarpsBag[t];
            int randomIndex = UnityEngine.Random.Range(t, randomWarpsBag.Count);
            randomWarpsBag[t] = randomWarpsBag[randomIndex];
            randomWarpsBag[randomIndex] = temporary;
        }
    }

    protected virtual bool OnTriggerEnter(Collider other)
    {
        if (base.OnTriggerEnter(other))
        {
            if (currentBossState == BossState.vulnerable)
            {
                currentBossState = BossState.Bitten;

                return true;
            }



        }
        else
        {
            return false;
        }
        
        return false;
    }




    protected virtual bool OnTriggerStay(Collider other)
    {
        //Debug.Log("STAYSTAY");

        if (base.OnTriggerStay(other))
        {
            currentBossState = BossState.Bitten;

            return true;

        }
        else
        {
            return false;
        }
        
        return false;
    }

    

    void ActivateCooldown()
    {
        currentBossState = BossState.cooldown;
    }


}
