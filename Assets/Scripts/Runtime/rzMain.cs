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
        public GameObject partsSelection;
        public Camera MainCamera;

        partType turnIndex = partType.PT_Pillow1;

        public float total_time = 360.0f;

        float timer;

        GameState mGameState;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            rankArray[0] = 0;
            rankArray[1] = 0;
            rankArray[2] = 0;
            rankArray[3] = 0;

            rzPanelScript.windowClosedEvent += onPanelClosed;
            resetToReady(GameState.GS_None);
            resetModels();

            Debug.Log("Main Started");
        }

        // Update is called once per frame
        void Update()
        {
            if (mGameState != GameState.GS_Game) return;

            timer -= Time.deltaTime;
            timer = Mathf.Max(0, timer);
            timeText.text = floatTimeToString(timer);

            if (timer <= 0)
            {
                Debug.Log("Game Failed!");

                resetToReady(GameState.GS_GameFailed);

                EndMessageBox.clearAllTimeUpHandler();
                EndMessageBox.onTimeUp += whenFailedFinished;
                EndMessage.text = "時間到! 任務失敗!";
                EndMessageBox.ShowTime(3.0f);
            }
        }

        void activateIndex(int index)
        {
            if (index < mainArray.Length)
            {
                mainArray[index].SetActive(true);
                wireArray[index].SetActive(false);
            }
        }

        void checkToWriteRank(float time)
        {
            int nFound = 999;
            for (int i = 0; i < 4; i++)
            {
                if (rankArray[i] <
                    time)
                {
                    nFound = i;
                    break;
                }
            }

            if (nFound != 999)
            {
                for (int j = 3; j >= nFound; j--)
                {
                    int k = j + 1;
                    if (k < 4)
                    {
                        rankArray[k] = rankArray[j];
                    }
                }
                rankArray[nFound] = timer;

                //reflect to Text
                for (int n = 0; n < 4; n++)
                {
                    rankTextArray[n].text = floatTimeToString(rankArray[n]);
                    Debug.Log("rank[" + n + "]=" + rankArray[n]);
                }
            }
        }

        void onPanelClosed()
        {
            moreButton.SetActive(true);
        }

        void resetToReady(GameState toState)
        {
            mGameState = toState;
            turnIndex = partType.PT_Pillow1;
            //hide
            //targetIcon.SetActive(false);
            targetUI.SetActive(false);
            targetHint.SetActive(false);
            clockSet.SetActive(false);
            menuPanel.SetActive(false);
            rankPanel.SetActive(false);
            correctBox.gameObject.SetActive(false);
            partsSelection.SetActive(false);

            //show
            resetButton.SetActive(true);
            moreButton.SetActive(true);
            rankButton.SetActive(true);

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

        String floatTimeToString(float value)
        {

            int nSec = (int)value % 60;
            int nMin = (int)(value - (float)nSec) / 60;

            return nMin.ToString("00") + ":" + nSec.ToString("00");
        }

        partType checkType(GameObject gameo)
        {
            Selectable sel = gameo.GetComponent<Selectable>();

            if (sel != null)
            {
                return sel.myType;
            }

            return partType.PT_None;
        }

        partType checkFamilyPartType(GameObject gameo)
        {
            partType finalType = checkType(gameo);

            if (finalType != partType.PT_None) return finalType;

            GameObject father = gameo.transform.parent.gameObject;
            if(father!=null)
            {
                finalType = checkType(father);

                if (finalType != partType.PT_None) return finalType;

                GameObject grandFather = father.transform.parent.gameObject;
                if (grandFather != null)
                {
                    finalType = checkType(grandFather);
                    if (finalType != partType.PT_None) return finalType;
                }
            }

            return partType.PT_None;
        }

        public void rayCastTest()
        {
            if (mGameState != GameState.GS_Game) return;

            Vector3 startPos = MainCamera.transform.position;

            Vector3 direction = MainCamera.transform.forward;

            RaycastHit hitInfo;

            bool Touched = false;
            if(Physics.Raycast(startPos, direction, out hitInfo, 100.0f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal))
            {
                if(hitInfo.collider!=null)
                {
                    Debug.Log("Hit collider = " + hitInfo.collider.ToString());

                    GameObject hitObject = hitInfo.collider.gameObject;
                    if (hitObject != null)
                    {
                        Debug.Log("Hit Object = " + hitObject.ToString());

                        partType pt = checkFamilyPartType(hitObject);
                        if (pt != partType.PT_None)
                        {
                            /*
                            EndMessageBox.clearAllTimeUpHandler();
                            EndMessage.text = "Part type=" + pt.ToString();
                            EndMessageBox.ShowTime(2.0f);
                            */
                            if(pt==turnIndex)
                            {
                                CorrectActive();
                            }
                            else if (pt == partType.PT_Fake0)
                            {
                                EndMessageBox.clearAllTimeUpHandler();
                                EndMessage.text = "零件錯誤~";
                                EndMessageBox.ShowTime(2.0f);
                            }
                            else
                            {
                                EndMessageBox.clearAllTimeUpHandler();
                                EndMessage.text = "順序錯了!";
                                EndMessageBox.ShowTime(2.0f);
                            }
                            Touched = true;
                        }
                    }
                }
            }
            if(!Touched)
            {
                EndMessageBox.clearAllTimeUpHandler();
                EndMessage.text = "MISS!";
                EndMessageBox.ShowTime(1.0f);
                Touched = true;
            }
        }

        public void CorrectActive()
        {
            if (mGameState != GameState.GS_Game ) return;

            activateIndex((int)turnIndex);
            turnIndex = (partType)((int)turnIndex + 1);

            if (turnIndex == partType.PT_Fake0)
            {
                resetToReady(GameState.GS_GameSucceed);

                EndMessageBox.clearAllTimeUpHandler();
                EndMessageBox.onTimeUp += whenSucceedFinished;
                EndMessage.text = "任務成功! 時間 " + timeText.text;
                EndMessageBox.ShowTime(3.0f);
                checkToWriteRank(timer);

                Debug.Log("Game Succeed");
            }
            else
            {
                correctBox.ShowTime(0.75f);
            }
        }

        public void onRankClosed()
        {
            rankButton.SetActive(true);
        }

        public void resetSwitch()
        {
            if(mGameState == GameState.GS_None)
            {
                resetToReady(GameState.GS_None);
                resetModels();
                startPressed();
            }
            else
            {
                resetToReady(GameState.GS_None);
                resetModels();
            }
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

        public void startPressed()
        {
            moreButton.SetActive(false);
            rankButton.SetActive(false);
            menuPanel.SetActive(false);
            rankPanel.SetActive(false);

            timer = total_time;
            mGameState = GameState.GS_Game;

            //targetIcon.SetActive(true);
            targetUI.SetActive(true);
            targetHint.SetActive(true);
            resetButton.SetActive(true);
            clockSet.SetActive(true);
            partsSelection.SetActive(true);
            //Selectable.bRotating = true;
        }

        public void whenSucceedFinished()
        {
            mGameState = GameState.GS_None;

            Debug.Log("whenSucceedFinished done");
        }

        public void whenFailedFinished()
        {
            mGameState = GameState.GS_None;
            resetModels();
            Debug.Log("whenFailedFinished done");
        }
    }
}
