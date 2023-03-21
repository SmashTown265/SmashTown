using System.ComponentModel;
using UnityEditor;
using UnityEngine.InputSystem.Controls;

namespace UnityEngine.InputSystem.Interactions
{
    #if UNITY_EDITOR
        [InitializeOnLoad]
    #endif

    [DisplayName("AttackPress")]
    public class AttackPressInteraction : IInputInteraction
    {
        private InputSettings settings = InputSystem.settings;

        private bool m_WaitingForRelease;

        public void Process(ref InputInteractionContext context)
        {
            var actuation = Mathf.Abs(context.ReadValue<Vector3>().z);
            //Debug.Log(context.ReadValue<Vector3>());
            /*if (m_WaitingForRelease)
            {
                if (actuation <= settings.buttonReleaseThreshold)
                {
                    m_WaitingForRelease = false;
                    if (Mathf.Approximately(0f, actuation))
                        context.Canceled();
                    else
                        context.Started();
                }
                else if (context.phase.IsInProgress())
                    context.Canceled();
                else
                    return;
            }
            else if (actuation >= settings.defaultButtonPressPoint)
            {
                m_WaitingForRelease = true;
                // Stay performed until release.
                context.PerformedAndStayPerformed();
            }
            else if (actuation > 0 && !context.phase.IsInProgress())
            {
                context.Started();
            }
            else if (Mathf.Approximately(0f, actuation) && context.isStarted)
            {
                context.Canceled();
            }*/
            if (m_WaitingForRelease)
            {
                if (actuation <= settings.buttonReleaseThreshold)
                {
                    m_WaitingForRelease = false;
                    context.Performed();
                    if (Mathf.Approximately(0, actuation))
                        context.Canceled();
                }
            }
            else if (actuation >= settings.defaultButtonPressPoint)
            {
                m_WaitingForRelease = true;
                context.PerformedAndStayPerformed();
            }
            else
            {
                var started = context.isStarted;
                if (actuation > 0 && !started)
                    context.Started();
                else if (Mathf.Approximately(0, actuation) && started)
                    context.Canceled();
            }
        }

        public void Reset()
        {
            m_WaitingForRelease = false;
        }
        static AttackPressInteraction()
        {
            InputSystem.RegisterInteraction<AttackPressInteraction>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() { }
    }

}
