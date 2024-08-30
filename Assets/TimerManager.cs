using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour {

	[SerializeField] private TMP_InputField inputNamePrefab;
	[SerializeField] private TMP_InputField inputTimerPrefab;
	[SerializeField] private TMP_Text textTimerPrefab;
	[SerializeField] private RawImage timerBackgroundPrefab;
	[SerializeField] private TMP_Text textNamePrefab;

	[SerializeField] private GameObject overlay;
	[SerializeField] private Transform canvasTransform;

	[SerializeField] private List<TMP_Text> textAnswers;


	[SerializeField] private Texture2D inactiveTimer;
	[SerializeField] private Texture2D activeTimer;

	[SerializeField] private TMP_Text questionText;

	private int correctAnswerTimeLoss;
	private List<AnswerSet> answers;
	private TeamData[] teams;

	private bool running = false;
	private bool paused = true;
	private int turn = 0;

	private int question = 0;

	private bool[] showAnswers = new bool[5];

	private void Start() {
		Screen.SetResolution(1280, 720, true);

		FileStream configFile = File.OpenRead(Path.Combine(Application.streamingAssetsPath, "config.json"));
		Config config = JsonUtility.FromJson<Config>(new StreamReader(configFile).ReadToEnd());
		correctAnswerTimeLoss = config.correctAnswerTimeLoss;
		configFile.Close();
		
		Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

		teams = new TeamData[config.teams];
		for (int i = 0; i < config.teams; i++) {
			teams[i] = new TeamData {
				inputName = Instantiate(inputNamePrefab, center + new Vector3(200 * i - (config.teams - 1) * 100, 50, 0), Quaternion.identity, overlay.transform),
				inputTimer = Instantiate(inputTimerPrefab, center + new Vector3(200 * i - (config.teams - 1) * 100, 0, 0), Quaternion.identity, overlay.transform),
				timerBackground = Instantiate(timerBackgroundPrefab, center + new Vector3(350 * i - (config.teams - 1) * 175, 175, 0), Quaternion.identity, canvasTransform),
				textTimer = Instantiate(textTimerPrefab, center + new Vector3(350 * i - (config.teams - 1) * 175, 175, 0), Quaternion.identity, canvasTransform),
				textName = Instantiate(textNamePrefab, center + new Vector3(350 * i - (config.teams - 1) * 175, 275, 0), Quaternion.identity, canvasTransform)
			};
		}
		
		overlay.transform.SetAsLastSibling();

		answers = new List<AnswerSet>();

		FileStream answersFile = File.OpenRead(Path.Combine(Application.streamingAssetsPath, "answers.csv"));
		TextReader answersReader = new StreamReader(answersFile);
		answersReader.ReadLine();

		string qText = answersReader.ReadLine();
		while (!string.IsNullOrEmpty(qText)) {
			string[] q = qText.Split(',');
			if (q.Length != 6) continue;
			AnswerSet answerSet = new AnswerSet {
				question = q[0],
				answer1 = q[1],
				answer2 = q[2],
				answer3 = q[3],
				answer4 = q[4],
				answer5 = q[5]
			};

			answers.Add(answerSet);
			qText = answersReader.ReadLine();
		}
		
		answersFile.Close();
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
			teams[turn].seconds--;
		}

		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			teams[turn].seconds++;
		}

		if (running && !paused) {
			float delta = Time.deltaTime;

			teams[turn].seconds -= delta;

			for (int i = 0; i < teams.Length; i++) {
				if (teams[i].seconds <= 0) teams[i].textTimer.color = Color.red;
			}
		}

		for (int i = 0; i < teams.Length; i++) {
			teams[i].seconds = Math.Max(teams[i].seconds, 0);
			teams[i].textTimer.text = Math.Ceiling(teams[i].seconds).ToString(CultureInfo.InvariantCulture);
		}
	}

	public void StartTimers() {
		bool parsed = true;

		for (int i = 0; i < teams.Length; i++) {
			parsed &= float.TryParse(teams[i].inputTimer.text, out teams[i].seconds);
			teams[i].textName.text = teams[i].inputName.text;
			teams[i].timerBackground.texture = i == turn ? activeTimer : inactiveTimer;
		}

		if (!parsed) return;

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
		if (num >= teams.Length) num = 0;
		if (num < 0) num = teams.Length - 1;

		turn = num;

		for (int i = 0; i < teams.Length; i++) {
			teams[i].timerBackground.texture = i == turn ? activeTimer : inactiveTimer;
		}
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

		float lowestSeconds = float.PositiveInfinity;
		int nextTurn = 0;
		for (int i = 0; i < teams.Length; i++) {
			if (teams[i].seconds > 0 && teams[i].seconds < lowestSeconds) {
				lowestSeconds = teams[i].seconds;
				nextTurn = i;
			}
		}

		ChangeTurn(nextTurn);
	}

	private void CorrectAnswer(int num) {
		if (showAnswers[num - 1]) return;
		showAnswers[num - 1] = true;
		textAnswers[num - 1].gameObject.SetActive(true);

		for (int i = 0; i < teams.Length; i++) {
			if (i == turn) continue;
			teams[i].seconds -= correctAnswerTimeLoss;
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

	private class TeamData {
		public string name;
		public float seconds;
		public TMP_InputField inputName;
		public TMP_InputField inputTimer;
		public TMP_Text textTimer;
		public RawImage timerBackground;
		public TMP_Text textName;
	}

	[Serializable]
	private struct Config {
		public int correctAnswerTimeLoss;
		public int teams;
	}
}
