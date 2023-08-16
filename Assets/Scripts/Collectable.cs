using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private GameObject _skin;
    [SerializeField] private GameObject _effect;

    public void Interaction()
    {
        _skin.SetActive(false);
        _effect.SetActive(true);
        
        Destroy(gameObject, 1.0f);
    }
}
