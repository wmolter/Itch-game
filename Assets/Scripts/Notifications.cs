using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Itch {
    public class Notifications : MonoBehaviour {

        static Notifications instance;

        public static void CreateNotification(Vector2 position, string text) {
            instance.CreateNotif(position, text, instance.stdColor);
        }

        public static void CreateError(Vector2 position, string text) {
            instance.CreateNotif(position, text, instance.errorColor);
        }

        public static void CreateNegative(Vector2 position, string text) {
            instance.CreateNotif(position, text, instance.negativeColor);
        }

        public static void CreatePositive(Vector2 position, string text) {
            instance.CreateNotif(position, text, instance.positiveColor);
        }

        public static void CreateNotification(Vector2 position, string text, Color color) {
            instance.CreateNotif(position, text, color);
        }

        public static ProgressHint CreateHint(Vector2 position) {
            return CreateHint(position, 0);
        }

        public static ProgressHint CreateHelperHint(Vector2 position) {
            return instance._ConvertToHelper(CreateHint(position));
        }

        public static ProgressHint CreateHint(Vector2 position, float progress) {
            return instance._CreateHint(position, progress);
        }

        public Canvas worldCanvas;
        public ProgressHint hintPrefab;
        public Vector2 littleHintOffset;
        public Vector2 littleHintScale;
        public Text textPrefab;
        public Vector2 stackSpacing;
        public Color errorColor;
        public Color negativeColor;
        public Color stdColor;
        public Color positiveColor;

        public float clearStackDelay = .5f;
        float clearTime;

        Dictionary<Vector2, int> stackCounts;
        // Use this for initialization
        void Awake() {
            instance = this;
            stackCounts = new Dictionary<Vector2, int>();
        }

        private void OnDestroy() {
            instance = null;
        }

        public void CreateNotif(Vector2 position, string text) {
            CreateNotif(position, text, textPrefab.color);
        }

        public void CreateNotif(Vector2 position, string text, Color color) {
            if(!stackCounts.ContainsKey(position)) {
                stackCounts[position] = 0;
            }
            InstantiateNotif(position, text, color, stackCounts[position]);
            stackCounts[position] += 1;
        }

        private void InstantiateNotif(Vector2 position, string text, Color color, int stackNumber) {
            clearTime = Time.time + clearStackDelay;
            Text notif = Instantiate(textPrefab, worldCanvas.transform);
            notif.text = text;
            notif.transform.position = position + stackNumber*stackSpacing;
            notif.color = color;
        }

        public ProgressHint _CreateHint(Vector2 position, float progress) {
            ProgressHint hint = Instantiate(hintPrefab, worldCanvas.transform);
            hint.transform.position = position;
            hint.SetProgress(progress);
            return hint;
        }

        public ProgressHint _CreateHint(Vector2 position, float progress, string spriteName, string text) {
            ProgressHint hint = _CreateHint(position, progress);
            Sprite sprite = Resources.Load<Sprite>("Sprites/" + spriteName);
            hint.under.sprite = sprite;
            hint.main.sprite = sprite;
            hint.cornerText.text = text;
            return hint;
        }

        private ProgressHint _ConvertToHelper(ProgressHint hint) {
            hint.transform.position = (Vector2)hint.transform.position + littleHintOffset;
            hint.transform.localScale = littleHintScale;
            return hint;
        }

        // Update is called once per frame
        void LateUpdate() {
            if(Time.time > clearTime)
                stackCounts.Clear();
        }
    }
}