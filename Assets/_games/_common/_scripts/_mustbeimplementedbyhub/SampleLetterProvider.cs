﻿namespace EA4S
{
    /// <summary>
    /// Example implementation of ILivingLetterDataProvider.
    /// Not to be used in actual production code.
    /// </summary>
    public class SampleLetterProvider : ILivingLetterDataProvider
    {
        public SampleLetterProvider()
        {

        }
        
        public ILivingLetterData GetNextData()
        {
            return AppManager.I.Teacher.GetRandomTestLetterLL();
        }
    }
}
