using UnityEngine;

public class ChinchillaController : MonoBehaviour
{
    [SerializeField] Vector3 initialPosition;
    
    void Start()
    {
        InitPosition(initialPosition);
    }

    void InitPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
