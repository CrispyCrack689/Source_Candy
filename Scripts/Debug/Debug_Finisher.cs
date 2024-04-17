using UnityEngine;
using UnityEngine.Playables;

public class Debug_Finisher : MonoBehaviour
{
    public PlayableDirector PlayableDirector;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            PlayableDirector.Play();
        }
    }
}
