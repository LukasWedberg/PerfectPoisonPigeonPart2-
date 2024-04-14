using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StinkEyeEnemy : BitableObject
{
    enum EyeState {
        ChargingBlast,
        Aiming,
        Bitten
    }

    Transform player;

    Transform boss;
    Boss bossScript;

    EyeState currentEyeState = EyeState.Aiming;


    [SerializeField]
    float chargeTime = 1f;
    float chargeTimer = 0f;


    [SerializeField]
    float rapidFireTime = .1f;
    float rapidFireTimer;

    [SerializeField]
    float aimTime = .1f;
    float aimTimer = 0f;

    [SerializeField]
    float shakiness = 1f;


    [SerializeField]
    float posLerpSpeed = 3f;

    [SerializeField]
    int clipSize = 3;

    int shotsLeft;


    [SerializeField]
    GameObject[] bulletPrefabs;



    public Vector3 targetPos;



    protected override void Start()
    {
        base.Start();

        player = GameObject.Find("PPPlayer").transform;

        boss = GameObject.Find("Chefil").transform;

        bossScript = boss.GetComponent<Boss>();


        targetPos = transform.position;

        shotsLeft = clipSize;

        rapidFireTimer = rapidFireTime;

    }


    void Update(){ 
        
        switch (currentEyeState)
        {
            case EyeState.Aiming:

                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * posLerpSpeed);

                

                if (bossScript.currentBossState != Boss.BossState.vulnerable && bossScript.currentBossState != Boss.BossState.vulnerable && Vector3.Distance(player.position, transform.position) < 20 ) 
                {

                    transform.LookAt(player);


                    if (aimTimer < aimTime)
                    {
                        aimTimer += Time.deltaTime;
                    }
                    else
                    {
                        aimTimer = 0;

                        currentEyeState = EyeState.ChargingBlast;


                    }

                }   

                break;

            case EyeState.ChargingBlast:

                //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * posLerpSpeed);

                

                if (bossScript.currentBossState != Boss.BossState.vulnerable && bossScript.currentBossState != Boss.BossState.vulnerable && Vector3.Distance(player.position, transform.position) < 20)
                {
                    transform.LookAt(player);

                    if (chargeTimer < chargeTime)
                    {
                        chargeTimer += Time.deltaTime;

                        float currentShakiness = Mathf.Pow(1 - (chargeTimer / chargeTime), 2) * shakiness;

                        transform.position = targetPos + new Vector3(Random.Range(-1.01f, 1.01f) * currentShakiness, Random.Range(-1.01f, 1.01f) * currentShakiness, Random.Range(-1.01f, 1.01f) * currentShakiness);
                    }
                    else
                    {
                        if (rapidFireTimer < rapidFireTime)
                        {
                            rapidFireTimer += Time.deltaTime;


                        }
                        else
                        {

                            //Debug.Log("PEW PEW");

                            rapidFireTimer = 0;

                            shotsLeft--;

                            FireBullet(bulletPrefabs[0]);

                            if (shotsLeft <= 0)
                            {
                                currentEyeState = EyeState.Aiming;

                                rapidFireTimer = rapidFireTime;

                                chargeTimer = 0;

                                shotsLeft = clipSize;


                            }


                        }


                    }

                    

                }




                break;

            case EyeState.Bitten:
                //transform.position = targetPos + new Vector3(Random.Range(-1.01f, 1.01f) * shakiness, Random.Range(-1.01f, 1.01f) * shakiness, Random.Range(-1.01f, 1.01f) * shakiness);

                break;



        }


    
    }

    void LateUpdate()
    {
        if (currentEyeState == EyeState.Bitten)
        {
            transform.position = targetPos + new Vector3(Random.Range(-1.01f, 1.01f) * shakiness, Random.Range(-1.01f, 1.01f) * shakiness, Random.Range(-1.01f, 1.01f) * shakiness);

        }

    }

    protected override bool OnTriggerEnter(Collider other)
    {
        if (base.OnTriggerEnter(other))
        {
            currentEyeState = EyeState.Bitten;

            return true;

        }
        else
        {
            return false;
        }

        
    }

    protected virtual bool OnTriggerStay(Collider other)
    {
        //Debug.Log("STAYSTAY");

        if (base.OnTriggerStay(other))
        {
            currentEyeState = EyeState.Bitten;

            return true;

        }
        else
        {
            return false;
        }

        return false;
    }


    void OnTriggerExit(Collider other)
    {     
        if (other.CompareTag("Player"))
        {
            currentEyeState = EyeState.Aiming;

        }
    }



    void FireBullet(GameObject bulletPrefab)
    {
        GameObject newBullet = Instantiate(bulletPrefab);

        newBullet.transform.position = transform.position;

        newBullet.GetComponent<ProjectileBase>().Fire(transform.forward);

        


    }

}
