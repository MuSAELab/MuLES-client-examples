using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Diagnostics;

public class auto_start_mules : MonoBehaviour
{
    static Text text_title;
    int channel = 4;
    // plot scale and position
    float x_scale = 0.2f;
    float x_offset = 500f;
    float y_scale = 0.3f;
    float y_offset = 300f;
    LineRenderer line_renderer;

    void Start()
    {
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

        // 1. Start MuLES instance
        //Process foo = new Process();
        string mules_path = @"C:\Program Files (x86)\MuSAE_Lab\MuLES\mules.exe";

        // Data from a given device DEVICEXX
        // Process.Start(mules_path, " -- " + "\"" + "DEVICE06" + "\"" + " PORT=30001 LOG=F TCP=T");

        // Data from a CSV file
        string data_file = @"C:\Program Files (x86)\MuSAE_Lab\MuLES\eeg_files\log20141210_195303.csv";
        Process.Start(mules_path, " -- " + "\"" +  data_file + "\"" + " PORT=30001 LOG=F TCP=T");

        // Allow MuLES to start    
        System.Threading.Thread.Sleep(5000);

        // audio source for tones
        AudioSource audio_source = gameObject.AddComponent<AudioSource>();

        // 2. Connection with MuLES
        MulesClient mules_client = new MulesClient("127.0.0.1", 30001); // connects with MuLES at 127.0.0.1 : 30001
        string device_name = mules_client.getdevicename();              // get device name
        string[] channel_names = mules_client.getnames();               // get channel names
        double fs = mules_client.getfs();                               // get sampling frequency

        UnityEngine.Debug.Log("Data from: " + device_name);
        text_title.text = "Data from: " + device_name;

        // 3. Request 10 seconds of EEG data
        mules_client.tone(audio_source, 600, 250);
        float[,] data = mules_client.getdata(10);
        mules_client.tone(audio_source, 900, 250);
        int n_samples = data.GetUpperBound(0) + 1;
        // Plot data from one channel
        float[] data_1ch_1 = new float[n_samples];
        float average_1 = 0;
        for (int ixr = 0; ixr < n_samples; ixr += 1)
        {
            average_1 += data[ixr, channel - 1] / n_samples;
        }
        line_renderer.positionCount = n_samples;
        UnityEngine.Debug.Log("EEG data from: " + device_name + ". Electrode: " + channel_names[channel - 1]);
        text_title.text = "EEG data from: " + device_name + ". Electrode: " + channel_names[channel - 1];
        // Plot data 1
        for (int ix_sample = 0; ix_sample < n_samples; ix_sample += 1)
        {
            line_renderer.SetPosition(ix_sample, new Vector3((x_scale * ix_sample) + x_offset,
                                                             (y_scale * (data[ix_sample, channel-1] - average_1)) + y_offset,
                                                             0));
        }
        System.Threading.Thread.Sleep(2000);

        // 4. Close connection and close MuLES instance
        mules_client.kill();
    }
}
