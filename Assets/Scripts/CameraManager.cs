using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Transform target = null;
    [SerializeField]
    MeshRenderer target1Renderer;
    
    
    [SerializeField]
    Transform target2 = null;
    [SerializeField]
    MeshRenderer target2Renderer;



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




    [SerializeField]
    float horizontalPixelBounds = 50;


    public float playerFocus = .5f;


    Quaternion targetRot;

    private Vector3 targetPoint;

    private Vector3 pointToLerpTo;


    [SerializeField]
    float forwardPositionLerpSpeed = .02f;
    
    [SerializeField]
    float backwardPositionLerpSpeed = .02f;


    float currentPositionLerpSpeed = .02f;


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
            //Camera Logic:
            //Rotate around the player towards a specific point. 
            //Stop rotating when both characters are in view, not necessarily when point is focused on.


            Bounds targetBounds = new Bounds(target.position, Vector3.zero);

            targetBounds.Encapsulate(target.position);
            targetBounds.Encapsulate(target2.position);

            targetPoint = targetBounds.center;

            float zoomAmount = Mathf.Max(targetBounds.size.x, targetBounds.size.y, targetBounds.size.z);

            //maxCameraDistance = zoomAmount + extraZoomDist;


            transform.position = new Vector3(transform.position.x, targetPoint.y + heightAbovePlayer, transform.position.z);
            
            Vector3 originDirection = targetPoint - transform.position;

            //targetRot = ;

            float currentCamHorizontalDist = Vector3.Distance(new Vector3(target.position.x,0,target.position.z) , new Vector3(transform.position.x, 0, transform.position.z));


            //Debug.Log(CheckBothCharactersVisible());


            if (!CheckBothCharactersVisible())
            {
                Debug.Log("I don't see you both!" + Time.realtimeSinceStartup);

                //Both characters aren't onscreen, so we'll rotate to find them by focusing on the center point

                targetRot = Quaternion.Slerp(Quaternion.LookRotation(targetPoint - transform.position), Quaternion.LookRotation(originDirection), playerFocus);

                
            }

            //targetRot = Quaternion.Slerp(Quaternion.LookRotation(target.position - transform.position), Quaternion.LookRotation(originDirection), playerFocus);


            //if (currentCamDist > maxCameraDistance)
            //{
            //transform.position = targetPoint + transform.forward * -maxCameraDistance;

            //transform.position = target.position + transform.forward * -Mathf.Lerp(20f, 30f, maxCameraDistance/40f);



            //}

            Debug.Log(currentCamHorizontalDist);

            
            if (currentCamHorizontalDist > maxCameraDistance)
            {

                pointToLerpTo = target.position + transform.forward * -maxCameraDistance;// * -Mathf.Lerp(20f, 30f, maxCameraDistance / 40f);


                
            }
            else if (currentCamHorizontalDist < minCameraDistance)
            {
                pointToLerpTo = target.position + transform.forward * -minCameraDistance;// * -Mathf.Lerp(20f, 30f, maxCameraDistance / 40f);

                

            }




            //transform.position = Vector3.Lerp(transform.position, pointToLerpTo, currentPositionLerpSpeed);

            transform.position = Vector3.Lerp(transform.position, pointToLerpTo, .02f);


            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, .02f);




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


    bool CheckBothCharactersVisible()
    {

        //First we check if the player is onscreen

        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(target.position);

        bool playerOnScreen = false;

        if (Mathf.Clamp(playerScreenPos.x, horizontalPixelBounds, Screen.width-horizontalPixelBounds) == playerScreenPos.x && Mathf.Clamp(playerScreenPos.y, 0, Screen.height) == playerScreenPos.y && target1Renderer.isVisible)
        {
            playerOnScreen = true;
        }

        //Next we need to see if the bnoss is onScreen

        //First we check if the player is onscreen

        Vector3 bossScreenPos = Camera.main.WorldToScreenPoint(target2.position);

        bool bossOnScreen = false;

        if (Mathf.Clamp(bossScreenPos.x, horizontalPixelBounds, Screen.width-horizontalPixelBounds) == bossScreenPos.x && Mathf.Clamp(bossScreenPos.y, 0, Screen.height) == bossScreenPos.y && target1Renderer.isVisible)
        {
            bossOnScreen = true;
        }


        if (bossOnScreen && playerOnScreen)
        {
            return true;
        }


        return false;
    }


    
}
