using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;

    [SerializeField]
    AudioClip[] sounds;

    AudioSource myAudioSource;

    private void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();

        //First time?
        if (instance == null)
        {
            //Assign instance
            instance = this;
            //Don't destroy this gameObject through different scene
            DontDestroyOnLoad(this);

        }
        else
        {
            //Destroy self if there is duplicate
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(int soundIndex)
    {

        myAudioSource.PlayOneShot(sounds[soundIndex]);


    }

}
