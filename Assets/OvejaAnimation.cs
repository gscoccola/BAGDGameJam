using UnityEngine;

public class OvejaAnimation : MonoBehaviour
{
    private Animator _aniamtor;
    
    
    private void Awake()
    {
        _aniamtor = GetComponent<Animator>();
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
