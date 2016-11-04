﻿using UnityEngine;
using UnityEngine.UI;

namespace EA4S.ColorTickle
{
    public class PlayGameState : IGameState
    {
        enum eHitState
        {
            HIT_NONE = 0, HIT_LETTERINSIDE_AND_BODY, HIT_LETTERINSIDE, HIT_LETTEROUTSIDE
        }

        ColorTickleGame game;
        
        #region PRIVATE MEMBERS

        private bool m_StartToPaint;

        private ColorsUIManager m_ColorsUIManager;
        private Button m_PercentageLetterColoredButton;

        private int m_PercentageLetterColored;
        private bool m_Tickle;
        private float m_TickleTime = 1;
        private eHitState m_HitState = eHitState.HIT_NONE;
        private Vector2 m_LastBrushPosition;
        private float m_BurshDistanceCovered;
        private float m_DelayCheckVelocity;

        private float m_ClockTime;
        private int m_Lives;
        private bool m_LifeLost = false;
        private float m_TimeOutsideLetterwithBrush = 0;

        private TMPTextColoring m_TMPTextColoringLetter;
        private SurfaceColoring m_SurfaceColoringLetter;


        private bool m_FirstLetter = true;

        private int m_Rounds;

        #endregion

        public PlayGameState(ColorTickleGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            m_Rounds = game.rounds;
            m_Lives = game.lives;
            m_ClockTime = game.clockTime;


            InitGameUI();
            InitLetter();
            
            game.anturaController.isEnable = true;
            game.anturaController.letterPosition = game.currentLetter.transform.position;

            m_StartToPaint = false;
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            if (m_ClockTime < 0 || m_Lives <= 0)
            {
                ResetState();
                game.SetCurrentState(game.ResultState);
                Debug.Log("You have lost");
            }
            else
            {
                if (m_StartToPaint)
                {
                    m_ClockTime -= delta;
                    game.gameUI.SetClockTime(m_ClockTime);
                                       
                    LifeCounter(delta);

                    TickleController(delta);

                    CheckBrushVelocity(delta);

                    CalcPercentageLetterColored();
                    m_PercentageLetterColoredButton.GetComponentInChildren<Text>().text = m_PercentageLetterColored + "%";

                    if (m_PercentageLetterColored >= 100)
                    {
                        if (m_Rounds <= 0)
                        {
                            ResetState();
                            game.SetCurrentState(game.ResultState);
                        }
                        else
                        {
                            m_Rounds--;
                            //game.SetCurrentState(game.IntroductionState);   
                        }
                        Debug.Log("You win!!!!");
                    }
                }
            }           
        }

        public void UpdatePhysics(float delta)
        {
        }

        #region PRIVATE FUNCTIONS

        private void SetBrushColor(Color color)
        {
            game.currentLetter.GetComponent<ColoringParameters>().SetBrushColor(color);
            Debug.Log("New BrushColor :" + game.currentLetter);
        }

        private void TicklesLetter()
        {           
            //Debug.Log("Tickle");
            m_TickleTime = 1;
            m_Tickle = true;
            game.currentLetter.GetComponentInChildren<Animator>().SetInteger("State", game.animatorTickleState);
            //m_TMPTextColoringLetter.enableColor = false;
            //m_SurfaceColoringLetter.enableColor = false;          
        }

        private void BodyTouched()
        {
            if (m_StartToPaint)
            {
                if (m_HitState != eHitState.HIT_LETTERINSIDE)
                {
                    m_HitState = eHitState.HIT_LETTEROUTSIDE;
                    TicklesLetter();
                }
                else
                {
                    m_HitState = eHitState.HIT_LETTERINSIDE_AND_BODY;
                    m_LifeLost = false;
                }
            }
        }

        private void ShapeTouched(bool bShapeHitted)
        {
            //when the hit is inside
            if (bShapeHitted)
            {
                // Call this function before we set m_HitState = HIT_LETTERINSIDE 
                TrackBrushDistanceCovered();
                m_HitState = eHitState.HIT_LETTERINSIDE;
                if (!m_StartToPaint)
                {
                    m_StartToPaint = true;
                    m_TMPTextColoringLetter.enableColor = true;
                    m_SurfaceColoringLetter.enableColor = true;
                }
            }
            //when the hit is outside
            else
            {
                if (m_StartToPaint)
                {
                    m_HitState = eHitState.HIT_LETTEROUTSIDE;
                    TicklesLetter();
                }
            }
        }

