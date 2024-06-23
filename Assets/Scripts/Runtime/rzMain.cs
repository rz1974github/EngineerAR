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

        float[] rankArray = new float[4];
        public TMPro.TMP_Text[] rankTextArray;

        //public GameObject theModel;
        public TMPro.TMP_Text timeText;
        public MessageTimer correctBox;
        public MessageTimer EndMessageBox;
        public TMPro.TMP_Text EndMessage;
        public GameObject targetIcon;
        public GameObject targetUI;
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
            rankArray[0] = 599.0f;
            rankArray[1] = 599.0f;
            rankArray[2] = 599.0f;
            rankArray[3] = 599.0f;

            rzPanelScript.windowClosedEvent += onPanelClosed;
            resetToReady(GameState.GS_None);
            resetModels();

            Debug.Log("Main Started");
        }

        void onPanelClosed()
        {
            moreButton.SetActive(true);
        }

        public void onRankClosed()
        {
            rankButton.SetActive(true);
        }

        void resetToReady(GameState toState)
        {
            mGameState = toState;
            turnIndex = 0;
            //hide
            //targetIcon.SetActive(false);
            targetUI.SetActive(false);
            targetHint.SetActive(false);
            clockSet.SetActive(false);
            menuPanel.SetActive(false);
            rankPanel.SetActive(false);
            correctBox.gameObject.SetActive(false);

            //show
            resetButton.SetActive(true);
            moreButton.SetActive(true);
            rankButton.SetActive(true);

            timer = 360.0f;
            timeText.text = floatTimeToString(timer);
        }

        void resetModels()
        {
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
                resetToReady(GameState.GS_None);
                resetModels();
            }
        }

        public void startPressed()
        {
            moreButton.SetActive(false);
            rankButton.SetActive(false);
            menuPanel.SetActive(false);
            rankPanel.SetActive(false);

            timer = 40.0f;
            mGameState = GameState.GS_Game;

            //targetIcon.SetActive(true);
            targetUI.SetActive(true);
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

        void checkToWriteRank(float time)
        {
            int nFound = 999;
            for(int i=0;i<4;i++)
            {
                if(rankArray[i] > time)
                {
                    nFound = i;
                    break;
                }
            }

            if(nFound!=999)
            {
                for(int j=nFound;j<4;j++)
                {
                    int k = j + 1;
                    if(k<4)
                    {
                        rankArray[k] = rankArray[j];
                    }
                }
                rankArray[nFound] = timer;
            }

            //reflect to Text
            for(int n=0;n<4;n++)
            {
                rankTextArray[n].text = floatTimeToString(rankArray[n]);
                Debug.Log("rank[" + n + "]=" + rankArray[n]);
            }
        }    

        public void inTurnActive()
        {
            activateIndex(turnIndex);
            turnIndex = (turnIndex + 1);

            if(turnIndex==8)
            {
                resetToReady(GameState.GS_GameSucceed);

                EndMessageBox.clearAllTimeUpHandler();
                EndMessageBox.onTimeUp += whenSucceedFinished;
                EndMessage.text = "成功! 時間 " + timeText.text;
                EndMessageBox.ShowTime(3.0f);
                checkToWriteRank(timer);

                Debug.Log("Game Succeed");
            }
            else
            {
                correctBox.ShowTime( 0.75f);
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

        public void whenSucceedFinished()
        {
            mGameState = GameState.GS_None;
        }

        public void whenFailedFinished()
        {
            mGameState = GameState.GS_None;
            resetModels();
        }

        String floatTimeToString(float value)
        {

            int nSec = (int)value % 60;
            int nMin = (int)(value - (float)nSec) / 60;

            return nMin.ToString("00") + ":" + nSec.ToString("00");
        }

        // Update is called once per frame
        void Update()
        {
            if(mGameState == GameState.GS_Game)
            {
                timer -= Time.deltaTime;
                timer = Mathf.Max(0, timer);
                timeText.text = floatTimeToString(timer);

                if (timer<=0)
                {
                    resetToReady(GameState.GS_GameFailed);

                    EndMessageBox.clearAllTimeUpHandler();
                    EndMessageBox.onTimeUp += whenFailedFinished;
                    EndMessage.text = "時間到 任務失敗";
                    EndMessageBox.ShowTime(3.0f);

                    Debug.Log("Game Failed!");
                }
            }
        }
    }
}
