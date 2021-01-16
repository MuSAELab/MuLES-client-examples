using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class data_two_intervals : MonoBehaviour
{
    static Text text_title;
    int channel = 4;
    // plot scale and position
    float x_scale = 0.1f;
    float x_offset = 500f;
    float y_scale = 0.2f;
    float y_offset_1 = 400f;
    float y_offset_2 = 100f;
    LineRenderer line_renderer_1;
    LineRenderer line_renderer_2;

    // Use this for initialization
    void Start()
    {
        MulesClient mules_client = new MulesClient("127.0.0.1", 30000);
        // audio source for tones
        AudioSource audio_source = gameObject.AddComponent<AudioSource>();
        // line renderers and their properties for plots
        GameObject obj_1 = new GameObject("line_1");
        GameObject obj_2 = new GameObject("line_2");
        line_renderer_1 = obj_1.AddComponent<LineRenderer>();
        line_renderer_2 = obj_2.AddComponent<LineRenderer>();

        line_renderer_1.material = new Material(Shader.Find("Sprites/Default"));
        line_renderer_2.material = new Material(Shader.Find("Sprites/Default"));

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        line_renderer_1.colorGradient = gradient;
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        line_renderer_2.colorGradient = gradient;

        text_title = GameObject.Find("text_title").GetComponent<Text>();
        text_title.text = "";

        string device_name = mules_client.getdevicename();           // get device name
        string [] channel_names = mules_client.getnames();           // get channel names
        double fs = mules_client.getfs();                            // get sampling frequency

        // 2. Sending trigger 10    
        mules_client.sendtrigger(10);
        mules_client.tone(audio_source, 600, 250);

        // 3. Request 15 seconds of EEG data
        float[,] data_1 = mules_client.getdata(15);
        int n_samples_1 = data_1.GetUpperBound(0) + 1;
        // Data from one channel
        float[] data_1ch_1 = new float[n_samples_1];
        float average_1 = 0;
        for (int ixr = 0; ixr < n_samples_1; ixr += 1)
        {
            data_1ch_1[ixr] = data_1[ixr, channel - 1];
            average_1 += data_1ch_1[ixr] / n_samples_1;
        }
        line_renderer_1.positionCount = n_samples_1;

        // 4. Sending trigger 20    
        mules_client.sendtrigger(20);
        mules_client.tone(audio_source, 600, 250);

        // 5. Request 10 seconds of EEG data
        float[,] data_2 = mules_client.getdata(10);
        int n_samples_2 = data_2.GetUpperBound(0) + 1;
        // Data from one channel
        float[] data_1ch_2 = new float[n_samples_2];
        float average_2 = 0;
        for (int ixr = 0; ixr < n_samples_2; ixr += 1)
        {
            data_1ch_2[ixr] = data_2[ixr, channel - 1];
            average_2 += data_1ch_2[ixr] / n_samples_2;
        }
        line_renderer_2.positionCount = n_samples_2;

        // 6. Sending trigger 30    
        mules_client.sendtrigger(30);
        mules_client.tone(audio_source, 900, 250);

        // 7. Close connection with MuLES
        mules_client.disconnect();

        // 8. Plot results
        Debug.Log("EEG data from: " + device_name + ". Electrode: " + channel_names[channel - 1]);
        text_title.text = "EEG data from: " + device_name + ". Electrode: " + channel_names[channel - 1];
        // Plot data 1
        for (int ix_sample = 0; ix_sample < n_samples_1; ix_sample += 1)
        {
            line_renderer_1.SetPosition(ix_sample, new Vector3((x_scale * ix_sample) + x_offset,
                                                             (y_scale * (data_1ch_1[ix_sample] - average_1)) + y_offset_1,
                                                             0));
        }
        // Plot data 2
        for (int ix_sample = 0; ix_sample < n_samples_2; ix_sample += 1)
        {
            line_renderer_2.SetPosition(ix_sample, new Vector3((x_scale * ix_sample) + x_offset,
                                                             (y_scale * (data_1ch_2[ix_sample] - average_2)) + y_offset_2,
                                                             0));
        }
    }
}
