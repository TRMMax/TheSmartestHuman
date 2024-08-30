## The Smartest Human

# Setup
- To change config, edit TheSmartestHuman_Data/StreamingAssets/config.json
  correctAnswerTimeLoss: Determines how many seconds the other teams lose when a correct answer is given.
  teams: The amount of teams to play with.
- To change the answers, edit TheSmartestHuman_Data/StreamingAssets/answers.csv

# How to use
Enter the team names of the top teams and their seconds they gathered.
The program will automatically count down and keep track of the rewards for correct answers.

# Controls
Space - Pause/unpause the timer. The timer will keep on running regardless of switching active team or going to the next question.
Left/Right Arrow - Set the previous/next team as active. The active team's timer will count down when active, and the inactive teams will get the penalty for a correct answer.
Up/Down Arrow - increase/decrease active teams seconds by 1.
Enter (return) - Go to the next question and hide all answers.
1-5 Keys - Mark given question as correct, show the answer and apply penalty to inactive teams.
