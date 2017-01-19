﻿

namespace EA4S
{
    /// <summary>
    /// Example implementation of ILivingLetterDataProvider.
    /// Not to be used in actual production code.
    /// </summary>
    public class SampleWordProvider : ILivingLetterDataProvider
    {
        public SampleWordProvider()
        {

        }

        public ILivingLetterData GetNextData()
        {
            return AppManager.I.Teacher.GetRandomTestWordDataLL();
        }
    }
}
