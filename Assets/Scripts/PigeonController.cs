using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonController : MonoBehaviour
{
    [HideInInspector]
    public CharacterController controller;

    Camera cam;

    Vector3 previousMoveDirection = new Vector3(.1f, .1f, .1f);

    [SerializeField]
    float moveSpeed = 2;

    [SerializeField]
    float averageSpeedMultiplier = .9f;

    [SerializeField]
    float maxMoveSpeed = 2;

    [SerializeField]
    float gravity = Physics.gravity.y;

    [SerializeField]
    float jumpHeight = -2f;

    float verticalVelocity = 0;

    [SerializeField]
    public float maxEnergy = 100;

    [SerializeField]
    public float currentEnergy = 100;

    [SerializeField]
    public float jumpEnergyCost = 20;

    [SerializeField]
    CharacterJoint[] myCharacterJoints;


    [SerializeField]
    Rigidbody[] myRigidbodies;




    [SerializeField]
    float respawnTime = 3f;
    float respawnTimer = 0f;



    public enum PigeonState
    { 
        Biting,
        Roaming,
        Respawning
    }

    [SerializeField]
    public PigeonState currentPigeonState = PigeonState.Roaming;

    [SerializeField]
    Animator animController;


    public delegate void WingFlap(float targetOpacity);
    public static event WingFlap onWingFlap;

    public delegate void Landed(float targetOpacity);
    public static event Landed onLanded;

    public Transform footLevel;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;

        UnRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10 || CheckIfWet())
        {
            Respawn();
        }


        switch (currentPigeonState)
        {
            case PigeonState.Roaming:
                Vector3 forwardDirection = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized;

                Vector3 rightDirection = Vector3.Scale(cam.transform.right, new Vector3(1, 0, 1)).normalized;

                Vector3 newMoveDirection = (forwardDirection * Input.GetAxisRaw("Vertical") + rightDirection * Input.GetAxisRaw("Horizontal")).normalized * moveSpeed;

                Vector3 averageMoveDirection = Vector3.Scale(newMoveDirection + previousMoveDirection, new Vector3(averageSpeedMultiplier, averageSpeedMultiplier, averageSpeedMultiplier));

                if (averageMoveDirection.magnitude > maxMoveSpeed)
                {
                    //Debug.Log("MAXC SPEED!");

                    averageMoveDirection = averageMoveDirection.normalized * maxMoveSpeed;

                }

                if (controller.isGrounded)
                {
                    animController.SetBool("GroundedJump", false);
                    animController.SetBool("FlappingPigeonJump", false);
                    animController.SetBool("Freefall", false);

                    //this first if statement is for animating the pigeon walking.
                    //Debug.Log(newMoveDirection.magnitude/moveSpeed);
                    if (newMoveDirection.magnitude/moveSpeed != 0f)
                    {
                        animController.SetBool("Walking", true);
                    }
                    else
                    {
                        animController.SetBool("Walking", false);
                    }

                    //These next if statements are for jumping and gravity!
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        //Debug.Log("Alleyoop!");
                        verticalVelocity = Mathf.Sqrt(2 * jumpHeight * gravity);

                        animController.SetBool("GroundedJump", true);

                        onWingFlap.Invoke(1);

                        
                    }
                    else
                    {
                        
                        verticalVelocity = -0.5f;
                    }
                }
                else
                {
                    //Not on the ground
                    //Debug.Log("Apparently We're not grounded!");

                    verticalVelocity += gravity * Time.deltaTime;


                    //Now we check for extra jumps. If the player has energy to spare, then we're set!

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (currentEnergy >= jumpEnergyCost)
                        {
                            //animController.SetBool("FlappingPigeonJump", true);
                            animController.Play("PigeonFlap");

                            currentEnergy -= jumpEnergyCost;

                            //Debug.Log("Alleyoop!");
                            verticalVelocity = Mathf.Sqrt(2 * jumpHeight * gravity);
                        }
                        else
                        {
                            //Debug.Log("We're out of jump juice!");

                            //Ragdoll(null);
                        }
                    }
                    else
                    {
                        if (verticalVelocity < -0.1f)
                        {
                            animController.SetBool("Freefall", true);
                        }

                    }
                }

                //Debug.Log(verticalVelocity);

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, Mathf.Atan2(averageMoveDirection.x, averageMoveDirection.z) * 180 / Mathf.PI, 0), .1f);

                controller.Move(averageMoveDirection * Time.deltaTime + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);

                previousMoveDirection = averageMoveDirection;


                break;

            case PigeonState.Biting:

                currentEnergy = Mathf.Min(currentEnergy + transform.parent.GetComponent<BitableObject>().nutritionalValue * Time.deltaTime, maxEnergy);

                if (Input.GetKeyDown(KeyCode.Space))
                {

                    animController.Play("PigeonFlap");

                    verticalVelocity = Mathf.Sqrt(2 * jumpHeight * gravity);

                    UnBite();
                }

                break;


            case PigeonState.Respawning:
                if (respawnTimer < respawnTime)
                {
                    //Debug.Log("WAIT");

                    respawnTimer += Time.deltaTime;

                }
                else
                {
                    respawnTimer = 0;

                    //Debug.Log("Welcome back, player!");



                    

                    transform.position = new Vector3(0, 1, 0);

                    controller.enabled = true;

                    //transform.GetChild(0).gameObject.SetActive(true);

                    verticalVelocity = 0;

                    previousMoveDirection = new Vector3(.1f, .1f, .1f);

                    currentPigeonState = PigeonState.Roaming;

                    //animController.Play("PigeonFlap");

                }


                
                break;
        }
        

    }


    public bool Bite(Transform thingToBite)
    {
        if (!controller.isGrounded && verticalVelocity < .1f && currentPigeonState != PigeonState.Biting)
        {
            currentPigeonState = PigeonState.Biting;

            transform.parent = thingToBite;

            Ragdoll(null);

            //controller.enabled = false;

            animController.SetBool("Freefall", false);

            return true;
        }

        return false;


    }

    public void UnBite()
    {
        currentPigeonState = PigeonState.Roaming;

        transform.parent = null;

        controller.enabled = true;

        UnRagdoll();
    }

    public void Ragdoll(Rigidbody target) {
        

        for (int i = 0; i < myRigidbodies.Length; i++)
        {
            Rigidbody currentRB = myRigidbodies[i];
            //Debug.Log(i);

            currentRB.isKinematic = false;
        }

        animController.enabled = false;

    }

    public void UnRagdoll()
    {
        for (int i = 0; i < myRigidbodies.Length; i++)
        {
            Rigidbody currentRB = myRigidbodies[i];
            //Debug.Log(i);

            currentRB.isKinematic = true;
        }


        animController.enabled = true;
    }


    public void Respawn() 
    {
        if (currentPigeonState != PigeonState.Respawning)
        {
            //Here we'll use feather particles to signifiy when the player gets hit and disappears;

            Debug.Log("Hit once!");

            

            //transform.GetChild(0).gameObject.SetActive(false);

            verticalVelocity = 0;

            previousMoveDirection = new Vector3(0, 1, 0);

            controller.enabled = false;

            
            UnBite();

            currentPigeonState = PigeonState.Respawning;

            


            transform.position += new Vector3(0, 800, 0);

            

        }

        
        
    
    }

    bool CheckIfWet()
    {
        if (footLevel.position.y < WaterManager.instance.GetWaveHeight(transform.position.x))
        {
            return true;
        }



        return false;
    }


}
