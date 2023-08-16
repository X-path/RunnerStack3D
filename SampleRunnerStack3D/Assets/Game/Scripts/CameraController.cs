using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public static CameraController instance = null;
    public Transform Target;
    public Vector3 Offset;
    public float SmoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    bool isFinish = false;

    public Transform finishCamRotator;

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
    private void Start()
    {
        // Offset = camTransform.position - Target.position;
    }

    private void FixedUpdate()
    {
        if (isFinish)
            return;

        Vector3 targetPosition = Target.position + Offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);

    }
    public IEnumerator FinishCam()
    {
        isFinish = true;
        yield return new WaitForSeconds(.5f);
        this.transform.parent = finishCamRotator;


        finishCamRotator.DORotate(new Vector3(0f, 360f, 0f), 4, RotateMode.WorldAxisAdd)
        .SetEase(Ease.Linear).OnComplete(() =>
        {
            LevelCreator.instance.LevelFinish();
            isFinish = false;
            this.transform.parent = null;

        });


    }
}
