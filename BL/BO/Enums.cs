using System;

namespace BO
{
    public class Enums
    {
        /// <summary>
        /// Enum for Distance TypeEnum
        /// </summary>
        public enum DistanceType
        {
            AirDistance,
            WalkingDistance,
            DrivingDistance
        }

        /// <summary>
        /// Enum for PositionEnum Status
        /// </summary>
        public enum PositionEnum
        {
            Manager,
            Volunteer
        }

        public enum CallTypeEnum
        {
            Urgent = 1,
            Medium_Urgency,
            General_Assistance,
            Non_Urgent,
        }

        // סטטוס הקריאה (ENUM)
        public enum CallStatus
        {
            Treated = 1,
            SelfCanceled,
            AdminCanceled,
            Expired
        }
    }
}

