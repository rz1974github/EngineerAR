using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class MessageTimer : MonoBehaviour
    {
        float showtime = 0;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            gameObject.GetComponent<Renderer>().enabled = false;
        }

        public void ShowTime()
        {
            showtime = 1.0f;
        }

        public void Finish()
        {
            gameObject.GetComponent<TMPro.TMP_Text>().text = "FINISHED!";
        }

        // Update is called once per frame
        void Update()
        {
            if(showtime>0)
            {
                showtime -= Time.deltaTime;
                gameObject.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                gameObject.GetComponent<Renderer>().enabled = false;
            }
        }
    }
}
