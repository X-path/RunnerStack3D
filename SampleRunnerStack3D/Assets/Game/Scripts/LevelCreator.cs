using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelCreator : MonoBehaviour
{
    public static LevelCreator instance = null;
    [SerializeField] GameObject cubePrefab;
    [SerializeField] GameObject finishCube;
    [SerializeField] GameObject starPrefab;
    int nextRow = 0;
    [SerializeField] int levelListCount = 0;
    [SerializeField] List<int> levelList = new List<int>();
    float firstCubePositionZ = 0;
    float cubeZScale = 2.5f;
    float nextCubePositionZ = 0;
    [SerializeField] List<GameObject> cubeList = new List<GameObject>();
    [SerializeField] List<Material> cubeColors = new List<Material>();
    [SerializeField] float _xPosition;
    GameObject currentCube = null;
    public GameObject lastCube = null;
    [SerializeField] GameObject standCube;
    List<GameObject> cubesPool = new List<GameObject>();
    [SerializeField] int cubesPoolCount = 0;
    float lastFinishPosZ = 0;
    List<GameObject> cutCubes = new List<GameObject>();
    List<GameObject> finishList = new List<GameObject>();

    AudioSource audioSource;
    bool isPerfectTime = false;
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
        audioSource = GetComponent<AudioSource>();

        firstCubePositionZ = standCube.transform.position.z;
        nextCubePositionZ = firstCubePositionZ;
        lastCube = standCube;
        CubePoolCreator();
        FinishCreator();
        CubeCreator();

    }

    void FinishCreator()
    {
        FinishDestroy();

        float _finishCubeZPosition = firstCubePositionZ + cubeZScale + (levelList[levelListCount] * cubeZScale);
        GameObject _go = Instantiate(finishCube, new Vector3(0, 0.01f, (_finishCubeZPosition - finishCube.transform.localScale.z)), cubePrefab.transform.rotation);
        lastFinishPosZ = _go.transform.position.z;
        finishList.Add(_go);
    }
    void FinishDestroy()
    {
        if (finishList.Count > 1)
        {
            GameObject _finish = finishList[0];
            finishList.RemoveAt(0);
            Destroy(_finish);
        }
    }
    void CubeCreator()
    {

        for (int i = 0; i < levelList[levelListCount]; i++)
        {
            if (i + 1 % 2 == 1)
            {
                _xPosition = +_xPosition;
            }
            else
            {
                _xPosition = -_xPosition;
            }

            GameObject _go = cubesPool[i]; //Instantiate(cubePrefab, new Vector3(_xPosition, -0.5f, (nextCubePositionZ + cubeZScale)), cubePrefab.transform.rotation);
            _go.transform.position = new Vector3(_xPosition, -0.5f, (nextCubePositionZ + cubeZScale));
            _go.GetComponent<MeshRenderer>().material = ColorCreator();

            GameObject _star = StarCreator();
            if (_star != null)
                _star.transform.position = new Vector3(0, 0, _go.transform.position.z);

            nextCubePositionZ += cubeZScale;
            cubeList.Add(_go);

        }



    }
    GameObject StarCreator()
    {
        int _rndm = Random.Range(0, 2);
        if (_rndm == 1)
        {
            GameObject _go = Instantiate(starPrefab);
            return _go;
        }
        return null;
    }
    Material ColorCreator()
    {
        return cubeColors[Random.Range(0, cubeColors.Count - 1)];
    }

    public void CutControl()
    {
        if (currentCube != null)
        {
            DOTween.Kill(currentCube.transform);
        }

        CutCube();
    }
    public void CubeMoveStart()
    {
        if (nextRow < cubeList.Count)
        {

            CubeMove(cubeList[nextRow], lastCube.transform.localScale.x);
            nextRow++;
        }
    }
    void CubeMove(GameObject _cube, float _xScale)
    {

        currentCube = _cube;
        _cube.transform.localScale = new Vector3(_xScale, _cube.transform.localScale.y, _cube.transform.localScale.z);
        _cube.SetActive(true);
        float _startXPos = _cube.transform.position.x;
        _cube.transform.DOMoveX(((-1) * _startXPos), PlayerController.instance.speed * 3f).SetEase(Ease.Flash);
    }

    void CubePoolCreator()
    {
        for (int i = 0; i < cubesPoolCount; i++)
        {
            GameObject _go = Instantiate(cubePrefab);
            cubesPool.Add(_go);

            _go.SetActive(false);
        }
    }
    void CubePoolResetAll()
    {
        cubeList.Clear();
        nextRow = 0;
        for (int i = 0; i < cubesPool.Count; i++)
        {
            cubesPool[i].SetActive(false);
            cubesPool[i].transform.localScale = standCube.transform.localScale;
            Destroy(cubesPool[i].GetComponent<Rigidbody>());
        }

        for (int i = cutCubes.Count - 1; i > -1; i--)
        {
            GameObject _go = cutCubes[i];
            cutCubes.RemoveAt(i);
            Destroy(_go);
        }
    }

    #region Cut //This cube cutting part is taken from the github account in the link and edited.  https://github.com/neziruran/GameGuruCaseStudy/blob/main/Assets/_Project/Task_2/Scripts/Runtime/Gameplay/Level/LevelWizard.cs
    void CutCube()
    {

        GameObject cutBlock = Instantiate(cubePrefab);
        cutCubes.Add(cutBlock);
        cutBlock.GetComponent<MeshRenderer>().material = currentCube.GetComponent<MeshRenderer>().material;

        cutBlock.name = "CutPart";
        Vector3 targetPos = lastCube.transform.position;
        Vector3 blockPos = currentCube.transform.position;
        Vector3 originalScale = currentCube.transform.localScale;
        cutBlock.transform.position = new Vector3((targetPos.x + blockPos.x) / 2f, blockPos.y, blockPos.z);
        cutBlock.transform.localScale = new Vector3(currentCube.transform.localScale.x - Mathf.Abs(lastCube.transform.position.x - currentCube.transform.position.x), currentCube.transform.localScale.y, currentCube.transform.localScale.z);

        Sound(cutBlock);

        var factor = SetFactor(blockPos, targetPos);
        SetCurrentBlock(targetPos, blockPos, originalScale, factor, cutBlock);
        DisableRigidbody(currentCube);

        bool failThreshold = cutBlock.transform.localScale.x > .05f;
        if (!failThreshold)
        {

            cutBlock.transform.position = blockPos;
            cutBlock.transform.localScale = lastCube.transform.localScale;
            DisableRigidbody(cutBlock);

        }
        else
        {
            lastCube = cutBlock;
            CubeMoveStart();
        }

    }
    private void SetCurrentBlock(Vector3 targetPos, Vector3 blockPos, Vector3 originalScale, float factor,
           GameObject cutBlock)
    {
        currentCube.transform.position = new Vector3((targetPos.x + blockPos.x) / 2f + originalScale.x * factor / 2f,
            blockPos.y, blockPos.z);
        currentCube.transform.localScale =
            new Vector3(lastCube.transform.localScale.x - cutBlock.transform.localScale.x,
                lastCube.transform.localScale.y, lastCube.transform.localScale.z);
    }
    private void DisableRigidbody(GameObject cutBlock)
    {
        if (cutBlock.GetComponent<Rigidbody>()) return;

        cutBlock.AddComponent<Rigidbody>().mass = 100f;
        cutBlock.GetComponent<Collider>().enabled = false;
    }
    private static float SetFactor(Vector3 blockPos, Vector3 targetPos)
    {

        float factor = 0;

        if (blockPos.x - targetPos.x > 0)
        {
            factor = 1f;
        }
        else
        {
            factor = -1f;
        }
        return factor;
    }

    public void Sound(GameObject _cutBlock)
    {
        isPerfectTime = _cutBlock.transform.localScale.x / currentCube.transform.localScale.x > .9f;
        if (isPerfectTime)
        {
            audioSource.pitch += audioSource.pitch * .1f;
        }
        else
        {
            audioSource.pitch = 0.5f;

        }

        audioSource.Stop();
        audioSource.Play();
    }
    #endregion
    public void LevelFinish()
    {
        Debug.Log("LevelFinish");

        if (levelListCount < levelList.Count - 1)
        {
            levelListCount++;
        }
        else
        {
            levelListCount = 0;
        }

        CubePoolResetAll();
        standCube.transform.position = new Vector3(standCube.transform.position.x, standCube.transform.position.y, lastFinishPosZ + 2.15f);
        firstCubePositionZ = standCube.transform.position.z;
        nextCubePositionZ = firstCubePositionZ;
        lastCube = standCube;
        CubeCreator();
        FinishCreator();
        PlayerController.instance.PlayerReset(standCube.transform.position.z);
        UIManager.instance.PlayButtonActiveted();
        audioSource.pitch = 0.5f;

    }

}
