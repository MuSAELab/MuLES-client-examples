The communication between MuLES and the client examples is handled by the [MulesClient class](https://github.com/rcassani/MuLES-client-examples/blob/main/unity3d/Assets/Scripts/MulesClient.cs).

The Unity3D project is comprised of 3 scenes, each one correspond to one of the following client example.

## Example 01: simple_client
This example shows the utilization of MuLES to:
- Start acquisition of data from one device
- Stream the acquire data to MATLAB
- Online plotting the data steam 

<p align="center">
<img src="https://user-images.githubusercontent.com/8238803/104816501-ac05f480-57e9-11eb-9abe-6a16250c1c8f.gif" width="900">
</p>

## Example 02: data_two_intervals
This example shows the utilization of MuLES to:
- Start acquisition of data from one device
- Get data from MuLES during two periods of 15 and 10 seconds
- Send triggers that will be reflected in the saved data

![unity02](https://user-images.githubusercontent.com/8238803/104816503-af00e500-57e9-11eb-8894-a75935a4afb4.PNG)

## Example 03: auto_start_mules
This example shows how to:
- Start programatically MuLES instances to acquired data from a given device or file
- Connect to MuLES instance
- Stream data for 10 s
- Close programatically MuLES instances  

![unity_03](https://user-images.githubusercontent.com/8238803/104816505-b1633f00-57e9-11eb-8ff9-1fd4a8cc724e.PNG)


