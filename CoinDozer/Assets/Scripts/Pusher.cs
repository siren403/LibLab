using LitMotion;
using LitMotion.Animation;
using UnityEngine;

public class Pusher : MonoBehaviour
{
    public LitMotionAnimation motion;

    private MotionHandle _playingHandle = MotionHandle.None;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_playingHandle.IsPlaying())
            {
                return;
            }
            _playingHandle = motion.Components[Random.Range(0, motion.Components.Count)].Play();
        }
    }
}