        private void TrackBrushDistanceCovered()
        {
            if (m_HitState == eHitState.HIT_LETTERINSIDE_AND_BODY)
            {
                // if we already painted the letter the frame before
                Vector2 newBrushPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Vector2 diffBrushPositions = newBrushPosition - m_LastBrushPosition;
                m_LastBrushPosition = newBrushPosition;
                m_BurshDistanceCovered += diffBrushPositions.magnitude;
                //Debug.Log("Distance Covered:" + m_BurshDistanceCovered);
            }
            else
            {
                // if we didn't painted the letter the frame before
                m_LastBrushPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
        }

        private void CheckBrushVelocity(float delta)
        {
            m_DelayCheckVelocity += delta;
            // Every 0.5f seconds we check if the velocity of the brush is more than "m_BrushLimitVelocity" (pixel/seconds)
            // if it's more we tickles the letter
            if (m_DelayCheckVelocity >= 0.5f)
            {
                //Debug.Log("Distance Covered after 0.5s:" + m_BurshDistanceCovered);
                //Debug.Log("Velocity in pixel:" + m_BurshDistanceCovered / 0.5f);
                if (m_BurshDistanceCovered / 0.5f >= game.brushLimitVelocity)
                {
                    TicklesLetter();
                }
                m_BurshDistanceCovered = 0.0f;
                m_DelayCheckVelocity = 0.0f;
            }
        }

        private void LifeCounter(float delta)
        {
            if (m_HitState == eHitState.HIT_LETTEROUTSIDE)
            {
                m_TimeOutsideLetterwithBrush += delta;
                if (!m_LifeLost || m_TimeOutsideLetterwithBrush >= 2.0f)
                {
                    // You can lose just one life when you color outside the letter
                    // But if you keep color outside, after 1 second you lose one more life
                    // After you color again inside the letter, if color outside, can lose one more life
                    m_Lives--;
                    m_LifeLost = true;
                    m_TimeOutsideLetterwithBrush = 0.0f;
                    game.gameUI.SetLives(m_Lives);
                    Debug.Log("You have lost one life");
                }
            }
            else
            {
                m_LifeLost = false;
            }
        }

        private void TickleController(float delta)
        {
            if (m_Tickle)
            {
                m_TickleTime -= delta;
                if (m_TickleTime < 0 && !game.currentLetter.GetComponentInChildren<Animator>().IsInTransition(0))
                {
                    m_HitState = eHitState.HIT_NONE;
                    m_Tickle = false;
                    game.currentLetter.GetComponentInChildren<Animator>().SetInteger("State", 1);
                }
            }
        }

        private void CalcPercentageLetterColored()
        {
            m_PercentageLetterColored = ((m_TMPTextColoringLetter.currentShapePixelsColored * 100) /
                                            m_TMPTextColoringLetter.totalShapePixels);
            if (m_PercentageLetterColored > 0)
            {
                m_PercentageLetterColored += 100 - m_TMPTextColoringLetter.percentageRequiredToWin;
            }
            if (m_PercentageLetterColored > 95)
            {
                m_PercentageLetterColored = 100;
            }
        }

        private void InitGameUI()
        {
            if (m_FirstLetter)
            {
                m_ColorsUIManager = game.colorsCanvas.GetComponentInChildren<ColorsUIManager>();
                m_ColorsUIManager.SetBrushColor += SetBrushColor;
            }

            m_PercentageLetterColoredButton = GameObject.Find("PercentageButton").GetComponent<Button>();
            m_PercentageLetterColoredButton.GetComponentInChildren<Text>().text = "0 %";
        }

        private void InitLetter()
        {
            if (m_FirstLetter)
            {              
                m_TMPTextColoringLetter = game.currentLetter.GetComponent<TMPTextColoring>();
                m_SurfaceColoringLetter = game.currentLetter.GetComponent<SurfaceColoring>();
                m_TMPTextColoringLetter.OnShapeHit += ShapeTouched;
                m_SurfaceColoringLetter.OnBodyHit += BodyTouched;
                //m_TMPTextColoringLetter.enableColor = false;
                //m_SurfaceColoringLetter.enableColor = false;
                m_FirstLetter = false;
            }

            SetBrushColor(m_ColorsUIManager.defaultColor);
            game.currentLetter.GetComponentInChildren<Animator>().SetInteger("State", 1);
        }

        private void ResetState()
        {
            game.currentLetter.GetComponentInChildren<Animator>().SetInteger("State", 1);
            m_ColorsUIManager.SetBrushColor -= SetBrushColor;
            m_TMPTextColoringLetter.OnShapeHit -= ShapeTouched;
            m_SurfaceColoringLetter.OnBodyHit -= BodyTouched;
            m_TMPTextColoringLetter.enableColor = false;
            m_SurfaceColoringLetter.enableColor = false;
            game.anturaController.isEnable = false;
        }

        #endregion
    }


}
