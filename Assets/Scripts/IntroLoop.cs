using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class IntroLoop : MonoBehaviour
{
    [SerializeField] public float loopStartSeconds;
    [SerializeField] public float loopEndSeconds;
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (source.time >= loopStartSeconds)
            source.time = loopEndSeconds;
    }
}
