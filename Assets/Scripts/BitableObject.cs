using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]

public class BitableObject : MonoBehaviour
{
    


    BoxCollider hitBox = null;


    [SerializeField]
    public float nutritionalValue = 10f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        hitBox = GetComponent<BoxCollider>();
    }

    protected virtual bool OnTriggerEnter( Collider other)
    {
        if (hitBox != null)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log(other.gameObject.name);

                PigeonController pigeonScript = other.GetComponent<PigeonController>();

                
                return pigeonScript.Bite(transform);
            }

            return false;
        }

        return false;
    }

    protected virtual bool OnTriggerStay(Collider other)
    {
        //Debug.Log("STAYSTAY");

        if (hitBox != null)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log(other.gameObject.name);

                PigeonController pigeonScript = other.GetComponent<PigeonController>();


                return pigeonScript.Bite(transform);
            }

            return false;
        }

        return false;
    }





}
