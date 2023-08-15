using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    [SerializeField] Button playButton;
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    public void PlayButtonClick()
    {
        PlayerController.instance.playerState = PlayerController.PlayerState.Run;
        playButton.gameObject.SetActive(false);
        LevelCreator.instance.CubeMoveStart();

    }
    public void PlayButtonActiveted()
    {
        playButton.gameObject.SetActive(true);
    }
}
