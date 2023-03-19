using System.ComponentModel;
using UnityEditor;



namespace UnityEngine.InputSystem.Interactions
{
    #if UNITY_EDITOR
        [InitializeOnLoad]
    #endif

    [DisplayName("AttackPress")]
    public class AttackPressInteraction : IInputInteraction
    {
        public static InputSettings Settings { get; }

        private bool m_WaitingForRelease;

        public void Process(ref InputInteractionContext context)
        {
            var actuation = Mathf.Abs(context.ReadValue<Vector3>().z);
            Debug.Log(context.ReadValue<Vector3>());

            if (m_WaitingForRelease)
            {
                if (actuation <= Settings.buttonReleaseThreshold)
                {
                    m_WaitingForRelease = false;
                    if (Mathf.Approximately(0f, actuation))
                    {
                        context.Canceled();
                    }
                    else
                        context.Started();
                }
            }
            else if (actuation >= Settings.defaultButtonPressPoint)
            {
                m_WaitingForRelease = true;
                // Stay performed until release.
                context.Performed();
            }
            else if (actuation > 0 && !context.isStarted)
            {
                context.Started();
            }
            else if (Mathf.Approximately(0f, actuation) && context.isStarted)
            {
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
