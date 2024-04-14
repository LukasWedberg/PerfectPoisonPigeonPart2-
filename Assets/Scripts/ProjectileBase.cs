using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]

public class ProjectileBase : MonoBehaviour
{
    bool inMotion = false;

    public float speed = 1f;

    public float acceleration = .5f;

    public float destroyDelay = 5f;

    Vector3 moveDirection;

    BoxCollider hitBox = null;

    protected virtual void Start()
    {
        hitBox = GetComponent<BoxCollider>();


        Fire(transform.forward);
    }


    protected virtual void Update() 
    { 
        if (inMotion)
        {
            speed += acceleration * Time.deltaTime;

            transform.position += moveDirection * speed * Time.deltaTime;




        }
    
    
    }

    public virtual void Fire(Vector3 direction)
    {
        moveDirection = direction.normalized;

        inMotion = true;


        Invoke("SelfDestruct", destroyDelay);

        transform.rotation = Quaternion.LookRotation(direction);

    }

    protected virtual void SelfDestruct()
    {
        Destroy(gameObject);
    }



    protected virtual void OnTriggerEnter(Collider other)
    {
        //Behavior for hurting the player here!

        if (!other.CompareTag("Eye"))
        {
            if (other.CompareTag("Player"))
            {
                PigeonController playerController = other.GetComponent<PigeonController>();

                if (playerController.currentPigeonState != PigeonController.PigeonState.Respawning)
                {
                    Debug.Log("Respawn player");

                    playerController.Respawn();
                }

                


            }
            else if (other.CompareTag("Boss"))
            {

                Debug.Log("Damage boss!");

                other.GetComponent<Boss>().GetTired();
            }

            SelfDestruct();

        }

    }




}
