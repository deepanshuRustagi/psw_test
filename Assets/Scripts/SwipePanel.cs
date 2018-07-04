using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class SwipePanel : MonoBehaviour {
	public GameObject[] Cards;
	public GameObject[] CardText;
	public GameObject[] CardNumber;
	public GameObject[] ShowSentenceBulb;
	public List<string> CurrentWordList;
	string[] words={"the","is","a","we","has","was","there","PREMIUM"};
	string[] wordSentences={"PREMIUM","the grapes are over there","the tiger was sleeping","he has a pair of binoculars","can we go on the boat","i saw a dog","the clock is green","i can catch the ball"};
	public Button backButton,audioToggle;
	public Sprite close,bulb,audioOff,audioOn;
	public static GameObject audioObject;
	public GameObject WordAudioPublic;
	int CurrentCardCounter = 0;

	Animator swipeAnim,backAnim;

	private bool isCardShowingSentence=false;
	private bool isDragging= false;
	private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
	private Vector2 startTouch, swipeDelta;

	public Transform panel;
	public Transform BackPanel;
	public GameObject CardPrefab;

	Vector3[] Positions;
	Vector3[] CardScales;
	Vector3 InActivePositions;
	Vector3 InActiveCardScales;
    Vector3 ImagePosition;
	string originalWord="",originalNumber="";

	private void Reset ()
	{
		startTouch=swipeDelta=Vector2.zero;
		isDragging=false;
	}

	void Start () {
		audioObject=WordAudioPublic;
		CurrentWordList.Clear();

		if(CurrentWordList.Count<1){
			CurrentWordList= new List<string>(CurrentWordList);

		}
		CurrentWordList.AddRange(words);

		SetCardStack();

		backButton.onClick.AddListener(onBackPress);

		backAnim = BackPanel.GetComponent<Animator>();
		audioToggle.onClick.AddListener(FireAudio);
		audioToggle.GetComponent<Image> ().sprite = audioOff;

		if (Application.platform == RuntimePlatform.Android) 
		{
         if (Input.GetKeyUp (KeyCode.Escape)) {
			ShowSentenceOnBulbClick();
         }
     	}
		
	}

	public int CompareObNames (GameObject x, GameObject y)
	{
		return y.name.CompareTo(x.name);
	}

	public void SetCardStack ()
	{
		Positions = new Vector3[4];
		CardScales = new Vector3[4];
		Positions [0] = new Vector3 (Screen.width / 2, (Screen.height / 2) + 135f, 0f);
		Positions [1] = new Vector3 (Screen.width / 2, (Screen.height / 2) + 90f, 0f);
		Positions [2] = new Vector3 (Screen.width / 2, (Screen.height / 2) + 45f, 0f);
		Positions [3] = new Vector3 (Screen.width / 2, Screen.height / 2, 0f);

        ImagePosition = new Vector3 (Screen.width / 2, Screen.height / 2 + -10f, 0f);

       

		CardScales [0] = new Vector3 (0.85f, 0.85f, 0f);
		CardScales [1] = new Vector3 (0.9f, 0.9f, 0f);
		CardScales [2] = new Vector3 (0.95f, 0.95f, 0f);
		CardScales [3] = new Vector3 (1f, 1f, 0f);

		InActivePositions = new Vector3 (0f, 0f, 0f);
		InActiveCardScales = new Vector3 (1f, 1f, 0f);

		Cards = new GameObject[words.Length];
		CardNumber = new GameObject[words.Length];
		CardText = new GameObject[words.Length];
		ShowSentenceBulb = new GameObject[words.Length];
		for (int i = 0, j = words.Length; i <= words.Length - 1; i++,j--) {
			GameObject obj = (GameObject)Instantiate (CardPrefab) as GameObject;
			Cards [i] = obj;
			CurrentCardCounter++;

			Cards [i].transform.SetParent (panel.transform, false);
			foreach (Transform child in obj.transform) {
				if (child.tag == "TextCardNumber")
					CardNumber [i] = child.gameObject;
				if (child.tag == "TextCard")
					CardText [i] = child.gameObject;
				if (child.tag == "ShowSentenceBulb")
					ShowSentenceBulb [i] = child.gameObject;
			}
			if(i==0)
				ShowSentenceBulb [i].gameObject.SetActive(false);
			if(i<7)
				ShowSentenceBulb [i].transform.GetChild(2).gameObject.SetActive(false);
			if (i == 0) {
				CardText [i].GetComponent<Text> ().text = CurrentWordList [j - 1];
				CardText [i].GetComponent<Text> ().fontSize = 70;
				CardText [i].GetComponent<Text> ().color = new Color (255, 215, 0);
			} else
				CardText [i].GetComponent<Text> ().text = CurrentWordList [j - 1];
			CardNumber [i].GetComponent<Text> ().text = j + "";
			ShowSentenceBulb [i].GetComponent<Button> ().onClick.AddListener (ShowSentenceOnBulbClick);
		}
		StartCoroutine(ShakeThatBulbInitially());

		var quotient = CurrentCardCounter / 4;

		for (int k = 3; CurrentCardCounter > 0; CurrentCardCounter--,k--) {
			if (CurrentCardCounter <= (4 * (quotient - 1))) {
				Cards [CurrentCardCounter - 1].SetActive (false);
			}
			else {
                Debug.LogError("Card counter in Stack: " + (CurrentCardCounter-1));
				Cards [CurrentCardCounter-1].transform.position = Positions [k];
				Cards [CurrentCardCounter-1].transform.localScale = CardScales [k];
				Cards [CurrentCardCounter-1].SetActive (true);
			}

		}
		CurrentCardCounter=words.Length-1;
		swipeAnim= Cards[CurrentCardCounter].GetComponent<Animator>();
	}

	public void ShowSentenceOnBulbClick ()
	{	
		if (CurrentCardCounter > 0) {
			swipeAnim.enabled = true;
			if (isCardShowingSentence) {
				swipeAnim.Play ("showCardFace");
				ShowSentenceBulb [CurrentCardCounter].transform.GetChild(2).gameObject.SetActive(false);
				
				CardText [CurrentCardCounter].GetComponent<Text> ().fontSize = 159;
                //CardText[CurrentCardCounter].GetComponent<Text>().color = new Color(0,0,0);
				//CardText[CurrentCardCounter].GetComponent<Text>().color = new Color(89,156,218);
				CardText [CurrentCardCounter].GetComponent<Text> ().text = originalWord;
                CardText[CurrentCardCounter].transform.localScale = new Vector3(1, 1, 1);
                ShowSentenceBulb [CurrentCardCounter].transform.GetChild (1).gameObject.GetComponent<Image> ().sprite = bulb;
				ShowSentenceBulb [CurrentCardCounter].transform.GetChild (1).gameObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (60f, 80f);
				CardNumber [CurrentCardCounter].GetComponent<Text> ().text = originalNumber;
				isCardShowingSentence = false;
			} else {
				originalNumber = CardNumber [CurrentCardCounter].GetComponent<Text> ().text;
				originalWord = CardText [CurrentCardCounter].GetComponent<Text> ().text;
				swipeAnim.Play ("showSentence");
				isCardShowingSentence = true;
				ShowSentenceBulb [CurrentCardCounter].transform.GetChild(2).gameObject.SetActive(false);
				int startColoredWordIndex = wordSentences [CurrentCardCounter].IndexOf (originalWord);
				int endColoredWordIndex = startColoredWordIndex+originalWord.Length;
				CardText [CurrentCardCounter].GetComponent<Text> ().color = new Color (0, 0, 0);
				string[] sentenceArray= wordSentences[CurrentCardCounter].Split(new[] {" "+originalWord+" "}, System.StringSplitOptions.None);
				CardText [CurrentCardCounter].GetComponent<Text> ().text= sentenceArray[0]+"<color=#599dda>"+" "+originalWord+" "+"</color>"+sentenceArray[1];
				CardText [CurrentCardCounter].transform.localScale = new Vector3 (-1, 1, 1);
				CardText [CurrentCardCounter].GetComponent<Text> ().fontSize = 60;
				CardNumber [CurrentCardCounter].GetComponent<Text> ().text = "";
				ShowSentenceBulb [CurrentCardCounter].transform.GetChild(1).gameObject.GetComponent<Image> ().sprite = close;
				ShowSentenceBulb [CurrentCardCounter].transform.GetChild(1).gameObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (60f, 60f);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{	
		tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

		#region Standalone Inputs
		if (Input.GetMouseButtonDown (0)) {
			tap = true;
			isDragging = true;
			startTouch = Input.mousePosition;
		} else if (Input.GetMouseButtonDown (0)) {
			isDragging = false;
			Reset ();
		}
		#endregion

		#region Mobile Inputs
		if (Input.touches.Length != 0) {
			if (Input.touches [0].phase == TouchPhase.Began) {
				tap = true;
				isDragging = true;
				startTouch = Input.touches [0].position;
			} else if (Input.touches [0].phase == TouchPhase.Ended || Input.touches [0].phase == TouchPhase.Canceled) {
				isDragging = false;
				Reset ();
			}

		}
		#endregion

		// Calculate the swipe distance
		swipeDelta = Vector2.zero;
		if (isDragging) {
			if (Input.touches.Length > 0)
				swipeDelta = Input.touches [0].position - startTouch;
			else if (Input.GetMouseButton (0))
				swipeDelta = (Vector2)Input.mousePosition - startTouch;
		}

		// Did we cross the swipe Threshold

		if (swipeDelta.magnitude > 125) {
			float x = swipeDelta.x;
			float y = swipeDelta.y;
			if (Mathf.Abs (x) > Mathf.Abs (y)) {
				//left or right
				if (x > y) {
					swipeRight = true;
					swipeLeft = false;
				} else {
					swipeLeft = true;
					swipeRight = false;
				}
			} else {
				//Up or down
				//do nothing
			}
			Reset ();
		}

		if (swipeLeft && CurrentCardCounter >= 0 && !isCardShowingSentence) {
			if(CurrentCardCounter==0)
					audioToggle.transform.gameObject.SetActive(false);
			if (CurrentCardCounter > 0) {
				swipeAnim.enabled = true;
				swipeAnim.Play ("swipe");
				CurrentCardCounter--;
				swipeAnim = Cards [CurrentCardCounter].GetComponent<Animator> ();
               
				if (CurrentCardCounter > 3) {
                    
					Cards [CurrentCardCounter].transform.localScale = CardScales [3];
                    Cards [CurrentCardCounter].transform.position = Positions[3];
					Cards [CurrentCardCounter - 1].transform.localScale = CardScales [2];
					Cards [CurrentCardCounter - 1].transform.position = Positions [2];
					Cards [CurrentCardCounter - 2].transform.localScale = CardScales [1];
					Cards [CurrentCardCounter - 2].transform.position = Positions [1];
					Cards [CurrentCardCounter - 3].SetActive (true);
					Cards [CurrentCardCounter - 3].transform.localScale = CardScales [0];
					Cards [CurrentCardCounter - 3].transform.position = Positions [0];

				} else if (CurrentCardCounter > 2) {
                    
					Cards [CurrentCardCounter].transform.localScale = CardScales [3];
					Cards [CurrentCardCounter].transform.position = Positions [3];
					Cards [CurrentCardCounter - 1].transform.localScale = CardScales [2];
					Cards [CurrentCardCounter - 1].transform.position = Positions [2];
					Cards [CurrentCardCounter - 2].transform.localScale = CardScales [1];
					Cards [CurrentCardCounter - 2].transform.position = Positions [1];
					Cards [CurrentCardCounter - 3].SetActive (true);
					Cards [CurrentCardCounter - 3].transform.localScale = CardScales [0];
					Cards [CurrentCardCounter - 3].transform.position = Positions [0];

				} else if (CurrentCardCounter > 1) {
                    
					Cards [CurrentCardCounter].transform.localScale = CardScales [3];
					Cards [CurrentCardCounter].transform.position = Positions [3];
					Cards [CurrentCardCounter - 1].transform.localScale = CardScales [2];
					Cards [CurrentCardCounter - 1].transform.position = Positions [2];
					Cards [CurrentCardCounter - 2].transform.localScale = CardScales [1];
					Cards [CurrentCardCounter - 2].transform.position = Positions [1];

				} else if (CurrentCardCounter > 0) {
                    
					Cards [CurrentCardCounter].transform.localScale = CardScales [3];
					Cards [CurrentCardCounter].transform.position = Positions [3];
					Cards [CurrentCardCounter - 1].transform.localScale = CardScales [2];
					Cards [CurrentCardCounter - 1].transform.position = Positions [2];
				}
			} else {
				swipeAnim = Cards [0].GetComponent<Animator> ();
				swipeAnim.enabled = true;
				swipeAnim.Play ("shake");
				StartCoroutine(WaitAfterShakeToStop());
			}

		}

		if (swipeRight && CurrentCardCounter < words.Length - 1  && !isCardShowingSentence) {
			audioToggle.transform.gameObject.SetActive(true);
			if (CurrentCardCounter < words.Length) {
				swipeAnim = Cards [CurrentCardCounter+1].GetComponent<Animator> ();
				swipeAnim.enabled = true;
				swipeAnim.Play ("swipeBack");
				CurrentCardCounter++;
			}
            Debug.LogError("Swipe Right Card Counter: " + CurrentCardCounter);
			if (CurrentCardCounter > 6) {

                Cards[CurrentCardCounter].transform.localScale = CardScales[3];
                Cards[CurrentCardCounter].transform.position = Positions[3];
                Cards[CurrentCardCounter - 1].transform.localScale = CardScales[2];
                Cards[CurrentCardCounter - 1].transform.position = Positions[2];
                Cards[CurrentCardCounter - 2].transform.localScale = CardScales[1];
                Cards[CurrentCardCounter - 2].transform.position = Positions[1];
                Cards[CurrentCardCounter - 3].transform.localScale = CardScales[0];
                Cards[CurrentCardCounter - 3].transform.position = Positions[0];

				Cards [CurrentCardCounter - 4].SetActive (false);
				Cards [CurrentCardCounter - 4].transform.localScale = InActiveCardScales;
				Cards [CurrentCardCounter - 4].transform.position = InActivePositions;

			}else if (CurrentCardCounter > 5) {
               
                Cards[CurrentCardCounter].transform.localScale = CardScales[3];
                Cards[CurrentCardCounter].transform.position = Positions[3];
                Cards[CurrentCardCounter - 1].transform.localScale = CardScales[2];
                Cards[CurrentCardCounter - 1].transform.position = Positions[2];
                Cards[CurrentCardCounter - 2].transform.localScale = CardScales[1];
                Cards[CurrentCardCounter - 2].transform.position = Positions[1];
                Cards[CurrentCardCounter - 3].transform.localScale = CardScales[0];
                Cards[CurrentCardCounter - 3].transform.position = Positions[0];

				Cards [CurrentCardCounter - 4].SetActive (false);
				Cards [CurrentCardCounter - 4].transform.localScale = InActiveCardScales;
				Cards [CurrentCardCounter - 4].transform.position = InActivePositions;

			}else if (CurrentCardCounter > 4) {
                
                Cards[CurrentCardCounter].transform.localScale = CardScales[3];
                Cards[CurrentCardCounter].transform.position = Positions[3];
                Cards[CurrentCardCounter - 1].transform.localScale = CardScales[2];
                Cards[CurrentCardCounter - 1].transform.position = Positions[2];
                Cards[CurrentCardCounter - 2].transform.localScale = CardScales[1];
                Cards[CurrentCardCounter - 2].transform.position = Positions[1];
                Cards[CurrentCardCounter - 3].transform.localScale = CardScales[0];
                Cards[CurrentCardCounter - 3].transform.position = Positions[0];

				Cards [CurrentCardCounter - 4].SetActive (false);
				Cards [CurrentCardCounter - 4].transform.localScale = InActiveCardScales;
				Cards [CurrentCardCounter - 4].transform.position = InActivePositions;

			} else if (CurrentCardCounter > 3) {
                
                Cards[CurrentCardCounter].transform.localScale = CardScales[3];
                Cards[CurrentCardCounter].transform.position = Positions[3];
                Cards[CurrentCardCounter - 1].transform.localScale = CardScales[2];
                Cards[CurrentCardCounter - 1].transform.position = Positions[2];
                Cards[CurrentCardCounter - 2].transform.localScale = CardScales[1];
                Cards[CurrentCardCounter - 2].transform.position = Positions[1];
                Cards[CurrentCardCounter - 3].transform.localScale = CardScales[0];
                Cards[CurrentCardCounter - 3].transform.position = Positions[0];

				Cards [CurrentCardCounter - 4].SetActive (false);
				Cards [CurrentCardCounter - 4].transform.localScale = InActiveCardScales;
				Cards [CurrentCardCounter - 4].transform.position = InActivePositions;

			} else if (CurrentCardCounter > 2) {


                Cards[CurrentCardCounter].transform.localScale = CardScales[3];
                Cards[CurrentCardCounter].transform.position = Positions[3];

				Cards [CurrentCardCounter - 1].transform.localScale = CardScales[2];
				Cards [CurrentCardCounter - 1].transform.position = Positions[2];

				Cards [CurrentCardCounter - 2].transform.localScale = CardScales[1];
				Cards [CurrentCardCounter - 2].transform.position = Positions[1];

				Cards [CurrentCardCounter - 3].transform.localScale = CardScales[0];
				Cards [CurrentCardCounter - 3].transform.position = Positions[0];

			} else if (CurrentCardCounter > 1) {

                Cards[CurrentCardCounter].transform.localScale = CardScales[3];
                Cards[CurrentCardCounter].transform.position = Positions[3];

				Cards [CurrentCardCounter - 1].transform.localScale = CardScales[2];
				Cards [CurrentCardCounter - 1].transform.position = Positions[2];

				Cards [CurrentCardCounter - 2].transform.localScale = CardScales[1];
				Cards [CurrentCardCounter - 2].transform.position = Positions[1];

               

			} else if (CurrentCardCounter > 0 ) {

                Cards[CurrentCardCounter].transform.localScale = CardScales[3];
                Cards[CurrentCardCounter].transform.position = Positions[3];
                
				Cards [CurrentCardCounter - 1].transform.localScale = CardScales[2];
				Cards [CurrentCardCounter - 1].transform.position = Positions[2];

               
			}
		}
	}

	IEnumerator ShakeThatBulbInitially ()
	{
		swipeAnim= Cards[7].GetComponent<Animator>();
		swipeAnim.enabled = true;
		swipeAnim.Play("GlowBulb");
		yield return new WaitForSeconds(3f);
		swipeAnim.enabled = false;
	}

	IEnumerator WaitAfterShakeToStop ()
	{
		yield return new WaitForSeconds(1f);
		swipeAnim.enabled = false;
	}

	private void onBackPress ()
	{	
		audioToggle.transform.gameObject.SetActive(false);
		backAnim.enabled=true;
		backAnim.Play("backButtonSlide");
		StartCoroutine(WaitRoutine());
	}

	IEnumerator WaitRoutine ()
	{
		yield return new WaitForSeconds(1f);
		SceneManager.UnloadScene("CommonScene");
	}

	public void MoveForwards (GameObject Card)
	{
		var speed = 5;
		Card.transform.position = Card.transform.position - new Vector3(0f,30f,0f);

	}
	
	public void FireAudio ()
	{
		//Debug.Log();
		if (isCardShowingSentence) {
			if (!audioObject.GetComponent<AudioSource> ().isPlaying) {
				string word = originalWord;

				AudioClip wordaudio = GetWordSentenceClip (word);

				audioObject.GetComponent<AudioSource> ().clip = wordaudio;
				StartCoroutine(WaitTillAudioIsPlaying());
			}

		} else {
			if (!audioObject.GetComponent<AudioSource> ().isPlaying) {
				string word = CardText [CurrentCardCounter].GetComponent<Text> ().text;

				AudioClip wordaudio = GetWordAudioClip (word);
			
				audioObject.GetComponent<AudioSource> ().clip = wordaudio;
				StartCoroutine(WaitTillAudioIsPlaying());
			}
		}
	}

	IEnumerator WaitTillAudioIsPlaying ()
	{
		audioToggle.GetComponent<Image> ().sprite = audioOn;
		audioObject.GetComponent<AudioSource> ().Play ();
		yield return new WaitUntil(()=> audioObject.GetComponent<AudioSource> ().isPlaying == false);
		audioToggle.GetComponent<Image> ().sprite = audioOff;
	}

	public static AudioClip GetWordAudioClip(string word){
		
		if(word !=""){
			if (word == ("it's")) {
				word = "its"; 
				return audioObject.GetComponent<Audio> ().WordAudioList.Single (x => string.Equals (x.name, word, System.StringComparison.CurrentCultureIgnoreCase));
			} else if(word == ("don’t")){
				word = "dont"; 
				return audioObject.GetComponent<Audio> ().WordAudioList.Single (x => string.Equals (x.name, word, System.StringComparison.CurrentCultureIgnoreCase));
			} else{
				return audioObject.GetComponent<Audio> ().WordAudioList.Single (x => string.Equals (x.name, word, System.StringComparison.CurrentCultureIgnoreCase));
			}
		}
		return null;
	}

	public static AudioClip GetWordSentenceClip(string word)
    {
        if (word != "")
        {
            word = word+"_sentence";
			return audioObject.GetComponent<Audio>().SenAudioList.Single(x => string.Equals(x.name, word, System.StringComparison.CurrentCultureIgnoreCase));
        }
        return null;
    }

    public void shakeObject (GameObject lastCard)
	{
		
	}

}

