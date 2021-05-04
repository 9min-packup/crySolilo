using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrySolilo
{
    public class GameSystem : MonoBehaviour
    {
        public Camera MainCamera;
        public ScenarioManager scenarioManager;
        public Database database;
        public SaveDataManager saveDataManager;
        public AudioManager audioManager;
        public UIManager uiManager;
        public SettingManager settingManager;
        public InputManager inputManager;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}