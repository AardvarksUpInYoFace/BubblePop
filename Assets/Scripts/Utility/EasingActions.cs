using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JoniUtility
{
    public class EasingActions
    {
        public IEnumerator CoFadeImageAlpha(float iterator, float time, float startA, float endA, Image image, Easing.Function function, Easing.Direction direction)
        {
            while (iterator < time)
            {
                iterator += Time.deltaTime;
                if (iterator > time) iterator = time;

                float val = JoniUtility.Easing.GetTerpedPosition(iterator, time, function, direction);

                float newA = ExtendedLerp.LerpWithoutClamp(startA, endA, val);

                Color col = image.color;

                image.color = new Color(col.r, col.g, col.b, newA);
                yield return 0;
            }
        }

        public IEnumerator CoFadeTextAlpha(float iterator, float time, float startA, float endA, Text text, Easing.Function function, Easing.Direction direction)
        {
            while (iterator < time)
            {
                iterator += Time.deltaTime;
                if (iterator > time) iterator = time;

                float val = JoniUtility.Easing.GetTerpedPosition(iterator, time, function, direction);

                float newA = ExtendedLerp.LerpWithoutClamp(startA, endA, val);

                Color col = text.color;

                text.color = new Color(col.r, col.g, col.b, newA);
                yield return 0;
            }
        }

        public IEnumerator CoScale(float iterator, float time, float startS, float endS, Transform transform, Easing.Function function, Easing.Direction direction)
        {
            while (iterator < time)
            {
                iterator += Time.deltaTime;
                if (iterator > time) iterator = time;

                float val = JoniUtility.Easing.GetTerpedPosition(iterator, time, function, direction);

                float newS = ExtendedLerp.LerpWithoutClamp(startS, endS, val);

                transform.localScale = new Vector3(newS, newS, newS);

                yield return 0;
            }
        }

        public IEnumerator CoScaleLoop(float iterator, float time, float startS, float endS, Transform transform, Easing.Function function, Easing.Direction direction)
        {

            while (iterator < time)
            {

                iterator += Time.deltaTime;
                if (iterator > time) iterator = time;

                float val = JoniUtility.Easing.GetTerpedPosition(iterator, time, function, direction);

                float newS = ExtendedLerp.LerpWithoutClamp(startS, endS, val);

                transform.localScale = new Vector3(newS, newS, newS);

                if (iterator == time)
                {
                    iterator = 0;
                    float tempS = endS;
                    endS = startS;
                    startS = tempS;

                }

                yield return 0;
            }
        }


        public IEnumerator CoMoveX(float iterator, float time, float startX, float endX, Transform transform, Easing.Function function, Easing.Direction direction)
        {
            while (iterator < time)
            {
                iterator += Time.deltaTime;
                if (iterator > time) iterator = time;

                float val = JoniUtility.Easing.GetTerpedPosition(iterator, time, function, direction);

                float newX = ExtendedLerp.LerpWithoutClamp(startX, endX, val);

                Vector3 position = transform.localPosition;

                transform.localPosition = new Vector3(newX, position.y, position.z);

                yield return 0;
            }
        }
    
        public IEnumerator CoMoveY(float iterator, float time, float startY, float endY, Transform transform, Easing.Function function, Easing.Direction direction)
        {
            while (iterator < time)
            {
                iterator += Time.deltaTime;
                if (iterator > time) iterator = time;

                float val = JoniUtility.Easing.GetTerpedPosition(iterator, time, function, direction);

                float newY = ExtendedLerp.LerpWithoutClamp(startY, endY, val);

                Vector3 position = transform.localPosition;

                transform.localPosition = new Vector3(position.x, newY, position.z);

                yield return 0;
            }
        }
    
    }
}
