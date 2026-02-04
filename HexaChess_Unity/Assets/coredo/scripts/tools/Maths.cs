
using UnityEngine;

namespace edocle.tools
{
    public class Maths
    {

        #region Ballistics

        /// <summary>
        /// Calculates the initial launch velocity required to hit a target at distance x with elevation yOffset.
        /// </summary>
        /// <param name="distance">Ground distance from origin to target</param>
        /// <param name="yOffset">Elevation of the origin from ground</param>
        /// <param name="gravity">Downward acceleration in m/s^2</param>
        /// <param name="angle">Initial launch angle in radians</param>
        /// <returns>Init launch velocity</returns>
        public static float GetInitLaunchVelocity(float distance, float yOffset, float gravity, float angle)
        {
            return (distance * Mathf.Sqrt(gravity) * Mathf.Sqrt(1 / Mathf.Cos(angle))) /
                Mathf.Sqrt(2 * distance * Mathf.Sin(angle) + 2 * yOffset * Mathf.Cos(angle));
        }


        // @edo -- NEED VERIFICATION
        /// <summary>
        /// Calculates the two possible initial angles that could be used to fire a projectile at the supplied
        /// speed to travel the desired distance
        /// </summary>
        /// <param name="speed">Initial speed of the projectile</param>
        /// <param name="distance">Distance along the horizontal axis the projectile will travel</param>
        /// <param name="yOffset">Elevation of the target with respect to the initial fire position</param>
        /// <param name="gravity">Downward acceleration in m/s^2</param>
        /// <param name="angle0">Higher angle</param>
        /// <param name="angle1">Lower angle</param>
        /// <returns>False if the target is out of range</returns>
        public static bool LaunchAngle(float speed, float distance, float yOffset, float gravity, out float angle0, out float angle1)
        {
            angle0 = angle1 = 0;

            float speedSquared = speed * speed;

            float operandA = Mathf.Pow(speed, 4);
            float operandB = gravity * (gravity * (distance * distance) + (2 * yOffset * speedSquared));

            // Target is not in range
            if (operandB > operandA)
                return false;

            float root = Mathf.Sqrt(operandA - operandB);

            angle0 = Mathf.Atan((speedSquared + root) / (gravity * distance));
            angle1 = Mathf.Atan((speedSquared - root) / (gravity * distance));

            return true;
        }


        // @edo -- NEED VERIFICATION
        /// <summary>
        /// Calculates how long a projectile will stay in the air before reaching its target
        /// </summary>
        /// <param name="speed">Initial speed of the projectile</param>
        /// <param name="angle">Initial launch angle in radians</param>
        /// <param name="yOffset">Elevation of the target with respect to the initial fire position</param>
        /// <param name="gravity">Downward acceleration in m/s^2</param>
        /// <returns></returns>
        public static float TimeOfFlight(float speed, float angle, float yOffset, float gravity)
        {
            float ySpeed = speed * Mathf.Sin(angle);

            float time = (ySpeed + Mathf.Sqrt((ySpeed * ySpeed) + 2 * gravity * yOffset)) / gravity;

            return time;
        }


        #endregion Ballistics

        #region Noises

        [System.Serializable]
        public class PerlinNoiseParameters
        {
            public int m_RandomOffsetXRange = 100;
            public int m_RandomOffsetYRange = 100;
            public float m_Intensity = 1;
            public float m_NoiseRange = 1;

            public void SetRandomRange()
            {
                m_RandomOffsetX = UnityEngine.Random.Range(0, m_RandomOffsetXRange);
                m_RandomOffsetY = UnityEngine.Random.Range(0, m_RandomOffsetYRange);
            }

            public int m_RandomOffsetX { get; private set; }
            public int m_RandomOffsetY { get; private set; }
        }

        #endregion Noises
    }
}
