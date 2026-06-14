using UnityEngine;

public class PipeRotate : MonoBehaviour
{
    [Header("Sound Effects")]
    public AudioClip rotateSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) // Right Mouse Button
        {
            transform.Rotate(0f, 0f, 90f);
            PlaySound(rotateSound); // Play rotate sound
        }
    }
}