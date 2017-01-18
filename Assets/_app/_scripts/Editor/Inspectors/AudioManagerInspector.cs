﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EA4S
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerInspector : UnityEditor.Editor
    {
        GUIStyle titlesStyle;
        GUIStyle subtitleStyle;
        GUIStyle subtitleBadStyle;
        GUIStyle goodStyle;
        GUIStyle badStyle;

        void Initialize()
        {
            titlesStyle = new GUIStyle();
            titlesStyle.fontSize = 20;
            titlesStyle.fontStyle = FontStyle.Bold;
            titlesStyle.normal.textColor = Color.white;

            subtitleStyle = new GUIStyle();
            subtitleStyle.fontSize = 14;
            subtitleStyle.fontStyle = FontStyle.Bold;
            subtitleStyle.normal.textColor = Color.white;

            subtitleBadStyle = new GUIStyle();
            subtitleBadStyle.fontSize = 14;
            subtitleBadStyle.fontStyle = FontStyle.Bold;
            subtitleBadStyle.normal.textColor = Color.red;

            goodStyle = new GUIStyle();
            goodStyle.normal.textColor = Color.yellow;

            badStyle = new GUIStyle();
            badStyle.normal.textColor = Color.red;

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Initialize();

            AudioManager myTarget = (AudioManager)target;

            if (GUILayout.Button("Grab from Fabric"))
            {
                myTarget.sfxConfs.Clear();

                var components = myTarget.transform.GetComponentsInChildren<Fabric.AudioComponent>(true);
                foreach (var audioComponent in components)
                {
                    var listener = audioComponent.GetComponent<Fabric.EventListener>();

                    if (listener != null)
                    {
                        bool found = false;
                        Sfx sfx = 0;
                        foreach (var s in Enum.GetValues(typeof(Sfx)))
                        {
                            if (AudioConfig.GetSfxEventName((Sfx)s) == listener._eventName)
                            {
                                sfx = (Sfx)s;
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            SfxConfiguration conf = myTarget.sfxConfs.Find((a) => { return a.sfx == sfx; });

                            if (conf == null)
                                conf = new SfxConfiguration();

                            conf.sfx = sfx;
                            myTarget.sfxConfs.Add(conf);

                            conf.volume = audioComponent.Volume;
                            conf.randomPitchOffset = audioComponent.PitchRandomization;

                            var random = audioComponent as Fabric.RandomAudioClipComponent;

                            if (random != null)
                            {
                                foreach (var c in random._audioClips)
                                    conf.clips.Add(c);
                            }
                            else
                                conf.clips.Add(audioComponent.AudioClip);
                        }
                    }
                }

                var rndcomponents = myTarget.transform.GetComponentsInChildren<Fabric.RandomComponent>(true);
                foreach (var randomComponent in rndcomponents)
                {
                    var listener = randomComponent.GetComponent<Fabric.EventListener>();

                    if (listener != null)
                    {
                        bool found = false;
                        Sfx sfx = 0;
                        foreach (var s in Enum.GetValues(typeof(Sfx)))
                        {
                            if (AudioConfig.GetSfxEventName((Sfx)s) == listener._eventName)
                            {
                                sfx = (Sfx)s;
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            SfxConfiguration conf = myTarget.sfxConfs.Find((a) => { return a.sfx == sfx; });

                            if (conf == null)
                                conf = new SfxConfiguration();
                            conf.sfx = sfx;
                            conf.volume = randomComponent.Volume;
                            conf.randomPitchOffset = randomComponent.PitchRandomization;

                            var childs = randomComponent.GetChildComponents();

                            if (childs != null)
                                foreach (var child in childs)
                                {
                                    var random = child as Fabric.RandomAudioClipComponent;

                                    if (random != null)
                                    {
                                        foreach (var c in random._audioClips)
                                            conf.clips.Add(c);
                                    }
                                    else
                                    {
                                        var c = child as Fabric.AudioComponent;
                                        if (c != null)
                                            conf.clips.Add(c.AudioClip);
                                    }
                                }
                        }
                    }
                }
            }

            EditorGUILayout.LabelField("-- Sound effects --", titlesStyle);
            EditorGUILayout.Separator();
            foreach (var sfx in Enum.GetValues(typeof(Sfx)))
            {
                SfxConfiguration conf = myTarget.sfxConfs.Find((a) => { return a.sfx == (Sfx)sfx; });

                if (conf == null)
                {
                    conf = new SfxConfiguration();
                    conf.sfx = (Sfx)sfx;
                    conf.volume = 1;
                    myTarget.sfxConfs.Add(conf);
                }


                List<AudioClip> clips = conf.clips;

                int count = clips != null ? clips.Count : 0;

                int nonNullCount = -1;
                for (int i = 0; i < count; ++i)
                {
                    var clip = clips[i];
                    if (clip != null)
                        ++nonNullCount;
                }

                // Shrink
                if (count != nonNullCount)
                {
                    var newClips = new List<AudioClip>();
                    for (int i = 0; i < count; ++i)
                    {
                        var clip = clips[i];
                        if (clip != null)
                            newClips.Add(clips[i]);
                    }
                    clips = newClips;
                }

                bool hasClips = clips.Count > 0;
                EditorGUILayout.LabelField("Sfx." + sfx.ToString(), hasClips ? subtitleStyle : subtitleBadStyle);

                for (int i = 0; i < clips.Count; ++i)
                    clips[i] = (AudioClip)EditorGUILayout.ObjectField(clips[i], typeof(AudioClip), false);

                EditorGUILayout.BeginHorizontal();

                if (clips.Count > 0)
                    EditorGUILayout.LabelField("Add Random Choice:", hasClips ? goodStyle : badStyle);

                var newClip = (AudioClip)EditorGUILayout.ObjectField(null, typeof(AudioClip), false);

                EditorGUILayout.EndHorizontal();

                if (newClip != null)
                {
                    clips.Add(newClip);

                }

                conf.clips = clips;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Volume:", hasClips ? goodStyle : badStyle);
                conf.volume = EditorGUILayout.Slider(conf.volume, 0, 1);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Random Pitch Offset (+/-):", hasClips ? goodStyle : badStyle);
                conf.randomPitchOffset = EditorGUILayout.Slider(conf.randomPitchOffset, 0, 10);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Separator();
            }

            EditorGUILayout.LabelField("-- Music --", titlesStyle);
            EditorGUILayout.Separator();
            foreach (var music in Enum.GetValues(typeof(Music)))
            {
                MusicConfiguration conf = myTarget.musicConfs.Find((a) => { return a.music == (Music)music; });

                if (conf == null)
                {
                    conf = new MusicConfiguration();

                    conf.music = (Music)music;
                    myTarget.musicConfs.Add(conf);
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Music." + music.ToString(), conf.clip ? goodStyle : badStyle);
                conf.clip = (AudioClip)EditorGUILayout.ObjectField(conf.clip, typeof(AudioClip), false);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}