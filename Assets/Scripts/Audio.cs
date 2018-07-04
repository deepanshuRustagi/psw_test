using UnityEngine;
using System.Collections;

public class Audio : MonoBehaviour {

	// Use this for initialization

	
	public AudioClip [] WordAudioList;
	public AudioClip[] SenAudioList;
   
    void Start () {

        
        this.GetComponent<AudioSource>().Play();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
