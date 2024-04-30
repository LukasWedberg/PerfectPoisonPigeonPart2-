using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    public float currentWaterLevel;

    public float targetWaterLevel;


    public static WaterManager instance;

    public float currentAmplitude = 1f;

    public float targetAmplitude = 0f;

    public float length = 2f;
    public float speed = 1f;
    public float offset = 0f;

    private void Awake()
    {
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

    // Start is called before the first frame update
    void Start()
    {
        currentWaterLevel = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        currentWaterLevel = Mathf.Lerp(currentWaterLevel, targetWaterLevel, 1 * Time.deltaTime);

        currentAmplitude = Mathf.Lerp(currentAmplitude, targetAmplitude, 1 * Time.deltaTime);

        offset += Time.deltaTime * speed; 
    }


    public float GetWaveHeight(float xPos)
    {
        return currentWaterLevel + (currentAmplitude * WaveHeightFunction((xPos / length) + offset));
    }

    private float WaveHeightFunction(float x)
    {
        return Mathf.Sin(x);
    }
}
