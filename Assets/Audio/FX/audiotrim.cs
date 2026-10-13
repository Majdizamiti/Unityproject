using UnityEngine;

public class audiotrim : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] float startTime = 0.5f;     // seconds to skip at the start
    [SerializeField] float duration = 1.0f;      // seconds to play
    [SerializeField] bool playOnStart = false;   // play automatically when the scene starts

    void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (playOnStart) PlayPart();
    }

    public void PlayPart()
    {
        if (audioSource == null || audioSource.clip == null)
        {
            Debug.LogWarning("audiotrim: no AudioSource or no clip assigned on " + name);
            return;
        }

        CancelInvoke(nameof(StopPart));                          // avoid overlapping stops
        audioSource.time = Mathf.Clamp(startTime, 0f, audioSource.clip.length - 0.01f);
        audioSource.Play();
        Invoke(nameof(StopPart), duration);
    }

    void StopPart()
    {
        audioSource.Stop();
    }
}