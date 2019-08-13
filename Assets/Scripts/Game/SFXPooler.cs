using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPooler : MonoBehaviour
{
    public GameObject SFXPrefab;

    private List<AudioSource> Pool = new List<AudioSource>();

    private void Start() 
    {
        SpawnItem();
        SpawnItem();
        SpawnItem();
    }


    private void SpawnItem()
    {
        var obj = Instantiate(SFXPrefab, transform);
        Pool.Add(obj.GetComponent<AudioSource>());
        obj.SetActive(false);
    }

    public void PlaySFX(int value)
    {
        bool played = false;
        foreach(AudioSource source in Pool)
        {
            if(source.isPlaying) continue;
            played = true;
            source.gameObject.SetActive(true);
            source.pitch = 1 + value * 0.001f;
            source.Play();
            StartCoroutine(DelayedDeActivate(source));
            return;
        }

        if(!played)
        {
            SpawnItem();
            var source = Pool[Pool.Count -1];
            source.gameObject.SetActive(true);
            source.pitch = 1 + value * 0.001f;
            source.Play();
            StartCoroutine(DelayedDeActivate(source));
        }
    }

    private IEnumerator DelayedDeActivate(AudioSource source)
    {
        yield return new WaitForSeconds(2f);
        source.gameObject.SetActive(false);
    }



}
