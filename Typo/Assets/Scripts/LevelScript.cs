using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Threading;
using TMPro; // Required for StringReader

public class LevelScript : MonoBehaviour
{
    public const int START_SECTION = 34;
    private TextAsset storyCSV;
    [SerializeField] int storyIndex;
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] TextMeshProUGUI incomingSMSBody;
    [SerializeField] TextMeshProUGUI preTemplate;
    [SerializeField] TextMeshProUGUI userInputSection;
    [SerializeField] TextMeshProUGUI postTemplate;

    private float time = 0.0f;
    public List<StoryRow> section1 = new List<StoryRow>();
    void Awake()
    {
        storyCSV = Resources.Load<TextAsset>("sms_templates");
        var fullText = storyCSV.text;
        Debug.Log("story csv" + storyCSV.ToString());


        using (StringReader reader = new StringReader(fullText))
        {
            // Optional: Read and skip the header line
            string headerLine = reader.ReadLine();

            string line;
            var count = 0;
            while ((line = reader.ReadLine()) != null)
            {
                if (count++ < START_SECTION)
                {
                    // skip lines before start
                    continue;
                }
                string[] values = line.Split(',');

                StoryRow entry = new StoryRow();
                // 1 charecter
                // 2 incoming_sms
                // 3 user template
                // 5 sms_reply
                // Charecter _charecter;
                if (Enum.TryParse(values[1], out Charecter _charecter)) entry.charecter = _charecter;
                entry.incomingSMS = values[2];
                entry.replyTemplate = values[3];
                entry.replySMS = values[5];
                entry.time = 30; // TODO: change later maybe
                entry.section = 1;
                section1.Add(entry);

            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        storyIndex = -1; // set to -1, so next one will be 0 (the first)
        setNextSMS(); // start the first sms
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        float seconds = time % 60f;
        float minutes = time / 60f;
        timer.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds); ;
    }

    void setNextSMS()
    {
        storyIndex++;
        var storyRow = section1[storyIndex];
        var parts = storyRow.replyTemplate.Split('`');
        preTemplate.text = parts[0];
        userInputSection.text = parts[1];
        postTemplate.text = parts[2];
        incomingSMSBody.text = storyRow.incomingSMS;
    }
}


public enum Charecter
{
    BestFriend,
    Teacher,
    Mom,
    Frenemy,
    Crush,
    Dog,
}
public class StoryRow
{
    // public int id;
    public int section;
    public Charecter charecter;
    public string incomingSMS;
    public string replyTemplate;
    public string replySMS;
    public float time;
}