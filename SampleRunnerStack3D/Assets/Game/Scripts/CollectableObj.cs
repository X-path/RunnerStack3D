using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObj : MonoBehaviour
{
    public IEnumerator ParticleActiveted()
    {
        transform.GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }
}
