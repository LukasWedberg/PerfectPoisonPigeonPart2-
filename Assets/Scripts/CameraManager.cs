using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Transform target = null;
    
    
    [SerializeField]
    Transform target2 = null;

    

    PigeonController playerScript;

    [SerializeField]
    float heightAbovePlayer = 2;

    [SerializeField]
    float maxCameraDistance = 10;

    [SerializeField]
    float minCameraDistance = 2;


    [SerializeField]
    float minFollowDistance = 2;

    [SerializeField]
    float extraZoomDist = 10;


    public float playerFocus = .5f;


    Quaternion targetRot;

    private Vector3 targetPoint;


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
            Bounds targetBounds = new Bounds(target.position, Vector3.zero);

            targetBounds.Encapsulate(target.position);
            targetBounds.Encapsulate(target2.position);

            targetPoint = targetBounds.center;

            float zoomAmount = Mathf.Max(targetBounds.size.x, targetBounds.size.y, targetBounds.size.z);

            maxCameraDistance = zoomAmount + extraZoomDist;


            transform.position = new Vector3(transform.position.x, targetPoint.y + heightAbovePlayer, transform.position.z);
            
            Vector3 originDirection = targetPoint - transform.position;

            //targetRot = ;

            float currentCamDist = Vector3.Distance(targetPoint, transform.position);


            targetRot = Quaternion.Slerp(Quaternion.LookRotation(target.position - transform.position), Quaternion.LookRotation(originDirection), playerFocus);


            //if (currentCamDist > maxCameraDistance)
            //{
            //transform.position = targetPoint + transform.forward * -maxCameraDistance;

            transform.position = target.position + transform.forward * -Mathf.Lerp(20f, 30f, maxCameraDistance/40f);
            //}

            //we want it to chase the target point--this means if its too far away, we go after it!
            //but we still want to zoom to keep an eye on characters!


            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, .3f);




            /*
           


            if (!(currentCamDist < minCameraDistance))
            {
                targetRot = Quaternion.LookRotation(originDirection);

                




                if (currentCamDist > maxCameraDistance)
                {

                    transform.position = targetPoint + transform.forward * -maxCameraDistance;

                }
            }
            else
            {
                //Debug.Log("GETTIBNG CLOSE");

                transform.position = targetPoint + Vector3.Scale(transform.forward, new Vector3(1, 1, 1)).normalized * -minCameraDistance;


            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, .3f);

            */

        }






    }
}
