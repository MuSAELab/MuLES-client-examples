using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class simple_client : MonoBehaviour
{
    // 1. Connection with MuLES 
    static MulesClient mules_client = new MulesClient("127.0.0.1", 30000);
    static AudioSource audio_source;
    static Text text_title;
    float[,] data_buffer;
    float[] data_buffer_1ch;
    int channel = 4;
    int n_samples;
    double update_period = 0.1;
    double toc;
    // plot scale and position
    float x_scale = 0.2f;
    float x_offset = 500f;
    float y_scale = 0.3f;
    float y_offset = 300f;
    LineRenderer line_renderer;

    // Use this for initialization
    void Start()
    {
        // audio source for tones
        audio_source = gameObject.AddComponent<AudioSource>();
        // line renderer and its properties for plot
        line_renderer = gameObject.AddComponent<LineRenderer>();
        line_renderer.material = new Material(Shader.Find("Sprites/Default"));
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        line_renderer.colorGradient = gradient;
        text_title = GameObject.Find("text_title").GetComponent<Text>();
        text_title.text = "";

        string device_name = mules_client.getdevicename();           // get device name
        string [] channel_names = mules_client.getnames();           // get channel names
        double fs = mules_client.getfs();                            // get sampling frequency

        // 2. Defining EEG data buffer for 10 seconds
        n_samples = (int)(10 * fs);
        int n_columns = channel_names.Length;
        data_buffer = new float[n_samples, n_columns];
        data_buffer_1ch = new float[n_samples];
        line_renderer.positionCount = n_samples;

        // 3. Flush old data from the Server 
        mules_client.flushdata();

        // Create text
        Debug.Log("EEG data from: " + device_name + ". Electrode: " + channel_names[channel - 1]);
        text_title.text = "EEG data from: " + device_name + ". Electrode: " + channel_names[channel - 1];
        mules_client.tone(audio_source, 600, 250);

        toc = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // If a update_period has passed
        if (Time.time - toc >= update_period)
        {
            toc = Time.time;
            // Get new EEG data from MuLES
            float[,] new_data = mules_client.getalldata();
            // Put new EEG data in buffer
            data_buffer = mules_client.update_buffer(data_buffer, new_data);
            // Data from one channel
            float average = 0;
            for (int ixr = 0; ixr < n_samples; ixr += 1)
            {
                data_buffer_1ch[ixr] = data_buffer[ixr, channel - 1];
                average += data_buffer_1ch[ixr] / n_samples;
            }
            // Plot data
            for (int ix_sample = 0; ix_sample < n_samples; ix_sample += 1)
            {
                line_renderer.SetPosition(ix_sample, new Vector3((x_scale * ix_sample) + x_offset,
                                                                 (y_scale * (data_buffer_1ch[ix_sample] - average) ) + y_offset,
                                                                 0));
            }
        }
    }

    public void OnButtonDown()
    // This method is called by the OnClick() method in the stop_button
    // Set in the Inspector
    // Close connection with MuLES
    {
        mules_client.tone(audio_source, 600, 250);
        mules_client.disconnect();
        System.Threading.Thread.Sleep(3000);
        UnityEditor.EditorApplication.isPlaying = false;
    }

}
