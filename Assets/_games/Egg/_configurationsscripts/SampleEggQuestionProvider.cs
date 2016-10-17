﻿using UnityEngine;

namespace EA4S.Egg
{
    public class SampleEggQuestionProvider : ILivingLetterDataProvider
    {
        float difficulty;
        bool onlyLetter = false;

        ILivingLetterDataProvider letterProvider;
        ILivingLetterDataProvider wordProvider;

        public SampleEggQuestionProvider(float difficulty)
        {
            this.difficulty = difficulty;

            letterProvider = new SampleLetterProvider(difficulty);
            wordProvider = new SampleWordProvider();

            onlyLetter = Random.Range(0, 2) == 1;
        }

        public ILivingLetterData GetNextData()
        {
            //onlyLetter = true;


            if (onlyLetter)
            {
                return letterProvider.GetNextData();
            }
            else
            {
                return wordProvider.GetNextData();
            }
        }
    }
}
