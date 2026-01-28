using System;
using UnityEngine;

public class SoundMaker : MonoBehaviour
{
    [SerializeField] private LayerMask _noiseMakingSurfaces;

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.layer.CompareTo(_noiseMakingSurfaces.value));

        if (LayerMask.LayerToName(other.gameObject.layer)  == _noiseMakingSurfaces.ToString())
        {
            EventBus.Sound?.Invoke(this.transform);
        }
    }
    public static bool ContainsLayer(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}


