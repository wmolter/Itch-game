using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Itch {
    public class LevelingWindow : MonoBehaviour {

        public List<Button> levelButtons;
        public GameObject buttonPrefab;
        public Transform buttonContainer;
        public PlayerManager player;
        // Use this for initialization
        void Awake() {
            levelButtons = new List<Button>();
        }

        private void OnEnable() {
            RefreshButtons();
        }

        // Update is called once per frame
        void Update() {
            int index = 0;
            foreach(string skill in player.skills) {
                int level = player.AbilityLevel(skill);
                levelButtons[index].interactable = player.CanLevel(skill);
                index++;
            }
        }

        void RefreshButtons() {
            int index = 0;
            if(levelButtons.Count == 0) {
                foreach(string skill in player.skills) {
                    GameObject button = Instantiate(buttonPrefab, buttonContainer);
                    levelButtons.Add(button.GetComponent<Button>());
                    button.GetComponent<Button>().onClick.AddListener(
                        delegate () {
                            player.LevelAbility(skill);
                            RefreshButtons();
                        });
                }
            }

            foreach(string skill in player.skills) {
                int level = player.AbilityLevel(skill);
                levelButtons[index].GetComponentInChildren<Text>().text = skill + " " + level + " (" + player.XpToLevel(skill, level) + " to level)";
                levelButtons[index].interactable = player.CanLevel(skill);
                index++;
            }
        }
    }
}