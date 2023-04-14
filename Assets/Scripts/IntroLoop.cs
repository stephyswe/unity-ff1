using UnityEngine;
using UnityEngine.Serialization;

public class IntroLoop : MonoBehaviour {

	[FormerlySerializedAs("loop_start_seconds")]
	public float loopStartSeconds;
	[FormerlySerializedAs("loop_end_seconds")]
	public float loopEndSeconds;

	AudioSource source;

	// Start is called before the first frame update
	void Start() {
		source = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update() {
		if (source.time >= loopEndSeconds)
			source.time = loopStartSeconds;
	}
}
