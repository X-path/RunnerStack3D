using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayButton : MonoBehaviour
{
    public void PlayButtonClick()
    {
        PlayerController.instance.playerState = PlayerController.PlayerState.Run;
        this.gameObject.SetActive(false);

    }
}
