using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    Transform target = null;

    PigeonController playerScript;

    [SerializeField]
    float heightAbovePlayer = 2;

    [SerializeField]
    float maxCameraDistance = 10;

    [SerializeField]
    float minCameraDistance = 2;


    Quaternion targetRot;


    // Start is called before the first frame update
    void Start()
    {
        if (target == null) {
            target = GameObject.Find("PPPlayer").transform;

            playerScript = target.GetComponent<PigeonController>();
        }

    }

    // Update is called once per frame
    void LateUpdate()
    {
        

        
        //Debug.Log(playerScript.currentPigeonState == PigeonController.PigeonState.Respawning);
        //Debug.Log(playerScript.currentPigeonState == PigeonController.PigeonState.Respawning);

        

        if (playerScript.currentPigeonState != PigeonController.PigeonState.Respawning)
        {
            transform.position = new Vector3(transform.position.x, target.position.y + heightAbovePlayer, transform.position.z);

            Vector3 originDirection = target.position - transform.position;

            float currentCamDist = Vector3.Distance(target.position, transform.position);


            if (!(currentCamDist < minCameraDistance))
            {
                targetRot = Quaternion.LookRotation(originDirection);

                




                if (currentCamDist > maxCameraDistance)
                {

                    transform.position = target.position + transform.forward * -maxCameraDistance;

                }
            }
            else
            {
                //Debug.Log("GETTIBNG CLOSE");

                transform.position = target.position + Vector3.Scale(transform.forward, new Vector3(1, 1, 1)).normalized * -minCameraDistance;


            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, .3f);

        }






    }
}
