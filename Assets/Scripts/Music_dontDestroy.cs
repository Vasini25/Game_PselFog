using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_dontDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
