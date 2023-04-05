
using UnityEngine.InputSystem.Controls;
using UnityEngine.Scripting;
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine.InputSystem.Editor;
#endif


////TODO: add ability to respond to any of the taps in the sequence (e.g. one response for single tap, another for double tap)

////TODO: add ability to perform on final press rather than on release

////TODO: change this so that the interaction stays performed when the tap count is reached until the button is released



namespace UnityEngine.InputSystem.Interactions
{
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    /// <summary>
    /// Interaction that requires that no more than a single tap (press and release within <see cref="tapTime"/>) 
    /// within <see cref="tapDelay"/> seconds. This equates to a chain of <see cref="TapInteraction"/> with
    /// a minimum delay between each tap.
    /// </summary>
    /// <remarks>
    /// The interaction goes into <see cref="InputActionPhase.Started"/> on the first press and then will not
    /// trigger again until either the full tap sequence is performed (in which case the interaction triggers
    /// <see cref="InputActionPhase.Performed"/>) or the multi-tap is aborted by a timeout being hit (in which
    /// case the interaction will trigger <see cref="InputActionPhase.Canceled"/>).
    /// </remarks>
    public class SingleTapInteraction : IInputInteraction
    {
        /// <summary>
        /// The time in seconds within which the control needs to be pressed and released to perform the interaction.
        /// </summary>
        /// <remarks>
        /// If this value is equal to or smaller than zero, the input system will use (<see cref="InputSettings.defaultTapTime"/>) instead.
        /// </remarks>
        [Tooltip("The maximum time (in seconds) allowed to elapse between pressing and releasing a control for it to register as a tap.")]
        public float tapTime;

        /// <summary>
        /// The time in seconds which needed to pass between taps.
        /// </summary>
        /// <remarks>
        /// If this time is not met, the single-tap interaction is canceled.
        /// If this value is equal to or smaller than zero, the input system will use the value of <see cref="tapTime"/> divided by 3 instead.
        /// </remarks>
        [Tooltip("The minimum delay (in seconds) required between each tap. If this time is not met, the single-tap interaction is canceled.")]
        public float tapDelay;

        /// <summary>
        /// Magnitude threshold that must be crossed by an actuated control for the control to
        /// be considered pressed.
        /// </summary>
        /// <remarks>
        /// If this is less than or equal to 0 (the default), <see cref="InputSettings.defaultButtonPressPoint"/> is used instead.
        /// </remarks>
        /// <seealso cref="InputControl.EvaluateMagnitude()"/>
        public float pressPoint;

        private float tapTimeOrDefault => tapTime > 0.0 ? tapTime : InputSystem.settings.defaultTapTime;
        internal float tapDelayOrDefault => tapDelay > 0.0 ? tapDelay : InputSystem.settings.multiTapDelayTime / 3f;
        private float pressPointOrDefault => pressPoint > 0 ? pressPoint : InputSystem.settings.defaultButtonPressPoint;
        private float releasePointOrDefault => pressPointOrDefault > InputSystem.settings.buttonReleaseThreshold ? InputSystem.settings.buttonReleaseThreshold : pressPointOrDefault;

