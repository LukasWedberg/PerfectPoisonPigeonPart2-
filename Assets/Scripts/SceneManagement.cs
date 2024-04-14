using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lukas.Utilities
{

    public class SceneManagement : MonoBehaviour
    {
        public static SceneManagement instance;

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

        // Update is called once per frame
        public void SceneTransition(int sceneNumber)
        {
            SceneManager.LoadScene(sceneNumber);
        }
    }

}
