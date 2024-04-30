using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private GameObject skin;
    [SerializeField] private GameObject effect;

    public void Interaction()
    {
        skin.SetActive(false);
        effect.SetActive(true);
        
        Destroy(gameObject, 1.0f);
    }
}