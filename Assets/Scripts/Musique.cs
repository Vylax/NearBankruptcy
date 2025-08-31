using UnityEngine;

/// <summary>
/// This script makes the GameObject it is attached to follow the main camera's position.
/// It also finds the attached AudioSource, sets it to loop, and plays it.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Musique : MonoBehaviour
{
    private AudioSource audioSource;
    private Transform cameraTransform;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        // Get the AudioSource component attached to this same GameObject.
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update.
    void Start()
    {
        // Find and store the main camera's transform for efficiency.
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            // Log an error if no camera is tagged as "MainCamera".
            Debug.LogError("MusicFollowCamera: Main Camera not found! Please tag your camera as 'MainCamera'.");
            return; // Stop the script if there's no camera to follow.
        }

        // Configure the audio to loop.
        audioSource.loop = true;

        // Play the audio clip from the beginning.
        audioSource.Play();
    }

    // LateUpdate is called once per frame, after all Update functions have been called.
    // This is the ideal place for camera-following logic.
    void LateUpdate()
    {
        // Match the camera's position every frame.
        if (cameraTransform != null)
        {
            transform.position = cameraTransform.position;
        }
    }
}