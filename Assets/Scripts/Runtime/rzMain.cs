using UnityEngine;
using TMPro;
using System;

namespace UnityEngine.XR.ARFoundation.Samples
{
    enum GameState
    {
        GS_None,
        GS_Game,
        GS_GameSucceed,
        GS_GameFailed
    }

    public class rzMain : MonoBehaviour
    {
        public GameObject[] mainArray;
        public GameObject[] wireArray;

        //public GameObject theModel;
        public TMPro.TMP_Text timeText;
        public GameObject correct;
        public GameObject startButton;
        public GameObject targetIcon;
        public GameObject targetHint;
        public GameObject resetButton;
        public GameObject clockSet;
        public GameObject moreButton;
        public GameObject rankButton;
        public GameObject menuPanel;
        public GameObject rankPanel;

        int turnIndex = 0;

        float timer = 360.0f;

        GameState mGameState;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            rzPanelScript.windowClosedEvent += onPanelClosed;
            resetToReady();
        }

        void onPanelClosed()
        {
            moreButton.SetActive(true);
        }

        public void onRankClosed()
        {
            rankButton.SetActive(true);
        }

        public void resetToReady()
        {
            mGameState = GameState.GS_None;
            //hide
            targetIcon.SetActive(false);
            targetHint.SetActive(false);
            clockSet.SetActive(false);
            menuPanel.SetActive(false);
            rankPanel.SetActive(false);

            //show
            resetButton.SetActive(true);
            moreButton.SetActive(true);
            rankButton.SetActive(true);

            timer = 360.0f;
            refreshTimeText();

            foreach (GameObject gmo in mainArray)
            {
                gmo.SetActive(false);
            }

            foreach (GameObject gmo in wireArray)
            {
                gmo.SetActive(true);
            }
        }

        public void resetSwitch()
        {
            if(mGameState == GameState.GS_None)
            {
                startPressed();
            }
            else
            {
                resetToReady();
            }
        }

        public void startPressed()
        {
            moreButton.SetActive(false);
            rankButton.SetActive(false);
            menuPanel.SetActive(false);
            rankPanel.SetActive(false);

            timer = 360.0f;
            mGameState = GameState.GS_Game;

            targetIcon.SetActive(true);
            targetHint.SetActive(true);
            resetButton.SetActive(true);
            clockSet.SetActive(true);
        }

        public void showMenuPanel()
        {
            menuPanel.SetActive(true);
            rankButton.SetActive(true);

            rankPanel.SetActive(false);

            moreButton.SetActive(false);
        }

        public void showRankPanel()
        {
            rankPanel.SetActive(true);
            moreButton.SetActive(true);

            rzPanelScript menuScript = menuPanel.GetComponent<rzPanelScript>();

            menuScript.closeAllPages();

            rankButton.SetActive(false);
        }

        public void inTurnActive()
        {
            activateIndex(turnIndex);
            turnIndex = (turnIndex + 1);

            if(turnIndex==8)
            {
                mGameState = GameState.GS_None;
            }
            else
            {
                correct.GetComponent<MessageTimer>().ShowTime();
            }
        }

        public void activateIndex(int index)
        {
            if(index < mainArray.Length)
            {
                mainArray[index].SetActive(true);
                wireArray[index].SetActive(false);
            }
        }

        void refreshTimeText()
        {

            timer = Mathf.Max(0, timer);

            int nSec = (int)timer % 60;
            int nMin = (int)(timer - (float)nSec) / 60;

            timeText.text = nMin.ToString("00") + ":" + nSec.ToString("00"); //test
        }

        // Update is called once per frame
        void Update()
        {
            if(mGameState == GameState.GS_Game)
            {
                timer -= Time.deltaTime;
                refreshTimeText();
            }
        }
    }
}
