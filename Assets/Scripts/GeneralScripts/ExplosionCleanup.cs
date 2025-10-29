using UnityEngine;

public class ExplosionCleanup : MonoBehaviour
{
    void Start()
    {
        // Automatically destroy explosion after its animation finishes so no memory leaks occur.
        float length = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, length);
    }
}

