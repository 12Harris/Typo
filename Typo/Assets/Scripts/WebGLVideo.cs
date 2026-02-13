using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.IO;

public class WebGLVideo : MonoBehaviour
{
    private VideoPlayer vp = null;

    void Start()
    {
        vp = GetComponent<VideoPlayer>();

#if UNITY_WEBGL && !UNITY_EDITOR
        vp.url = Application.streamingAssetsPath + "/video-background.mp4";
#else
        vp.url = Path.Combine(Application.streamingAssetsPath, "video-background.mp4");
#endif

        vp.audioOutputMode = VideoAudioOutputMode.AudioSource;
        //vp.GetTargetAudioSource(0).mute = false;
        vp.Pause();

    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        if (vp == null)
            return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            vp.Play();
        }
    }
}