﻿using EA4S.Audio;
using EA4S.Core;
using EA4S.UI;
using EA4S.Utilities;
using UnityEngine;
using TMPro;
using EA4S.Antura;
using System.Collections.Generic;
using EA4S.MinigamesCommon;

namespace EA4S.AnturaSpace
{
    /// <summary>
    /// Manages the AnturaSpace scene.
    /// </summary>
    // refactor: group this class with other scene managers
    public class AnturaSpaceManager : MonoBehaviour
    {
        const int MaxBonesInScene = 5;

        public AnturaLocomotion Antura;
        public AnturaSpaceUI UI;

        public Transform SceneCenter;
        public Transform AttentionPosition;
        public Transform BoneSpawnPosition;
        public GameObject BonePrefab;
        public GameObject PoofPrefab;

        public Transform DraggingBone { get; private set; }
        public Transform NextBoneToCatch
        {
            get
            {
                if (bones.Count == 0)
                    return null;
                return bones[0].transform;
            }
        }
        List<GameObject> bones = new List<GameObject>();

        public readonly AnturaIdleState Idle;
        public readonly AnturaCustomizationState Customization;
        public readonly AnturaDrawingAttentionState DrawingAttention;
        public readonly AnturaSleepingState Sleeping;
        public readonly AnturaWaitingThrowState WaitingThrow;
        public readonly AnturaCatchingState Catching;

        StateManager stateManager = new StateManager();
        public AnturaState CurrentState
        {
            get
            {
                return (AnturaState)stateManager.CurrentState;
            }
            set
            {
                stateManager.CurrentState = value;
            }
        }

        public bool HasPlayerBones
        {
            get
            {
                var totalBones = AppManager.I.Player.GetTotalNumberOfBones();

                return totalBones > 0;
            }
        }

        public void ThrowBone()
        {
            if (DraggingBone != null)
                return;

            if (bones.Count < MaxBonesInScene && AppManager.I.Player.TotalNumberOfBones > 0)
            {
                var bone = Instantiate(BonePrefab);
                bone.SetActive(true);
                bone.transform.position = BoneSpawnPosition.position;
                bone.GetComponent<BoneBehaviour>().SimpleThrow();
                bones.Add(bone);
                --AppManager.I.Player.TotalNumberOfBones;
            }
        }

        public bool InCustomizationMode { get; private set; }

        /// <summary>
        /// Drag a bone around.
        /// </summary>
        public void DragBone()
        {
            if (DraggingBone != null)
                return;

            if (bones.Count < MaxBonesInScene && AppManager.I.Player.TotalNumberOfBones > 0)
            {
                var bone = Instantiate(BonePrefab);
                bone.SetActive(true);
                bone.transform.position = BoneSpawnPosition.position;
                DraggingBone = bone.transform;
                bones.Add(bone);
                --AppManager.I.Player.TotalNumberOfBones;
                bone.GetComponent<BoneBehaviour>().Drag();
            }
        }

        public void EatBone(GameObject bone)
        {
            if (bones.Remove(bone))
            {
                AudioManager.I.PlaySound(Sfx.EggMove);
                var poof = Instantiate(PoofPrefab).transform;
                poof.position = bone.transform.position;

                foreach (var ps in poof.GetComponentsInChildren<ParticleSystem>())
                {
                    var main = ps.main;
                    main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                }

                poof.localScale = poof.localScale * 0.5f;
                poof.gameObject.AddComponent<AutoDestroy>().duration = 2;
                AudioManager.I.PlaySound(Sfx.Poof);
                Destroy(bone);
            }
        }

        public AnturaSpaceManager()
        {
            Idle = new AnturaIdleState(this);
            Customization = new AnturaCustomizationState(this);
            DrawingAttention = new AnturaDrawingAttentionState(this);
            Sleeping = new AnturaSleepingState(this);
            WaitingThrow = new AnturaWaitingThrowState(this);
            Catching = new AnturaCatchingState(this);
        }

        void Awake()
        {
            CurrentState = Idle;

            UI.onEnterCustomization += OnEnterCustomization;
            UI.onExitCustomization += OnExitCustomization;

            Antura.onTouched += () => { if (CurrentState != null) CurrentState.OnTouched(); };
        }

        public void Update()
        {
            stateManager.Update(Time.deltaTime);

            UI.ShowBonesButton(bones.Count < MaxBonesInScene);
            UI.BonesCount = AppManager.I.Player.GetTotalNumberOfBones();

            if (DraggingBone != null && !Input.GetMouseButton(0))
            {
                DraggingBone.GetComponent<BoneBehaviour>().LetGo();
                DraggingBone = null;
            }
        }

        public void FixedUpdate()
        {
            stateManager.UpdatePhysics(Time.fixedDeltaTime);
        }

        public Music backgroundMusic;

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);

            if (!AppManager.I.Player.IsFirstContact())
            {
                ShowBackButton();
            }

            AudioManager.I.PlayMusic(backgroundMusic);
            LogManager.I.LogInfo(InfoEvent.AnturaSpace, "enter");

        }

        public void ShowBackButton()
        {
            GlobalUI.ShowBackButton(true, OnExit);
        }

        void OnExit()
        {
            LogManager.I.LogInfo(InfoEvent.AnturaSpace, "exit");
            AppManager.I.NavigationManager.GoBack();
        }

        void OnEnterCustomization()
        {
            InCustomizationMode = true;
            CurrentState = Customization;
        }

        void OnExitCustomization()
        {
            InCustomizationMode = false;
            CurrentState = Idle;
        }
    }
}
