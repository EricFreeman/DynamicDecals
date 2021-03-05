using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LlockhamIndustries.Misc
{
    public class DebugManager : MonoBehaviour
    {

        //Initializable MultiScene Singleton
        private static DebugManager singleton;
        private void Awake()
        {
            if (singleton == null) singleton = this;
            else if (singleton != this)
            {
                Destroy(gameObject);
            }
        }

        //Log Rect
        public Font font;
        public RectTransform logRect;
        public float logHeight = 40;

        //Current output
        private List<DebugEntry> Entries = new List<DebugEntry>();

        private void Start()
        {
            CreateEntries();
        }
        private void OnEnable()
        {
            Application.logMessageReceived += OnLog;
        }
        private void OnDisable()
        {
            Application.logMessageReceived -= OnLog;
        }

        //Log
        public static void Log(string Title, string Log)
        {
            if (singleton != null)
            {
                //Grab our entries
                List<DebugEntry> entries = singleton.Entries;

                //If log already exists, update it
                foreach (DebugEntry entry in entries)
                {
                    if (entry.title == Title)
                    {
                        entry.Update(Title, Log);
                        return;
                    }
                }

                //If log doen't exist, move all other logs down and replace the first
                for (int i = entries.Count - 1; i > 0; i--)
                {
                    entries[i].Update(entries[i - 1].title, entries[i - 1].log);
                }
                entries[0].Update(Title, Log);
            }
        }
        private void OnLog(string logString, string stackTrace, LogType type)
        {
            Log(type.ToString(), logString);
        }

        //Initialization
        private void CreateEntries()
        {
            int EntryCount = Mathf.FloorToInt(logRect.rect.height / logHeight);

            for (int i = 0; i < EntryCount; i++)
            {
                Entries.Add(new DebugEntry(font, logRect, logHeight, i));
            }
        }
    }

    internal class DebugEntry
    {
        public Text text;
        public RectTransform transform;

        public string title;
        public string log;

        public DebugEntry(Font Font, RectTransform LogRect, float LogHeight, int Index)
        {
            //Create go
            GameObject go = new GameObject("Entry");

            //Create and setup transform
            transform = go.AddComponent<RectTransform>();
            transform.SetParent(LogRect, false);

            transform.anchorMin = new Vector2(0, 1 - ((LogHeight / LogRect.rect.height) * Index));
            transform.anchorMax = new Vector2(0, 1 - ((LogHeight / LogRect.rect.height) * Index));
            transform.offsetMin = new Vector2(0, -LogHeight);
            transform.offsetMax = new Vector2(LogRect.rect.width, 0);

            text = go.AddComponent<Text>();
            text.alignment = TextAnchor.MiddleLeft;
            text.color = Color.white;
            text.fontSize = 20;
            text.font = Font;
            text.text = "";
        }
        public void Update(string Title, string Log)
        {
            title = Title;
            log = Log;

            if (Title != "" || Log != "") text.text = title + " : " + log;
            else text.text = "";

        }
    }
}