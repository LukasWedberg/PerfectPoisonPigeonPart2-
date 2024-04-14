using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lukas.Utilities;

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

            transform.position = Vector3.Lerp(transform.position, targetPos, flyAwayLerpSpeed);
        }

        switch (currentBossState)
        {
            case BossState.cooldown:

                if (Vector3.Distance(player.position, transform.position) < 40)
                {
                    transform.LookAt(player.position);

                    if (cooldownTimer > 0)
                    {
                        cooldownTimer -= Time.deltaTime;
                    }
                    else
                    {
                        cooldownTimer = cooldownTime;

                        currentBossState = BossState.attacking;
                    }

                }



                break;


            case BossState.attacking:

                if (Vector3.Distance(player.position, transform.position) < 40)
                {

                    transform.LookAt(player.position);

                    Debug.Log("Take that!");

                    currentBossState = BossState.cooldown;

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
            currentBossState = BossState.vulnerable;

            Transform randomWarpPointPicked = randomWarpsBag[0];

            randomWarpsBag.Remove(randomWarpPointPicked);

            if (randomWarpsBag.Count == 0)
            {
                MakerandomWarpsBag();
            }

            targetPos = randomWarpPointPicked.position;

        }
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
            int randomIndex = Random.Range(t, randomWarpsBag.Count);
            randomWarpsBag[t] = randomWarpsBag[randomIndex];
            randomWarpsBag[randomIndex] = temporary;
        }
    }

    protected virtual bool OnTriggerEnter(Collider other)
    {
        if (base.OnTriggerEnter(other))
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


}
