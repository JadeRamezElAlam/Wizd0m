
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EndScreen : MonoBehaviour
{

    public PostProcessProfile blurProfile;
    public PostProcessProfile normalProfile;
    public PostProcessVolume postProcessVolume;

    // Switch between the blurProfile and normalProfile applied during startscreen
    public void EnableCameraBlur(bool state)
    {
        if (postProcessVolume != null && blurProfile != null && normalProfile != null)
        {
            postProcessVolume.profile = (state) ? blurProfile : normalProfile;
        }
    }
}
