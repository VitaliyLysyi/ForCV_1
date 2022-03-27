using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    public string SceneName = "EnterNameOfYourSceneHere";

    private CircleCollider2D Body;

    // Start is called before the first frame update
    void Start()
    {
        Body = GetComponent<CircleCollider2D>();
        if (SceneName == "EnterNameOfYourSceneHere")
            Debug.Log("Введи назву сцени в параметрах обєкту тормоз!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && Input.GetAxisRaw("Jump") != 0)
        {
            SceneManager.LoadScene(SceneName);
        }
            
    }
}
