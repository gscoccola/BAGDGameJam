using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class OvejaAnimation : MonoBehaviour
{
    public bool retrasarAnimation = false;
    private Animator _aniamtor;
    
    
    private void Awake()
    {
        _aniamtor = GetComponent<Animator>();
        if (retrasarAnimation) return;
        StartCoroutine(Coroutine());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator Coroutine()
    {
        float random = Random.Range(0f, 5f);
        _aniamtor.StopPlayback();
        yield return new WaitForSeconds(random);
        _aniamtor.StartPlayback();
    }
    

    public void StartWalkAnimation()
    {
        _aniamtor.SetBool("Walk", true);
    }
    
    public void EndWalkAnimation()
    {
        _aniamtor.SetBool("Walk", false);
    }
}
