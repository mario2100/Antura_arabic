﻿using EA4S.Helpers;
using SQLite;

namespace EA4S.Database
{
    /// <summary>
    /// Score (in stars) relative to a journey element or a minigame. Updated at runtime.
    /// </summary>
    [System.Serializable]
    public class JourneyScoreData : IData, IScoreData
    {
        /// <summary>
        /// Primary key for the database.
        /// Set based on ElementId and JourneyDataType
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// Type of journey data recorded.
        /// </summary>
        public JourneyDataType JourneyDataType { get; set; }

        /// <summary>
        /// Id of the element for which the score has been recorded.
        /// This is related to the primary key of the Static table for the related JourneyDataType.
        /// </summary>
        public string ElementId { get; set; }

        /// <summary>
        /// Stars obtained during this play session.
        /// Integer in the [0,3] range
        /// </summary>
        public int Stars { get; set; }

        /// <summary>
        /// Timestamp of the last update of this entry.
        /// </summary>
        public int UpdateTimestamp { get; set; }

        /// <summary>
        /// Empty constructor required by MySQL.
        /// </summary>
        public JourneyScoreData(){}

        public JourneyScoreData(string elementId, JourneyDataType dataType, int stars) : this(elementId, dataType, stars, GenericHelper.GetTimestampForNow())
        {
        }

        public JourneyScoreData(string elementId, JourneyDataType dataType, int stars, int timestamp)
        {
            ElementId = elementId;
            JourneyDataType = dataType;
            Id = JourneyDataType + "." + ElementId;
            Stars = stars;
            UpdateTimestamp = timestamp;
        }

        public float GetScore()
        {
            return Stars;
        }

        public string GetId()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("T{0},E{1},S{2},T{3}",
                JourneyDataType,
                ElementId,
                Stars,
                UpdateTimestamp
                );
        }

    }
}