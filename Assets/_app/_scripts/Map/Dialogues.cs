﻿using UnityEngine;
using EA4S.Core;

namespace EA4S.Map
{
    public class Dialogues : MonoBehaviour
    {
        public int numberStage;
        // int IsBeginningNewStage;
        void OnTriggerEnter(Collider other)
        {
            bool isMaxPosition = IsMaxJourneyPosition();
            //IsBeginningNewStage = PlayerPrefs.GetInt("IsNewStage" + numberStage);
            if ((other.gameObject.tag == "Player") && (isMaxPosition) && (numberStage > 1)) {
                Database.LocalizationDataId[] data = new Database.LocalizationDataId[7];
                data[2] = Database.LocalizationDataId.Map_Intro_Map2;
                data[3] = Database.LocalizationDataId.Map_Intro_Map3;
                data[4] = Database.LocalizationDataId.Map_Intro_Map4;
                data[5] = Database.LocalizationDataId.Map_Intro_Map5;
                data[6] = Database.LocalizationDataId.Map_Intro_Map6;
                KeeperManager.I.PlayDialog(data[numberStage], true, true);
                //PlayerPrefs.SetInt("IsNewStage" + numberStage, 1);
            }
        }

        bool IsMaxJourneyPosition()
        {
            if ((AppManager.I.Player.CurrentJourneyPosition.Stage == AppManager.I.Player.MaxJourneyPosition.Stage) &&
                (AppManager.I.Player.CurrentJourneyPosition.LearningBlock == AppManager.I.Player.MaxJourneyPosition.LearningBlock) &&
                (AppManager.I.Player.CurrentJourneyPosition.PlaySession == AppManager.I.Player.MaxJourneyPosition.PlaySession)) {
                return true;
            } else {
                return false;
            }
        }
    }
}
