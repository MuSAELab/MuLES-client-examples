using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net.Sockets;


// Raymundo Cassani
// April 2017
// This file contains the MuLES class which hadles a TCP/IP connection to the 
// MuLES software, send commands, get data, send triggers among other functions.
// Methods:
//    __init__(ip, port)
//  connect()
//  disconnect()
//  kill()
//  sendcommand(command)
//  flushdata()
//  sendtrigger(trigger)
//  getparam()
//  getfs()
//  getdevicename()
//  getmessage()
//  getheadder()
//  parseheader(package)
//  getnames()
//  getalldata()
//  parsedata(package)
//  getdata(seconds, flush)
//  tone(frequency, duration_ms)

public class MulesClient
{
    // This class represents a TCP/IP client for the MuLES software.

    TcpClient client_socket = new TcpClient();
    //public string data_format;
    public string ip;
    public int port;
    public Dictionary<string, object> parameters = new Dictionary<string, object>();

    public MulesClient(string ip, int port)
    // Constructor method. This method connects to a MuLES Instance, request and 
    // retrieves the following information about the data acquisition device being used:
    //        Device name
    //        Device hardware
    //        Sampling frequency(samples/second)
    //        Data format
    //        Number of channels
    //        Extra parameters
    //
    //  Arguments:
    //        ip:   the IP adress to be used to connect to the MuLES Server.
    //        port: the port to use for a particular MuLES client. Every instance of MuLES should
    //              use a different port.To determine which port to use, please refer to the
    //              configuration file you are using for each instance of MuLES.
    {
        this.ip = ip;
        this.port = port;

        // TCP/IP connection
        connect();
        // Header information
        string[] header_fields = getheader();
        string[] channel_names = getnames();

        // Dictionary containing information about the device
        parameters.Add("device name", header_fields[0]);
        parameters.Add("device hardware", header_fields[1]);
        parameters.Add("sampling frequency", System.Convert.ToDouble(header_fields[2]));
        parameters.Add("data format", header_fields[3]);
        parameters.Add("number of channels", System.Convert.ToInt32(header_fields[4]));
        parameters.Add("names of channels", channel_names);
    }

    public void connect()
    // If, for some reason, the connection should be lost, this method can be used
    // to attempt to reconnect to the MuLES(Server). 
    {
        Debug.Log("Attempting connection");
        client_socket = new TcpClient();
        client_socket.Connect(ip, port);
        Debug.Log("Connection with the server " + ip + ":" + port + " OK");
    }

    public void disconnect()
    // This method shuts down the connection to the MuLES and sets client to None.
    // The connection parameters are preserved, so the connection can later be reestablished
    // by using the connect() method.
    {
        client_socket.Close();
        Debug.Log("Connection closed successfully");
    }

    public void kill()
    // This method send the command Kill to the MuLES software, which causes to end its execution
    {
        sendcommand("K");
    }

    void sendcommand(string command)
    // Sends an arbitrary command to the MuLES software
    //
    // Arguments:
    //        command: the command to be sent.
    {
        NetworkStream data_stream = client_socket.GetStream();
        byte[] to_send = System.Text.Encoding.UTF8.GetBytes(command);
        // Debug.Log("The command: " + command + " was sent");
        data_stream.Write(to_send, 0, to_send.Length);
    }

    public void flushdata()
    // This method flushes the data from the MuLES software.
    {
        sendcommand("F"); 
        Debug.Log("Flush Command");
    }

    public void sendtrigger(int trigger)
    // Send a trigger to the MuLES software.
    // Arguments:
    //        trigger: the trigger to be sent, it has to be in the range[1 64].
    {
        //Ignore Trigger
        if (trigger > 64 || trigger <= 0) { return; }
        NetworkStream data_stream = client_socket.GetStream();
        byte[] to_send = BitConverter.GetBytes(trigger);
        Debug.Log("The trigger: " + trigger + " was sent");
        data_stream.Write(to_send, 0, 1);
    }

