using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && PlayerController.instance.playerState == PlayerController.PlayerState.Run)
        {
            LevelCreator.instance.CutControl();
        }
    }
}
