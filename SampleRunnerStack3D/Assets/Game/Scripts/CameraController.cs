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
        yield return new WaitForSeconds(2f);

        LevelCreator.instance.LevelFinish();
        isFinish = false;
        /*  transform.DORotate(new Vector3(0f, 360f, 0f), 4, RotateMode.WorldAxisAdd)
              .SetLoops(-1, LoopType.Incremental) // Sonsuz döngü için
              .SetEase(Ease.Linear)               // Dönme hızını düzgün hale getirme
              .OnUpdate(() => {
                 // transform.LookAt(targetPosition); // Her güncellemede hedefe bak
              });
      */
    }
}
