using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class SceneState : MonoBehaviour
    {
        private static SceneState instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static SceneState Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (new GameObject("SceneState")).AddComponent<SceneState>();
                    DontDestroyOnLoad(instance);
                }

                return instance;
            }
        }
    
        public bool PlayerSelected { get; private set; }

        public string Player1 { get; private set; }
        public string Player2 { get; private set; }

        public void SetPlayerSelection(string player1, string player2)
        {
            this.PlayerSelected = true;
            this.Player1 = player1;
            this.Player2 = player2;
        }
    }
}
