using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeScroller : MonoBehaviour {

	public Text infoTexttest;
	public RectTransform[] introImages;

	public Button MyWords,SightWords,Verbs,Nouns,Phonics;

	private float wide;

	private float mousePositionStartX;
	private float mousePositionEndX;
	private float dragAmount;
	private float screenPosition;
	private float lastScreenPosition;
	private float lerpTimer;
	private float lerpPage;

	public int pageCount = 1;
	public string side = "";

	public int swipeThrustHold = 100;
	public int spaceBetweenCards = 60;
	private bool canSwipe;

	Animator infoTextAnim;

	#region mono functions

	void Start() {

		//Time.timeScale = 1;
		infoTextAnim = infoTexttest.GetComponent<Animator>();
		infoTextAnim.enabled=false;

		wide = (introImages[0].GetComponent<RectTransform>().rect.width)-100;

		for(int i = 1; i < introImages.Length; i++){
			introImages[i].anchoredPosition = new Vector2(((wide+spaceBetweenCards)*i),0);
		}

		side = "right";

		SightWords.onClick.AddListener(delegate{buttonDownEvent("Sight Words");});
		MyWords.onClick.AddListener(delegate{buttonDownEvent("My Words");});
		Verbs.onClick.AddListener(delegate{buttonDownEvent("Verbs");});
		Nouns.onClick.AddListener(delegate{buttonDownEvent("Nouns");});
		Phonics.onClick.AddListener(delegate{buttonDownEvent("Phonics");});
		Application.targetFrameRate=60;
	}


	void Update() {

		lerpTimer=Time.smoothDeltaTime;
		if(lerpTimer<.333){
			screenPosition = Mathf.Lerp(lastScreenPosition ,lerpPage*-1 , lerpTimer*5f);
			lastScreenPosition=screenPosition;
		}

		if(Input.GetMouseButtonDown(0)) {
			canSwipe = true;
			mousePositionStartX = Input.mousePosition.x;
		}

		if(Input.GetMouseButton(0)) {
			if(canSwipe){
				mousePositionEndX = Input.mousePosition.x;
				dragAmount=mousePositionEndX-mousePositionStartX;
				screenPosition=lastScreenPosition+dragAmount;
			}
		}

		if(Mathf.Abs(dragAmount) > (swipeThrustHold) && canSwipe){
			canSwipe = false;
			lastScreenPosition=screenPosition;
			if(pageCount < introImages.Length )
				OnSwipeComplete () ;
			else if(pageCount == introImages.Length && dragAmount < 0)
				lerpTimer=0;
			else if(pageCount == introImages.Length && dragAmount > 0)
				OnSwipeComplete () ;
		}

		if(Input.GetMouseButtonUp(0)) {
			if(Mathf.Abs(dragAmount) < (swipeThrustHold)) {
				lerpTimer = 0;
			}
		}

		for(int i = 0; i < introImages.Length; i++){
			introImages[i].anchoredPosition = new Vector2(screenPosition+((wide+spaceBetweenCards)*i),0);
			//Debug.Log(introImages[i].anchoredPosition+"");

			if(side == "right") {
				if(i == pageCount-1) {
					introImages[i].localScale = Vector3.Lerp(introImages[i].localScale,new Vector3(1f,1f,1f),Time.smoothDeltaTime*5f);
					Color temp = introImages[i].GetComponent<Image>().color;
					introImages[i].GetComponent<Image>().color = new Color(temp.r,temp.g,temp.b,1);
				} else {
					introImages[i].localScale = Vector3.Lerp(introImages[i].localScale,new Vector3(0.9f,0.9f,0.9f),Time.smoothDeltaTime*2f);
					Color temp = introImages[i].GetComponent<Image>().color;
					introImages[i].GetComponent<Image>().color = new Color(temp.r,temp.g,temp.b,0.8f);
				}
			} else {
				if(i == pageCount) {
					introImages[i].localScale = Vector3.Lerp(introImages[i].localScale,new Vector3(1f,1f,1f),Time.smoothDeltaTime*5f);
					Color temp = introImages[i].GetComponent<Image>().color;
					introImages[i].GetComponent<Image>().color = new Color(temp.r,temp.g,temp.b,1);
				} else {
					introImages[i].localScale = Vector3.Lerp(introImages[i].localScale,new Vector3(0.9f,0.9f,0.9f),Time.smoothDeltaTime*2f);
					Color temp = introImages[i].GetComponent<Image>().color;
					introImages[i].GetComponent<Image>().color = new Color(temp.r,temp.g,temp.b,0.8f);
				}
			}
		}

		if (Application.platform == RuntimePlatform.Android) 
		{
         if (Input.GetKeyUp (KeyCode.Escape)) {
             Application.Quit ();
             return;
         }
     	}

	}

	public void buttonDownEvent (string type)
	{ 	
		switch (type) {

		case "My Words":
			//SceneManager.LoadScene("CommonScene");
			break;
		case "Sight Words":
            SceneManager.LoadScene("CommonScene", LoadSceneMode.Additive);
			break;
		case "Verbs":
			//SceneManager.LoadScene("CommonScene");
			break;
		case "Nouns":
			//SceneManager.LoadScene("CommonScene");
			break;
		case "Phonics":
			//SceneManager.LoadScene("CommonScene");
			break;
		case "Play":
			//SceneManager.LoadScene("CommonScene");
			break;
		default:
			break;
		}
	}

	#endregion

	private void OnSwipeComplete ()
	{
		lastScreenPosition = screenPosition;

		if (dragAmount > 0) {

			if (Mathf.Abs (dragAmount) > (swipeThrustHold)) {

				if (pageCount == 0) {
					lerpTimer = 0;
					lerpPage = 0;
				} else {
					if (side == "right")
						pageCount--;
					
					lerpTimer = 0;
					if (pageCount < 0)
						pageCount = 0;
					
					if (pageCount > 0) {
						UpdateText ("right");
						pageCount--;
					}

					side = "left";
					lerpPage = (wide + spaceBetweenCards) * pageCount;
				}

			} else {
				lerpTimer = 0;
			}

		} else if (dragAmount < 0 ) {
			
			if (Mathf.Abs (dragAmount) > (swipeThrustHold)) {

				if (pageCount == introImages.Length) {
					lerpTimer = 0;
					lerpPage = (wide + spaceBetweenCards) * introImages.Length - 1;
			
				} else {
					if(side == "left")
						pageCount++;
					side = "right";
					lerpTimer = 0;

					lerpPage = (wide + spaceBetweenCards) * pageCount;

					pageCount++;
					UpdateText ("left");

				}

			} else {
				lerpTimer=0;
			}
		}
	}

	private void UpdateText (string type)
	{
		StartCoroutine(StartTextScroll(type));
	}


	public IEnumerator StartTextScroll (string type)
	{
		infoTextAnim.enabled = true;
		infoTextAnim.Play ("moveUp");
		yield return new WaitForSeconds(0.75f);
		if(type.Equals("right"))
			infoTexttest.text= pageCount+1+"";
		else
			infoTexttest.text= ""+pageCount;
		infoTextAnim.Play ("comeFromDown");
    }

}
