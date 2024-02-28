using UnityEngine;

public class Carryable : MonoBehaviour
{
    [SerializeField] private Transform topPoint;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dropForce;

    private Rigidbody _rb;
    private Transform _attachPoint;

    public bool CanBeTaken => _attachPoint == null;
    public Rigidbody Rigidbody => _rb;
    public Transform TopPoint => topPoint;

    public void Take(Transform attachPoint)
    {
        _rb.isKinematic = true;
        _attachPoint = attachPoint;
    }
    
    public void Drop()
    {
        _attachPoint = null;
        _rb.isKinematic = false;

        Vector3 force = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)) * Vector3.forward;
        force.y = 1f;
        force *= dropForce;
        _rb.AddForce(force, ForceMode.VelocityChange);
    }
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_attachPoint != null)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.Euler(transform.rotation.eulerAngles.Y()), moveSpeed * Time.fixedDeltaTime);
            transform.position = Vector3.Lerp(transform.position, _attachPoint.position, 
                moveSpeed * Time.fixedDeltaTime);
        }
    }
}