        /// <inheritdoc />
        public void Process(ref InputInteractionContext context)
        {
            if (context.timerHasExpired)
            {
                // We use timers multiple times but no matter what, if they expire it means
                // that we didn't get input in time.
                context.Canceled();
                return;
            }

            switch (m_CurrentTapPhase)
            {
                case TapPhase.None:
                    if (context.ControlIsActuated(pressPointOrDefault))
                    {
                        m_CurrentTapPhase = TapPhase.WaitingForNextRelease;
                        m_CurrentTapStartTime = context.time;
                        context.Started();

                        var maxTapTime = tapTimeOrDefault;
                        var minDelayInBetween = tapDelayOrDefault;
                        context.SetTimeout(maxTapTime);

                        // We'll be using multiple timeouts so set a total completion time that
                        // effects the result of InputAction.GetTimeoutCompletionPercentage()
                        // such that it accounts for the total time we allocate for the interaction
                        // rather than only the time of one single timeout.
                        context.SetTotalTimeoutCompletionTime(maxTapTime * minDelayInBetween);
                    }
                    break;

                case TapPhase.WaitingForNextRelease:
                    if (!context.ControlIsActuated(releasePointOrDefault))
                    {
                        if (context.time - m_CurrentTapStartTime <= tapTimeOrDefault)
                        {
                            ++m_CurrentTapCount;
                            if (m_CurrentTapCount == 1)
                            {
                                context.Performed();
                                m_CurrentTapPhase = TapPhase.WaitingForNextPress;
                                m_LastTapReleaseTime = context.time;
                                context.SetTimeout(tapDelayOrDefault);
                            }
                        }
                        else
                        {
                            context.Canceled();
                        }
                    }
                    break;

                case TapPhase.WaitingForNextPress:
                    if (context.ControlIsActuated(pressPointOrDefault))
                    {
                        if (context.time - m_LastTapReleaseTime <= tapDelayOrDefault)
                        {
                            m_CurrentTapPhase = TapPhase.WaitingForNextRelease;
                            m_CurrentTapStartTime = context.time;
                            context.SetTimeout(tapTimeOrDefault);
                        }
                        else
                        {
                            context.Canceled();
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
            m_CurrentTapPhase = TapPhase.None;
            m_CurrentTapCount = 0;
            m_CurrentTapStartTime = 0;
            m_LastTapReleaseTime = 0;
        }

        private TapPhase m_CurrentTapPhase;
        private int m_CurrentTapCount;
        private double m_CurrentTapStartTime;
        private double m_LastTapReleaseTime;

        private enum TapPhase
        {
            None,
            WaitingForNextRelease,
            WaitingForNextPress,
        }
        static SingleTapInteraction()
        {
            InputSystem.RegisterInteraction<SingleTapInteraction>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() { }
    }

#if UNITY_EDITOR
    /// <summary>
    /// UI that is displayed when editing <see cref="SingleTapInteraction"/> in the editor.
    /// </summary>
    internal class SingleTapInteractionEditor : InputParameterEditor<SingleTapInteraction>
    {
        protected override void OnEnable()
        {
            m_TapTimeSetting.Initialize("Max Tap Duration",
                "Time (in seconds) within with a control has to be released again for it to register as a tap. If the control is held "
                + "for longer than this time, the tap is canceled.",
                "Default Tap Time",
                () => target.tapTime, x => target.tapTime = x, () => InputSystem.settings.defaultTapTime);
            m_TapDelaySetting.Initialize("Min Tap Spacing",
                "The minimum delay (in seconds) allowed between each tap. If this time is not met, the single-tap is canceled.",
                "Default Tap Spacing",
                () => target.tapDelay, x => target.tapDelay = x, () => InputSystem.settings.multiTapDelayTime / 3f);
            m_PressPointSetting.Initialize("Press Point",
                "The amount of actuation a control requires before being considered pressed. If not set, default to "
                + "'Default Button Press Point' in the global input settings.",
                "Default Button Press Point",
                () => target.pressPoint, v => target.pressPoint = v,
                () => InputSystem.settings.defaultButtonPressPoint);
        }

        public override void OnGUI()
        {
            m_TapDelaySetting.OnGUI();
            m_TapTimeSetting.OnGUI();
            m_PressPointSetting.OnGUI();
        }

        private CustomOrDefaultSetting m_PressPointSetting;
        private CustomOrDefaultSetting m_TapTimeSetting;
        private CustomOrDefaultSetting m_TapDelaySetting;
        internal struct CustomOrDefaultSetting
        {
            public void Initialize(string label, string tooltip, string defaultName, Func<float> getValue,
                Action<float> setValue, Func<float> getDefaultValue, bool defaultComesFromInputSettings = true,
                float defaultInitializedValue = default)
            {
                m_GetValue = getValue;
                m_SetValue = setValue;
                m_GetDefaultValue = getDefaultValue;
                m_ToggleLabel = EditorGUIUtility.TrTextContent("Default",
                    defaultComesFromInputSettings
                    ? $"If enabled, the default {label.ToLower()} configured globally in the input settings is used. See Edit >> Project Settings... >> Input (NEW)."
                    : "If enabled, the default value is used.");
                m_ValueLabel = EditorGUIUtility.TrTextContent(label, tooltip);
                if (defaultComesFromInputSettings)
                    m_OpenInputSettingsLabel = EditorGUIUtility.TrTextContent("Open Input Settings");
                m_DefaultInitializedValue = defaultInitializedValue;
                m_UseDefaultValue = Mathf.Approximately(getValue(), defaultInitializedValue);
                m_DefaultComesFromInputSettings = defaultComesFromInputSettings;
                m_HelpBoxText =
                    EditorGUIUtility.TrTextContent(
                        $"Uses \"{defaultName}\" set in project-wide input settings.");
            }

            public void OnGUI()
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(m_UseDefaultValue);

                var value = m_GetValue();

                if (m_UseDefaultValue)
                    value = m_GetDefaultValue();

                // If previous value was an epsilon away from default value, it most likely means that value was set by our own code down in this method.
                // Revert it back to default to show a nice readable value in UI.
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if ((value - float.Epsilon) == m_DefaultInitializedValue)
                    value = m_DefaultInitializedValue;

                ////TODO: use slider rather than float field
                var newValue = EditorGUILayout.FloatField(m_ValueLabel, value, GUILayout.ExpandWidth(false));
                if (!m_UseDefaultValue)
                {
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (newValue == m_DefaultInitializedValue)
                        // If user sets a value that is equal to default initialized, change value slightly so it doesn't pass potential default checks.
                        ////TODO: refactor all of this to use tri-state values instead, there is no obvious float value that we can use as default (well maybe NaN),
                        ////so instead it would be better to have a separate bool to show if value is present or not.
                        m_SetValue(newValue + float.Epsilon);
                    else
                        m_SetValue(newValue);
                }

                EditorGUI.EndDisabledGroup();

                var newUseDefault = GUILayout.Toggle(m_UseDefaultValue, m_ToggleLabel, GUILayout.ExpandWidth(false));
                if (newUseDefault != m_UseDefaultValue)
                {
                    if (!newUseDefault)
                        m_SetValue(m_GetDefaultValue());
                    else
                        m_SetValue(m_DefaultInitializedValue);
                }

                m_UseDefaultValue = newUseDefault;
                EditorGUILayout.EndHorizontal();

                // If we're using a default from global InputSettings, show info text for that and provide
                // button to open input settings.
                if (m_UseDefaultValue && m_DefaultComesFromInputSettings)
                {
                    EditorGUILayout.HelpBox(m_HelpBoxText);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(m_OpenInputSettingsLabel, EditorStyles.miniButton))
                        SettingsService.OpenProjectSettings("Project/Input System Package");
                    EditorGUILayout.EndHorizontal();
                }
            }

            private Func<float> m_GetValue;
            private Action<float> m_SetValue;
            private Func<float> m_GetDefaultValue;
            private bool m_UseDefaultValue;
            private bool m_DefaultComesFromInputSettings;
            private float m_DefaultInitializedValue;
            private GUIContent m_ToggleLabel;
            private GUIContent m_ValueLabel;
            private GUIContent m_OpenInputSettingsLabel;
            private GUIContent m_HelpBoxText;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() { }

    }
    
#endif
}