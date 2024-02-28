using UnityEngine;

public class Carryable : MonoBehaviour
{
    [SerializeField] private Transform topPoint;
    [SerializeField] private float moveSpeed;

    private Rigidbody _rb;
    private Transform _attachPoint;

    public bool CanBeTaken => _attachPoint == null;
    public Rigidbody Rigidbody => _rb;
    public Transform TopPoint => topPoint;

    public void Take(Transform attachPoint)
    {
        _attachPoint = attachPoint;
    }
    
    public void Drop()
    {
        _attachPoint = null;
    }
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_attachPoint != null)
        {
            transform.position = Vector3.Lerp(transform.position, _attachPoint.position, 
                moveSpeed * Time.fixedDeltaTime);
        }
    }
}
