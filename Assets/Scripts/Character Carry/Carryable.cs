using UnityEngine;
using UnityEngine.UIElements;

public class Carryable : MonoBehaviour
{
    [SerializeField] private Transform topPoint;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dropForce;

    private float _anchoringValue;
    private Rigidbody _rb;
    private Collider _collider;
    private Transform _attachPoint;

    public bool CanBeTaken => _attachPoint == null;
    public Rigidbody Rigidbody => _rb;
    public Transform TopPoint => topPoint;

    public void Take(Transform attachPoint, float anchoringValue)
    {
        _anchoringValue = anchoringValue;
        _attachPoint = attachPoint;
        _rb.isKinematic = true;
        _collider.enabled = false;
    }
    
    public void Drop()
    {
        _attachPoint = null;
        _rb.isKinematic = false;
        _collider.enabled = true;

        Vector3 force = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)) * Vector3.forward;
        force.y = 1f;
        force *= dropForce;
        _rb.AddForce(force, ForceMode.VelocityChange);
    }
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        if (_attachPoint != null)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.Euler(transform.rotation.eulerAngles.Y()), moveSpeed * Time.fixedDeltaTime);
            transform.forward = Vector3.Lerp(transform.forward, _attachPoint.forward, moveSpeed * Time.fixedDeltaTime);

            Vector3 startPosition = Vector3.Lerp(_attachPoint.position, transform.position, _anchoringValue);
            transform.position = Vector3.Lerp(startPosition, _attachPoint.position, 
                moveSpeed * Time.fixedDeltaTime);
        }
    }
}
