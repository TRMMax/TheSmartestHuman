using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour {

	[SerializeField] private TextAsset answersFile;
	
	[SerializeField] private TMP_InputField inputName1;
	[SerializeField] private TMP_InputField inputName2;
	[SerializeField] private TMP_InputField inputName3;

	[SerializeField] private TMP_InputField inputTimer1;
	[SerializeField] private TMP_InputField inputTimer2;
	[SerializeField] private TMP_InputField inputTimer3;

	[SerializeField] private Button startButton;

	[SerializeField] private GameObject overlay;

	[SerializeField] private TMP_Text textTimer1;
	[SerializeField] private TMP_Text textTimer2;
	[SerializeField] private TMP_Text textTimer3;

	[SerializeField] private List<TMP_Text> textAnswers;
	
	[SerializeField] private int correctAnswerTimeLoss = 20;

	[SerializeField] private Texture2D inactiveTimer;
	[SerializeField] private Texture2D activeTimer;

	[SerializeField] private RawImage timerBackground1;
	[SerializeField] private RawImage timerBackground2;
	[SerializeField] private RawImage timerBackground3;

	[SerializeField] private TMP_Text textName1;
	[SerializeField] private TMP_Text textName2;
	[SerializeField] private TMP_Text textName3;

	[SerializeField] private TMP_Text questionText;
	
	private List<AnswerSet> answers;

	private float timer1;
	private float timer2;
	private float timer3;

	private bool running = false;
	private bool paused = true;
	private int turn = 1;

	private int question = 0;

	private bool[] showAnswers = new bool[5];

	private void Start() {
		// Screen.SetResolution(1280, 720, true);
		
		answers = new List<AnswerSet>();
		
		string[] questions = answersFile.text.Split('\n');
		foreach (string qText in questions) {
			string[] q = qText.Split(',');
			AnswerSet answerSet = new AnswerSet {
				question = q[0],
				answer1 = q[1],
				answer2 = q[2],
				answer3 = q[3],
				answer4 = q[4],
				answer5 = q[5]
			};
			
			answers.Add(answerSet);
		}
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) paused = !paused;
		if (Input.GetKeyDown(KeyCode.RightArrow)) NextTurn();
		if (Input.GetKeyDown(KeyCode.LeftArrow)) PreviousTurn();
		if (Input.GetKeyDown(KeyCode.Return)) NextQuestion();
		if (Input.GetKeyDown(KeyCode.Alpha1)) CorrectAnswer(1);
		if (Input.GetKeyDown(KeyCode.Alpha2)) CorrectAnswer(2);
		if (Input.GetKeyDown(KeyCode.Alpha3)) CorrectAnswer(3);
		if (Input.GetKeyDown(KeyCode.Alpha4)) CorrectAnswer(4);
		if (Input.GetKeyDown(KeyCode.Alpha5)) CorrectAnswer(5);

		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			if (turn == 1) timer1--;
			if (turn == 2) timer2--;
			if (turn == 3) timer3--;
		}
		
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			if (turn == 1) timer1++;
			if (turn == 2) timer2++;
			if (turn == 3) timer3++;
		}

		if (running && !paused) {
			float delta = Time.deltaTime;

			if (turn == 1) timer1 -= delta;
			if (turn == 2) timer2 -= delta;
			if (turn == 3) timer3 -= delta;
			
			if (timer1 <= 0 || timer2 <= 0 || timer3 <= 0) {
				if (timer1 <= 0) textTimer1.color = Color.red;
				if (timer2 <= 0) textTimer2.color = Color.red;
				if (timer3 <= 0) textTimer3.color = Color.red;
			}
		}
		
		timer1 = Math.Max(timer1, 0);
		timer2 = Math.Max(timer2, 0);
		timer3 = Math.Max(timer3, 0);

		textTimer1.text = Math.Ceiling(timer1).ToString(CultureInfo.InvariantCulture);
		textTimer2.text = Math.Ceiling(timer2).ToString(CultureInfo.InvariantCulture);
		textTimer3.text = Math.Ceiling(timer3).ToString(CultureInfo.InvariantCulture);
	}

	public void StartTimers() {
		bool parsed = true;
		parsed &= float.TryParse(inputTimer1.text, out timer1);
		parsed &= float.TryParse(inputTimer2.text, out timer2);
		parsed &= float.TryParse(inputTimer3.text, out timer3);

		if (!parsed) return;

		textName1.text = inputName1.text;
		textName2.text = inputName2.text;
		textName3.text = inputName3.text;

		timerBackground1.texture = activeTimer;
		timerBackground2.texture = inactiveTimer;
		timerBackground3.texture = inactiveTimer;
		
		question = 0;
		questionText.text = "";
		
		for (int i = 0; i < 5; i++) {
			showAnswers[i] = false;
			textAnswers[i].gameObject.SetActive(false);
		}
		
		overlay.SetActive(false);
		
		running = true;
	}

	private void ChangeTurn(int num) {
		if (num >= 4) num = 1;
		if (num <= 0) num = 3;

		turn = num;
		
		timerBackground1.texture = inactiveTimer;
		timerBackground2.texture = inactiveTimer;
		timerBackground3.texture = inactiveTimer;


		if (turn == 1) timerBackground1.texture = activeTimer;
		if (turn == 2) timerBackground2.texture = activeTimer;
		if (turn == 3) timerBackground3.texture = activeTimer;
	}

	private void NextTurn() {
		ChangeTurn(turn + 1);
	}
	
	private void PreviousTurn() {
		ChangeTurn(turn - 1);
	}
	
	private void NextQuestion() {
		question++;
		for (int i = 0; i < 5; i++) {
			showAnswers[i] = false;
			textAnswers[i].gameObject.SetActive(false);
			if (question <= answers.Count) {
				textAnswers[i].text = answers[question - 1].GetAnswer(i + 1);
			} else {
				textAnswers[i].text = "";
			}
		}

		if (question <= answers.Count) {
			questionText.text = answers[question - 1].question;
		} else {
			questionText.text = "";
		}

		if ((timer1 < timer2 || timer2 <= 0) && (timer1 < timer3 || timer3 <= 0) && timer1 > 0) {
			ChangeTurn(1);
		} else if ((timer2 < timer3 || timer3 <= 0) && timer2 > 0) {
			ChangeTurn(2);
		} else {
			ChangeTurn(3);
		}
	}

	private void CorrectAnswer(int num) {
		if (showAnswers[num - 1]) return;
		showAnswers[num - 1] = true;
		textAnswers[num - 1].gameObject.SetActive(true);
		
		switch (turn) {
			case 1:
				timer2 -= correctAnswerTimeLoss;
				timer3 -= correctAnswerTimeLoss;
				break;
			case 2:
				timer1 -= correctAnswerTimeLoss;
				timer3 -= correctAnswerTimeLoss;
				break;
			case 3:
				timer1 -= correctAnswerTimeLoss;
				timer2 -= correctAnswerTimeLoss;
				break;
		}
	}

	[Serializable]
	private class AnswerSet {
		[SerializeField] public string question;
		[SerializeField] public string answer1;
		[SerializeField] public string answer2;
		[SerializeField] public string answer3;
		[SerializeField] public string answer4;
		[SerializeField] public string answer5;

		public string GetAnswer(int num) {
			return num switch {
				1 => answer1,
				2 => answer2,
				3 => answer3,
				4 => answer4,
				5 => answer5,
				_ => ""
			};

		}
	}
}
