﻿using EA4S.MinigamesAPI;
using EA4S.MinigamesCommon;

namespace EA4S.Egg
{
    public class EggConfiguration : IGameConfiguration
    {
        public enum EggVariation : int
        {
            Single = 1,
            Sequence = 2,
        }

        // Game configuration
        public IGameContext Context { get; set; }
        public IQuestionProvider Questions { get; set; }
        public float Difficulty { get; set; }
        public EggVariation Variation { get; set; }

        /////////////////
        // Singleton Pattern
        static EggConfiguration instance;
        public static EggConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new EggConfiguration();
                return instance;
            }
        }
        /////////////////

        private EggConfiguration()
        {
            // Default values
            // THESE SETTINGS ARE FOR SAMPLE PURPOSES, THESE VALUES MUST BE SET BY GAME CORE
            Context = new MinigamesGameContext(MiniGameCode.Egg, System.DateTime.Now.Ticks.ToString());
            Difficulty = 0.1f;
            Variation = EggVariation.Sequence;

            if (Variation == EggVariation.Sequence)
                Questions = new SampleEggSequenceQuestionProvider();
            else
                Questions = new SampleEggSingleQuestionProvider();
        }

        public IQuestionBuilder SetupBuilder()
        {
            IQuestionBuilder builder = null;

            int nPacks = 10;
            int nCorrect = 6;
            int nWrong = 7;

            var builderParams = new Teacher.QuestionBuilderParameters();
            builderParams.correctSeverity = Teacher.SelectionSeverity.AsManyAsPossible;

            builder = new RandomLettersQuestionBuilder(nPacks, nCorrect, nWrong, parameters: builderParams);

            return builder;
        }

        public MiniGameLearnRules SetupLearnRules()
        {
            var rules = new MiniGameLearnRules();
            // example: a.minigameVoteSkewOffset = 1f;
            return rules;
        }
    }
}