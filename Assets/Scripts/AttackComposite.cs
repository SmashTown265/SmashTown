using System.ComponentModel;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.InputSystem.Editor;
#endif

namespace UnityEngine.InputSystem.Composites
{
#if UNITY_EDITOR
    [InitializeOnLoad] // Automatically register in editor.
#endif

    // Determine how GetBindingDisplayString() formats the composite by applying
    // the  DisplayStringFormat attribute.
    [DisplayStringFormat("{attackButtonPrimary}+{attackButtonAlternate}+{attackDirection}")]
    [DisplayName("Directional binding with primary and alternate modifiers")]
    public class AttackComposite : Vector3Composite
    {
        [InputControl(layout = "Button")]
        public int attackButtonPrimary; // This button will override the alternate if both are pressed

        [InputControl(layout = "Button")]
        public int attackButtonAlternate;

        [InputControl(layout = "Vector2")]
        public int attackDirection;

        private Vector3 attack;
        public bool NeedsToBeWithinACertainTimeFrame;
        public float PressedWithinNumberOfSeconds;
        private bool PrimaryPressed;
        private bool AlternatePressed;
        private Vector2 attackDir;
        private int m_ValueSizeInBytes;


        // This method computes the resulting input value of the composite based
        // on the input from its part bindings.
        public override Vector3 ReadValue(ref InputBindingCompositeContext context)
        {
            PrimaryPressed = context.ReadValueAsButton(attackButtonPrimary);
            AlternatePressed = context.ReadValueAsButton(attackButtonAlternate);
            attackDir = context.ReadValue<Vector2, Vector2MagnitudeComparer>(attackDirection);

            //... do some processing and return value
            attack = attackDir;
            attack.z = PrimaryPressed ? 1 : AlternatePressed ? -1 : 0;

            if (IsPressed(ref context))
            {
                return attack;
            }
            else
                return default;
        }

        private bool IsPressed(ref InputBindingCompositeContext context)
        {
            if (NeedsToBeWithinACertainTimeFrame && (PrimaryPressed || AlternatePressed))
            {
                double timestamp;
                double timestamp1 = context.GetPressTime(attackDirection);

                if (PrimaryPressed)
                {
                    timestamp = context.GetPressTime(attackButtonPrimary);
                }
                else
                {
                    timestamp = context.GetPressTime(attackButtonAlternate);
                }



                if (Mathf.Abs((float)(timestamp - timestamp1)) > PressedWithinNumberOfSeconds)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return PrimaryPressed || AlternatePressed;
            }
        }
        static AttackComposite()
        {
            InputSystem.RegisterBindingComposite<AttackComposite>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() { }

    }
}