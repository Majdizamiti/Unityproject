using System.Collections.Generic;
using UnityEngine;

public class AudioDetective : MonoBehaviour
{
    readonly HashSet<AudioSource> reported = new HashSet<AudioSource>();

    void Update()
    {
        if (Time.timeSinceLevelLoad > 3f) return;   // only watches the first 3 seconds

        foreach (AudioSource src in FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (src.isPlaying && reported.Add(src))
            {
                Debug.Log("AUDIO PLAYING at t=" + Time.timeSinceLevelLoad.ToString("F2") +
                          " | object = " + GetPath(src.transform) +
                          " | clip = " + (src.clip != null ? src.clip.name : "(none, PlayOneShot)") +
                          " | playOnAwake = " + src.playOnAwake +
                          " | loop = " + src.loop, src);
            }
        }
    }

    static string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null) { t = t.parent; path = t.name + "/" + path; }
        return path;
    }
}