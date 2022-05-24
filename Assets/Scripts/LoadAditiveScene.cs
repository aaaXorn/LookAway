using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadAditiveScene : MonoBehaviour
{
    public string SceneName;
    AsyncOperation asyncOperation;
    bool sceneloaded = false;

    //collider do load additive scene
    private SphereCollider col;
    private CapsuleCollider caps_col;

    [SerializeField]
    private bool lower_fps = false, capsule = false;
    private float radius_col, height_col;

    // Start is called before the first frame update
    void Start()
    {
        if(lower_fps)
        {
            if (!capsule)
            {
                //collider e raio do collider
                col = GetComponent<SphereCollider>();
                radius_col = col.radius;
            }
            else
            {
                //collider, raio e altura do collider
                caps_col = GetComponent<CapsuleCollider>();
                radius_col = caps_col.radius;
                height_col = caps_col.height;
            }
        }
    }

    // Update is called once per frame
    /*void Update()
    {
        if(asyncOperation != null)
        {
           LoadIcon.instance.LoadIconRun(asyncOperation.progress+0.1f);
            if (asyncOperation.isDone&&LoadIcon.instance)
            {
                LoadIcon.instance.LoadIconRun(0);
                asyncOperation = null;
            }
        }
    }*/
    
    private IEnumerator LoadAdd()
    {
        while (asyncOperation != null)
        {
            LoadIcon.instance.LoadIconRun(asyncOperation.progress + 0.1f);
            if (asyncOperation.isDone && LoadIcon.instance)
            {
                LoadIcon.instance.LoadIconRun(0);
                asyncOperation = null;

                StopCoroutine("LoadAdd");
            }

            yield return new WaitForEndOfFrame();
        }

        if(asyncOperation == null) StopCoroutine("LoadAdd");
        yield return new WaitForEndOfFrame();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !sceneloaded)
        {
            //diminui o raio do col se necessário
            if(lower_fps)
            {
                if (!capsule)
                {
                    if (col.radius >= radius_col)
                    {
                        //FPS
                        float current_frame = (1f / Time.unscaledDeltaTime);
                        
                        if (current_frame < 28f)
                        {
                            col.radius = radius_col / 2;

                            return;
                        }
                    }
                }
                else
                {
                    if(caps_col.radius >= radius_col)
                    {
                        //FPS
                        float current_frame = (1f / Time.unscaledDeltaTime);

                        if (current_frame < 28f)
                        {
                            caps_col.radius = radius_col / 2;
                            caps_col.height = height_col / 2;

                            return;
                        }
                    }
                }
            }

            sceneloaded = true;
            asyncOperation = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            StartCoroutine("LoadAdd");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && sceneloaded)
        {
            //aumenta o raio do col se necessário
            if (lower_fps)
            {
                if (!capsule)
                {
                    if (col.radius < radius_col)
                    {
                        //FPS
                        float current_frame = (1f / Time.unscaledDeltaTime);

                        if (current_frame >= 28f)
                        {
                            col.radius = radius_col;

                            return;
                        }
                    }
                }
                else
                {
                    if (caps_col.radius < radius_col)
                    {
                        //FPS
                        float current_frame = (1f / Time.unscaledDeltaTime);

                        if (current_frame >= 28f)
                        {
                            caps_col.radius = radius_col;
                            caps_col.height = height_col;

                            return;
                        }
                    }
                }
            }

            sceneloaded = false;
            SceneManager.UnloadSceneAsync(SceneName);
        }
    }
}
