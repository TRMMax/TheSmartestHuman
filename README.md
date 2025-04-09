# The Smartest Human

## How to run
A built version for Windows is available [here](https://github.com/TRMMax/TheSmartestHuman/releases).
Alternatively, you can build the project yourself:
- Download Unity Hub and install editor version 2022.3.2f1.
- Clone the repo and open the project using Unity Hub. It may complain about compilation errors when opening. Ignore these and continue with importing (don't enter safe mode).
- Create a build via File->Build Settings...
- Select PC, Mac & Linux Standalone, change Target Platform to the desired OS and click Build in the bottom right.
- Give the build a name. In case of a Mac build, a single runnable file is created. Windows and Linux should create a folder with the executable inside.

## Setup
- To change config (built version of Windows only), edit TheSmartestHuman_Data/StreamingAssets/config.json
  correctAnswerTimeLoss: Determines how many seconds the other teams lose when a correct answer is given.
  teams: The amount of teams to play with.
- To change the answers, edit TheSmartestHuman_Data/StreamingAssets/answers.csv
- In Unity, these can be found under the Project Tab (bottom tab of screen), then under Assets->StreamingAssets.

## How to use
Enter the team names of the top teams and their seconds they gathered.
The program will automatically count down and keep track of the rewards for correct answers.

## Controls
- Space - Pause/unpause the timer. The timer will keep on running regardless of switching active team or going to the next question.
- Left/Right Arrow - Set the previous/next team as active. The active team's timer will count down when active, and the inactive teams will get the penalty for a correct answer.
- Up/Down Arrow - increase/decrease active teams seconds by 1.
- Enter (return) - Go to the next question and hide all answers.
- 1-5 Keys - Mark given question as correct, show the answer and apply penalty to inactive teams.