    public Dictionary<string, object> getparams()
    // Returns the data acquisition device's parameters. These are stored in a dictionary.
    //    To obtain a value from the dictionary, the following strings should be used:
    //        'device name'
    //        'device hardware'
    //        'sampling frequency'
    //        'data format'
    //        'number of channels'
    // Returns:
    //        A dictionary containing information about the device.
    {
        return parameters;
    }

    public double getfs()
    // Retrieves sampling frequency 'fs' [Hz]
    {
        return (double)parameters["sampling frequency"];
    }

    public string getdevicename()
    // Retrieves the name of the device
    {
        return (string)parameters["device name"];
    }

    byte[] getmessage()
    // This gets a Message sent by MuLES an returns a byte array with the Message content
    {
        NetworkStream data_stream = client_socket.GetStream();
        // Reads the first 4 bytes (Int32) that indicate the data package size
        byte[] to_read_size = new byte[4];
        data_stream.Read(to_read_size, 0, 4);
        Array.Reverse(to_read_size);
        int pack_size = BitConverter.ToInt32(to_read_size, 0);
        // pack_size ins the size in bytes of the following data package
        //Debug.Log("Bytes to read: " + pack_size.ToString());
        // Reads the data package
        byte[] package = new byte[pack_size];
        data_stream.Read(package, 0, pack_size);
        return package;
    }

    public string[] getheader()
    // Request and Retrieves Header Information from MuLES 
    {
        sendcommand("H");
        string package = System.Text.Encoding.ASCII.GetString(getmessage());
        return parseheader(package);
    }

 
    string[] parseheader(string package)
    // This function parses the Header Package sent by MuLES to obtain the
    // device's parameters. NAME, HARDWARE, FS, DATAFORMAT, #CH, EXTRA      
    // Argument:
    //        package: Header package sent by MuLES.
    {
        string[] header_fields = new string[5];
        string[] header_entries = package.Split(',');       
        foreach (string header_entry in header_entries)
        {
            string[] key_value = header_entry.Split('=');       
            string key = key_value[0];           
            switch (key)
            {
                case "NAME":
                    header_fields[0] = key_value[1];
                    break;
                case "HARDWARE":
                    header_fields[1] = key_value[1];
                    break;
                case "FS":
                    header_fields[2] = key_value[1];
                    break;
                case "DATA":
                    header_fields[3] = key_value[1];
                    break;
                case "#CH":
                    header_fields[4] = key_value[1];
                    break;
            }
        }
        return header_fields;
    }

    public string[] getnames()
    // Request and Retrieves the names of channels from MuLES 
    {
        sendcommand("N");
        string package = System.Text.Encoding.ASCII.GetString(getmessage());
        return package.Split(',');
    }

    public float[,] getalldata()
    // Request and Retrieves ALL Data present in MuLES buffer 
    // (Data collected since the last Flush or last DataRequest)
    // in the shape [samples, channels] 
    {
        sendcommand("R");
        byte[] package = getmessage();
        return parsedata(package);
    }

    float[,] parsedata(byte[] package)
    // This function parses the Data Package sent by MuLES to obtain all the data 
    // available in MuLES as matrix of the size[n_samples, n_columns], therefore the
    // total of elements in the matrix is n_samples* n_columns.Each column represents
    // one channel
    //
    // Argument:
    //        package: Data package sent by MuLES.
    {
        string data_format = (string)parameters["data format"];
        //How many samples are contained in data
        int samples = package.Length / (data_format.Length * 4);
        float[,] data_array = new float[samples, data_format.Length];
        byte[] tmp_bytes = new byte[4];
        float tmp = 0;
        int index_data = 0;

        for (int ins = 0; ins < samples; ins += 1)
        {
            for (int inx = 0; inx < data_format.Length; inx += 1)
            {
                index_data = ins * (data_format.Length * 4) + (inx * 4);
                Array.Copy(package, index_data, tmp_bytes, 0, 4);
                Array.Reverse(tmp_bytes);
                //switch depending of data format
                char decode_type = data_format[inx];
                switch (decode_type)
                {
                    case 'f':
                        tmp = BitConverter.ToSingle(tmp_bytes, 0);
                        break;
                    case 'i':
                        tmp = (float)BitConverter.ToInt32(tmp_bytes, 0);
                        break;
                }
                data_array[ins, inx] = tmp;
            }
        }
        return data_array;
    }

