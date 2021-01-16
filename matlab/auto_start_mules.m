% MuLES client example: auto_start_mules
% This example shows how to:
% - Start a MuLES instance from MATLAB 
% - Connect to MuLES instance
% - Stream data for 10 s
% - Close a MuLES instance from MATLAB  
%
% The scrip is divided as follows
% 1. Start MuLES instance
% 2. Connection with MuLES
% 3. Get 10 s of data
% 4. Close connection and close MuLES instance
% 
%  Instructions:
%  MuLES and the Client are expected to be in the same computer
% 
%  1 Run this script

close all
clear all

% 1. Start MuLES instance
mules_exe_path = '"C:\Program Files (x86)\MuSAE_Lab\MuLES\mules.exe"';

% Data from a given device DEVICEXX
% system( [mules_exe_path, ' -- "DEVICE06" PORT=30001 LOG=F TCP=T &']);

% Data from a CSV file
data_file = '"C:\Program Files (x86)\MuSAE_Lab\MuLES\eeg_files\log20141210_195303.csv"';
system( [mules_exe_path, ' -- ', data_file, ' PORT=30001 LOG=F TCP=T &']);

% Allow MuLES to start 
pause(5)

% 2. Connection with MuLES
mules_client = MulesClient('127.0.0.1', 30001); % connects with MuLES at 127.0.0.1 : 30001
device_name = mules_client.getdevicename();     % get device name
channel_names = mules_client.getnames();        % get channel names
fs = mules_client.getfs();                      % get sampling frequency

% 3. Request 10 seconds of data
mules_client.tone(600,250);
eeg_data = mules_client.getdata(10);
mules_client.tone(900,250);
time_vector = (1:size(eeg_data,1)) / fs;
channel = 4;
h = figure('name',['EEG data from: ', device_name, '. Electrode: ', channel_names{channel}]);
plot(time_vector, eeg_data(:,channel));


% 4. Close connection and close MuLES instance
mules_client.kill()