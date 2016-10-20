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

            letterProvider = new SampleLetterProvider();
            wordProvider = new SampleWordProvider();

            onlyLetter = Random.Range(0, 2) == 1;
        }

        public void SetOnlyLetter(bool onlyLetter)
        {
            this.onlyLetter = onlyLetter;
        }

        public ILivingLetterData GetNextData()
        {
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