    public float[,] getdata(float seconds, bool flush = true)
    // Flush all the Data present in MuLES buffer and, 
    // Request and Retrieve a certain amount of Data indicated as seconds
    // Data returned has the shape[seconds * sampling_frequency, channels]
    //
    // Argument:
    //        seconds: used to calculate the amount of samples requested n_samples
    //                 n_samples = seconds * sampling_frequency
    //        flush:   Boolean, if True send the command Flush before getting Data,
    //                 Default = True
    {
        if (flush){flushdata();}
        // Size of data requested
        int n_samples = (int)(seconds * getfs());
        int n_columns = ((string)parameters["data format"]).Length;
        float[,] data_buffer = new float[n_samples, n_columns];
        for (int ixr = 0; ixr < n_samples; ixr += 1)
        {
            for (int ixc = 0; ixc < n_columns; ixc += 1)
            {
                data_buffer[ixr, ixc] = -1;
            }
        }
        // While the first row has not been rewriten
        while (data_buffer[0,n_columns-1] == -1)
        {
            float[,] new_data = getalldata();
            int new_samples = new_data.GetUpperBound(0) + 1;
            data_buffer = update_buffer(data_buffer, new_data);           
        }
        return data_buffer;
    }

    public float [,] update_buffer(float[,] array1, float[,] array2)
    // concatenate array2 to array 1 in the 1st dimension (rows)
    // returns a 2D array of the same size as array1, that consists on the bottom part to array3
    {
        int array1_nr = array1.GetUpperBound(0) + 1;
        int arrays_nc = array1.GetUpperBound(1) + 1;
        int array2_nr = array2.GetUpperBound(0) + 1;
        float[,] array3 = new float[array1_nr + array2_nr, arrays_nc];
        // copy array 1
        for (int ixr = 0; ixr < array1_nr; ixr += 1)
        {
            for (int ixc = 0; ixc < arrays_nc; ixc += 1)
            {
                array3[ixr, ixc] = array1[ixr, ixc];
            }
        }
        // copy array 2
        for (int ixr = 0; ixr < array2_nr; ixr += 1)
        {
            for (int ixc = 0; ixc < arrays_nc; ixc += 1)
            {
                array3[ixr + array1_nr, ixc] = array2[ixr, ixc];
            }
        }
        // get bottom part of array3
        for (int ixr = 0; ixr < array1_nr; ixr += 1)
        {
            for (int ixc = 0; ixc < arrays_nc; ixc += 1)
            {
                array1[ixr, ixc] = array3[ixr + array2_nr, ixc];
            }
        }
        return array1;
    }


    public void tone(AudioSource aud, float frequency, int duration_ms)
    // Plays a pure tone of a certain frequency and duration in 
    // miliseconds.Tone is sampled at 48 kHz
    //
    // Argument:
    //        frequency:    tone frequency in Hz
    //        duration_ms:  tone duration in miliseconds
    {
        int fs = 48000;

        float[] samples = new float[(int)(duration_ms * fs / 1000)];
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = Mathf.Sin(Mathf.PI * 2 * i * frequency / fs);
            
        }
        AudioClip myClip = AudioClip.Create("Test", samples.Length, 1, fs, false);
        myClip.SetData(samples, 0);
        aud.PlayOneShot(myClip);
        return;
    }
}