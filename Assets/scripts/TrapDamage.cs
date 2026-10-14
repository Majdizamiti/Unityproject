using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] bool instantKill = false;
    [SerializeField] bool autoAddCollider = true;                  // creates colliders if the obstacle has none
    [SerializeField, Range(0.3f, 1f)] float colliderSize = 0.8f;   // 1 = full mesh size, lower = more forgiving

    public int Damage => instantKill ? 9999 : damage;

    void Awake()
    {
        Collider[] cols = GetComponentsInChildren<Collider>();

        if (cols.Length == 0 && autoAddCollider)
        {
            AddTriggerColliders();
            cols = GetComponentsInChildren<Collider>();
        }

        Debug.Log("TrapDamage is on: " + name + " | colliders: " + cols.Length);
    }

    void AddTriggerColliders()
    {
        // One trigger box per mesh, in the mesh's own space: follows animation and rotation
        foreach (MeshFilter mf in GetComponentsInChildren<MeshFilter>())
        {
            if (mf.sharedMesh == null) continue;

            BoxCollider box = mf.gameObject.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.center = mf.sharedMesh.bounds.center;
            box.size = mf.sharedMesh.bounds.size * colliderSize;
        }

        // Needed so this script on the root receives the trigger events of its children
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trap trigger: " + name + " touched by " + other.name);
        Hit(other);
    }

    void OnTriggerStay(Collider other) { Hit(other); }

    void Hit(Collider other)
    {
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health != null) health.TakeDamage(Damage);
    }
}