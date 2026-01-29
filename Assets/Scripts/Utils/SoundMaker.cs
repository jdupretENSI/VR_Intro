using UnityEngine;

public class SoundMaker : MonoBehaviour
{
    [SerializeField] private LayerMask _noiseMakingSurfaces;

    private void OnCollisionEnter(Collision other)
    {
        if ((_noiseMakingSurfaces & (1 << other.gameObject.layer)) > 0)
        {
            EventBus.Sound?.Invoke(this.transform);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool ContainsLayer(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}


