using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFXPooler : MonoBehaviour
{
    public GameObject PFXPrefab;

    private List<ParticleSystem> Pool = new List<ParticleSystem>();

    private void Start() 
    {
        SpawnItem();
        SpawnItem();
        SpawnItem();
    }


    private void SpawnItem()
    {
        var obj = Instantiate(PFXPrefab, transform);
        Pool.Add(obj.GetComponent<ParticleSystem>());
        obj.SetActive(false);
    }

    public void PlayPFX(Vector3 position)
    {
        bool played = false;
        foreach(ParticleSystem ps in Pool)
        {
            if(ps.isPlaying) continue;
            played = true;
            ps.gameObject.SetActive(true);
            ps.transform.position = position;
            ps.Play();
            StartCoroutine(DelayedDeActivate(ps));
            return;
        }

        if(!played)
        {
            SpawnItem();
            var ps = Pool[Pool.Count -1];
            ps.gameObject.SetActive(true);
            ps.transform.position = position;
            StartCoroutine(DelayedDeActivate(ps));
            ps.Play();
        }
    }

    private IEnumerator DelayedDeActivate(ParticleSystem ps)
    {
        yield return new WaitForSeconds(ps.main.duration);
        ps.gameObject.SetActive(false);
    }



}
