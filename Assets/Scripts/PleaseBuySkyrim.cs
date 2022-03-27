using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PleaseBuySkyrim : MonoBehaviour
{
    public bool TurnOn;

    private SpriteRenderer me;

    // Start is called before the first frame update
    void Start()
    {
        me = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!TurnOn)
            return;

        if (Input.GetKeyDown(KeyCode.W))
            me.enabled = true;
        if (Input.GetKeyUp(KeyCode.W))
            me.enabled = false;
    }
}
