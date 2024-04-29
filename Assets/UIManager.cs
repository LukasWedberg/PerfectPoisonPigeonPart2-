using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    PigeonController playerController;

    public Animator feathersAnimator;


    //When 0 no feathers will be displayed, when 1 display all feathers
    [SerializeField]
    float targetAnimProgress = 1f;
    public float currentAnimProgress = 0;



    [SerializeField]
    float targetOpacity = 1f;
    public float currentOpacity = 0f;

    [SerializeField]
    float fadingOrFallingModeTarget = 1f;
    public float currentFadingOrFallingMode = 0f;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("PPPlayer").GetComponent<PigeonController>();

        PigeonController.onWingFlap += wingFlapDelegate;

    }

    // Update is called once per frame
    void Update()
    {

        //First we have to adjust the transparency of the feathers to be visible only when the player is airborn.
        if (playerController.controller.isGrounded)
        {
            targetOpacity = 0;
        }
        else
        {
            targetOpacity = 1;
        }

        currentOpacity = Mathf.Lerp(currentOpacity, targetOpacity, 2.5f * Time.deltaTime);

        feathersAnimator.SetFloat("Transparency", currentOpacity);



        //This part is only for having a different animation based off of if the player is gaining or losing stamina

        currentFadingOrFallingMode = Mathf.Lerp(currentFadingOrFallingMode, fadingOrFallingModeTarget, 3 * Time.deltaTime);

        feathersAnimator.SetFloat("FeathersBlend", currentFadingOrFallingMode);




        //Finally, we also want to adjust the progress of the animation, so that we can watch the player regain or lose stamina in real time!

        targetAnimProgress = (playerController.currentEnergy - (playerController.currentEnergy % playerController.jumpEnergyCost)) / playerController.maxEnergy;


        currentAnimProgress = Mathf.Lerp(currentAnimProgress, targetAnimProgress, 2 * Time.deltaTime);

        feathersAnimator.SetFloat("AnimProgress", currentAnimProgress);

        

    }

    void wingFlapDelegate(float newTargetOpacity)
    {
        //targetOpacity = newTargetOpacity;

        Debug.Log("FLAPPY FLAPPY, MAKE DADDY HAPPY");
    }
}
