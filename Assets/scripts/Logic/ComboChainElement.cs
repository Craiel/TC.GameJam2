using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class ComboChainElement : MonoBehaviour
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Animation;

        public float AnimationSpeed = 1.0f;
        public float Delay;
        public float Force;

        public float Damage = 10;

        public AudioClip SFX;
        public AudioClip SFXHit;
    }
}
