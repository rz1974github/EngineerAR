using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public enum partType
    {
        PT_Fake0=0,
        PT_Pillow1 = 1,
        PT_DarkGray2 = 2,
        PT_Purple3 = 3,
        PT_TRLight4 = 4,
        PT_Pillars5 = 5,
        PT_Floor6 = 6,
        PT_Front_Panel7 = 7,
        PT_Front_Pillars8 = 8,
        PT_None = 9,
    }

    public class Selectable : MonoBehaviour
    {
        public static bool bRotating=true;

        public partType myType;
        public float PosDiameter = 1.0f;
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
