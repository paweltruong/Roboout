using UnityEngine;


/// <summary>
/// tranforms object Transform to face the camera, useful for UI object in worldSpace
/// </summary>
public class BillboardUI : MonoBehaviour
{
    Camera camera;

    void Start()
    {
        if (Camera.main == null)
        {
            camera = FindObjectOfType<Camera>(true);
            if(camera == null)
                Debug.LogError("No main camera found");
        }
        else
            camera = Camera.main;

        camera = Camera.main;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }
}