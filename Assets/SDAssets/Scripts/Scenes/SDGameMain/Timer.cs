/*
 * File Name: TimerScore.cs
 * Description: Script for TimeText. This sciprt works as a timer.
 */


using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace SD {
    public class Timer : MonoBehaviour {

        public Text timeText;
        public Text countdownText;
        public float time = 180f;
        public GameObject panelTimesUp;

        private int maxCountdownSeconds = 6;
        private DateTime offsetDateTime;
        private static GameManager sdGameManager;
        private static GameController sdGameController;
        private static SDPersistentData sdPersistentData;
        private bool hasTimerStarted;

        public void Start(){
            timeText.text = "Time: " +time.ToString ();
            sdGameManager = GameManager.getInstance ();
            sdGameController = GameController.getInstance ();
            sdPersistentData = SDPersistentData.getInstance ();
            hasTimerStarted = false;
        }

        void Update() {
            if (!hasTimerStarted && sdPersistentData != null) {
                if (DateTime.Equals (TrimMilliseconds (DateTime.UtcNow), sdPersistentData.getRoundStartTime ())) {
                    sdGameController.setIsGameTimeTicking (true);
                    Debug.Log ("Starting the timer now at: " + DateTime.UtcNow);
                    hasTimerStarted = true;
                    sdGameController.countdownPanelCanvas.SetActive (false);
                } else {
                    double secondsToGo = (sdPersistentData.getRoundStartTime () - TrimMilliseconds (DateTime.UtcNow)).TotalSeconds;
                    if (secondsToGo >= 0) {
                        if (secondsToGo > maxCountdownSeconds) { // use the timer threshold in case local system clock is incorrect.
                            if (offsetDateTime == null)
                                offsetDateTime = (TrimMilliseconds (DateTime.UtcNow)).AddSeconds(maxCountdownSeconds);
                            secondsToGo = (offsetDateTime - TrimMilliseconds (DateTime.UtcNow)).TotalSeconds;
                        }
                        countdownText.GetComponent<Text> ().text = secondsToGo.ToString ();
                    }
                    else {
                        // Start the timer anyway since it has passed the scheduled start time.
                        sdGameController.setIsGameTimeTicking (true);
                        Debug.Log ("Starting the timer now at: " + DateTime.UtcNow);
                        hasTimerStarted = true;
                        sdGameController.countdownPanelCanvas.SetActive (false);
                    }
                }
            }
            if (sdGameController.getIsGameTimeTicking ()) {
                time -= 1f * Time.deltaTime;
                timeText.text = "Time:\n" + ((int)time).ToString ();
                if (time <= 0) {
                    //timeText.text = "Time's Up!";
                    panelTimesUp.SetActive (true);
                    sdGameController.setIsGameTimeTicking (false);
                    StartCoroutine (EndCurrentRound ());
                }
            }
            
        }

        float GetTime (){
            return this.time;
        }
        
        public static DateTime TrimMilliseconds(DateTime dt) {
            return new DateTime (dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0);
        }

        IEnumerator EndCurrentRound() {
            yield return new WaitForSeconds(3);
            sdGameManager.EndGame (true, sdGameController.getPlayerScore ());
            Debug.Log ("The player's final score is " + sdGameController.getPlayerScore ());
            Destroy (this);
        }
    }
}