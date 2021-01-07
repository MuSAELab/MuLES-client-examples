# -*- coding: utf-8 -*-
"""
MuLES client example: data_two_intervals
This example shows the utilization of MuLES to:
- Start acquisition of data from one device
- Get data from MuLES during two periods of 15 and 10 seconds
- Send triggers that will be reflected in the saved data

The scrip is divided as follows
1. Connection with MuLES, and retrieve EEG data parameters
2. A trigger is sent (trigger = 10) + beep
3. Request 15 seconds of EEG data
4. A trigger is sent (trigger = 20) + beep
5. Request 10 seconds of EEG data
6. A trigger is sent (trigger = 30) + beep
7. Acquisition is finished
8. Plot acquired signals

 Instructions:
 (MuLES and the Client are expected to be in the same computer, if that is not 
 the case, modify ip address, in Section 1 of this script)

 1 Run MuLES
 2 Select your device 
   (Alternatively you can select FILE and the example recording:
    log20141210_195303.csv)
 3 Select Streamming and Logging
   (In casse of reading from a File, You cannot change these options)
 4 Click on PLAY
 5 Run this script
"""


import mules              # The signal acquisition toolbox we'll use (MuLES)
import numpy as np        # Module that simplifies computations on matrices 
import matplotlib.pyplot as plt # Module used for plotting


if __name__ == "__main__":
    
    plt.close('all')    
    
    # 1. Acquisition is started
    # creates mules_client object and:
    mules_client = mules.MulesClient('127.0.0.1', 30000) # connects with MuLES at 127.0.0.1 : 30000
    device_name = mules_client.getdevicename()           # get device name
    channel_names = mules_client.getnames()              # get channel names
    fs = 1.0 * mules_client.getfs()                      # get sampling frequency

    
    # 2. Sending trigger 10    
    mules_client.sendtrigger(10)
    mules_client.tone(600,250)

    # 3. Request 15 seconds of EEG data
    eeg_data_1 = mules_client.getdata(15)

    # 4. Sending trigger 20    
    mules_client.sendtrigger(20)
    mules_client.tone(600,250)

    # 5. Request 10 seconds of EEG data
    eeg_data_2 = mules_client.getdata(10)

    # 6. Sending trigger 30    
    mules_client.sendtrigger(30)
    mules_client.tone(900,250)

    # 7. Close connection with MuLES
    mules_client.disconnect()

    # 8. Plot results
    time_vector_1 =  np.arange(0,eeg_data_1.shape[0]) / fs
    time_vector_2 =  np.arange(0,eeg_data_2.shape[0]) / fs
    channel = 4;
    h, axarr = plt.subplots(2, sharex=True)
    h.canvas.set_window_title('EEG data from: ' + device_name + '. Electrode: ' + channel_names[channel-1]    )
    axarr[0].plot(time_vector_1, eeg_data_1[:,channel - 1])
    axarr[1].plot(time_vector_2, eeg_data_2[:,channel - 1])
