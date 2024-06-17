using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class Selectable : MonoBehaviour
    {
        bool bRotating=true;

   
        public    float PosDiameter = 1.0f;
        public float angle = 0;
        public float xPlace = -0.4f;
        public float rotateSpeed = 20.0f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            float posZ = PosDiameter * Mathf.Cos(Mathf.Deg2Rad * angle);
            float posY = PosDiameter * Mathf.Sin(Mathf.Deg2Rad * angle);

            gameObject.transform.localPosition = new Vector3(xPlace, posY , posZ);
        }

        // Update is called once per frame
        void Update()
        {
            if(bRotating)
            {
                gameObject.transform.Rotate(Vector3.left, rotateSpeed * Time.deltaTime);
            }
        }
    }
}
