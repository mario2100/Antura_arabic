﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Teacher
{
    /// <summary>
    /// Handles the selection of the difficulty to use for a given minigame
    /// </summary>
    public class DifficultySelectionAI 
    {

        // References
        private DatabaseManager dbManager;
        private PlayerProfile playerProfile;
        //private TeacherAI teacher;

        // Weights
        private float ageWeight = ConfigAI.difficulty_weight_age;       // Higher age -> higher difficulty
        private float journeyWeight = ConfigAI.difficulty_weight_journey;     // Higher journey stage -> higher difficulty
        private float performanceWeight = ConfigAI.difficulty_weight_performance;       // Higher performance -> higher difficulty

        // @todo: move these to ConfigAI
        private const float minimumAge = 7;
        private const float maximumAge = 15;

        private const float minimumStage = 0;
        private const float maximumStage = 7;

        private const float minimumMiniGameScore = -1;
        private const float maximumMiniGameScore = 1;
        private const float startingDifficultyForNewMiniGame = 0f;

        public DifficultySelectionAI(DatabaseManager _dbManager, PlayerProfile _playerProfile, TeacherAI _teacher)
        {
            this.dbManager = _dbManager;
            this.playerProfile = _playerProfile;
            //this.teacher = _teacher;
        }

        public float SelectDifficulty(MiniGameCode miniGameCode)
        {
            float totalWeight = ageWeight + journeyWeight + performanceWeight;

            // Journey
            var currentJourneyPosition = playerProfile.CurrentJourneyPosition;
            var playerStage = currentJourneyPosition.Stage;
            float journeyDifficulty = Mathf.Clamp01(Mathf.InverseLerp(minimumStage, maximumStage, playerStage));
            float weightedJourneyDifficulty = journeyDifficulty * journeyWeight / totalWeight;

            // Age
            var playerAge = playerProfile.Age;
            float ageDifficulty = Mathf.Clamp01(Mathf.InverseLerp(minimumAge, maximumAge, playerAge));
            float weightedAgeDifficulty = ageDifficulty * ageWeight / totalWeight;

            // Performance
            float playerPerformance;
            string query = string.Format("SELECT * FROM ScoreData WHERE TableName = 'MiniGames' AND ElementId = '{0}'", (int)miniGameCode);
            List<Db.ScoreData> minigame_scoreData_list = dbManager.FindScoreDataByQuery(query);
            if (minigame_scoreData_list.Count == 0)
            {
                playerPerformance = startingDifficultyForNewMiniGame;
            }
            else
            {
                float minigameScore = minigame_scoreData_list[0].Score;
                playerPerformance = minigameScore;
            }

            float performanceDifficulty = Mathf.Clamp01(Mathf.InverseLerp(minimumMiniGameScore, maximumMiniGameScore, playerPerformance));
            float weightedPerformanceDifficulty = performanceDifficulty * performanceWeight / totalWeight;

            // Total
            float totalDifficulty = weightedAgeDifficulty + weightedJourneyDifficulty + weightedPerformanceDifficulty;

            // Debug log
            if (ConfigAI.verboseTeacher)
            {
                string debugString = "-----  TEACHER: Selected Difficulty: " + totalDifficulty + " -----";
                debugString += "\n From Age (W " + ageWeight + "): " + ageDifficulty + " xw(" + weightedAgeDifficulty + ")";
                debugString += "\n From Stage (W " + journeyWeight + "): " + journeyDifficulty + " xw(" + weightedJourneyDifficulty + ")";
                debugString += "\n From Performance (W " + performanceWeight + "): " + performanceDifficulty + " xw(" + weightedPerformanceDifficulty + ")";
                Debug.Log(debugString);
            }

            return totalDifficulty;
        }

    }
}