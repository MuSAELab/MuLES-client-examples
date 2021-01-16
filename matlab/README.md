The communication between MuLES and the client examples is handled by the [MulesClient class](https://github.com/rcassani/MuLES-client-examples/blob/main/matlab/MulesClient.m)

## Example 01: simple_client
This example shows the utilization of MuLES to:
- Start acquisition of data from one device
- Stream the acquire data to MATLAB
- Online plotting the data steam 

![matlab_01](https://user-images.githubusercontent.com/8238803/104816486-9b557e80-57e9-11eb-8e73-ffa27c22cb85.gif)

## Example 02: data_two_intervals
This example shows the utilization of MuLES to:
- Start acquisition of data from one device
- Get data from MuLES during two periods of 15 and 10 seconds
- Send triggers that will be reflected in the saved data

![matlab_02](https://user-images.githubusercontent.com/8238803/104816490-9f819c00-57e9-11eb-863f-073a0d8a60da.PNG)

## Example 03: auto_start_mules
This example shows how to:
- Start programatically MuLES instances to acquired data from a given device or file
- Connect to MuLES instance
- Stream data for 10 s
- Close programatically MuLES instances  

![matlab_03](https://user-images.githubusercontent.com/8238803/104816493-a14b5f80-57e9-11eb-8bff-9fda75689a2c.PNG)
