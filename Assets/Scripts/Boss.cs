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

    List<BossAttackManager.BossAttackType> randomAttacksBag;

    private Animator myAnimator;

    [SerializeField]
    float bossInvincibilityFramessTimeInSeconds = 2;
    float bossInvincibilityFramessTimer = 0;

    [SerializeField]
    private bool canBeDamaged = true;

    protected override void Start()
    {
        base.Start();

        randomAttacksBag = new List<BossAttackManager.BossAttackType>();

        player = GameObject.Find("PPPlayer").transform;

        targetPos = transform.position;

        cooldownTimer = cooldownTime;

        MakerandomWarpsBag();

        MakeRandomAttacksBag(3);


        BossAttackManager.onEndBossAttack += ActivateCooldown;

        //This next line is just testing to see if we can call the delegate!
        //BossAttackManager.onBeginBossAttack.Invoke(BossAttackManager.BossAttackType.Flooding);

        myAnimator = GetComponent<Animator>();

        

    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, flyAwayLerpSpeed * Time.deltaTime);



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

            
            if (canBeDamaged == false )
            {
                if (bossInvincibilityFramessTimer < bossInvincibilityFramessTimeInSeconds)
                {
                    bossInvincibilityFramessTimer += Time.deltaTime;
                }
                else
                {
                    canBeDamaged = true;
                    bossInvincibilityFramessTimer = 0;
                }

                
            }

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
                        //BossAttackManager.BossAttackType randomAttack = (BossAttackManager.BossAttackType)UnityEngine.Random.Range(0, 3); //System.Enum.GetValues(typeof(BossAttackManager.BossAttackType)).Length);

                        BossAttackManager.BossAttackType randomAttackPicked = randomAttacksBag[0];

                        randomAttacksBag.Remove(randomAttackPicked);

                        if (randomAttacksBag.Count == 0)
                        {
                            MakeRandomAttacksBag(3);
                        }

                        BossAttackManager.onBeginBossAttack.Invoke(randomAttackPicked);

                        Debug.Log("Take that!");

                    }

                    transform.LookAt(player.position);

                    
                    //currentBossState = BossState.cooldown;

                }

                break;


            case BossState.vulnerable:

                
                targetPos = new Vector3(0, 1, 0);

                myAnimator.SetBool("Vulnerable", true);

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

        canBeDamaged = false;

        myAnimator.SetTrigger("Damaged");

        if (staminaPoints <= 0 && currentBossState != BossState.Bitten)
        {
            BossAttackManager.onEndBossAttack -= ActivateCooldown;

            currentBossState = BossState.vulnerable;


            //This is where we need to set the parameter that they can go into their vulnerable animation, and that they can't go back to idle now.
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

    void MakeRandomAttacksBag(int numberOfAttacks)
    {
        for (int i = 0; i < numberOfAttacks; i++)
        {
            randomAttacksBag.Add( (BossAttackManager.BossAttackType)i );
        }

        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < randomAttacksBag.Count; t++)
        {
            BossAttackManager.BossAttackType temporary = randomAttacksBag[t];
            int randomIndex = UnityEngine.Random.Range(t, randomAttacksBag.Count);
            randomAttacksBag[t] = randomAttacksBag[randomIndex];
            randomAttacksBag[randomIndex] = temporary;
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
