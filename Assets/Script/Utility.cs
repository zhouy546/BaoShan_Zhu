using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zhou.utility{

    public static class Utility 
    {

        public static float Maping(float value, float inputMin, float inputMax, float outputMin, float outputMax, bool clamp)
        {
            float outVal = ((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin);

            if (clamp)
            {
                if (outputMax < outputMin)
                {
                    if (outVal < outputMax) outVal = outputMax;
                    else if (outVal > outputMin) outVal = outputMin;
                }
                else
                {
                    if (outVal > outputMax) outVal = outputMax;
                    else if (outVal < outputMin) outVal = outputMin;
                }
            }


            return outVal;
        }
    }

}

