using UnityEngine;

public class gemcontrol : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 90f;      // degrees per second
    [SerializeField] AudioSource gemCollect;       // optional: its clip is used if collectClip is empty
    [SerializeField] AudioClip collectClip;
    [SerializeField, Range(0f, 1f)] float volume = 1f;
    [SerializeField] string playerTag = "Player";

    [SerializeField] int gemScore = 100;

    Renderer gemRenderer;
    bool collected;

    void Awake()
    {
        gemRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        if (collected || gemRenderer == null) return;
        Vector3 center = gemRenderer.bounds.center;
        transform.RotateAround(center, Vector3.up, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        bool isPlayer = other.CompareTag(playerTag)
                        || other.GetComponentInParent<CharacterController>() != null;
        if (!isPlayer) return;

        collected = true;

        ScoreControl.totalScore += gemScore;   // CHANGED: add the gem's points to the score

        AudioClip clip = collectClip != null ? collectClip
                       : (gemCollect != null ? gemCollect.clip : null);

        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, transform.position, volume);
        else
            Debug.LogWarning("gemcontrol: no audio clip assigned on " + name);

        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;

        Destroy(gameObject);
    }
}