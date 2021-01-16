The communication between MuLES and the client examples is handled by the [MulesClient class](https://github.com/rcassani/MuLES-client-examples/blob/main/python/mules.py)

## Example 01: simple_client
This example shows the utilization of MuLES to:
- Start acquisition of data from one device
- Stream the acquire data to MATLAB
- Online plotting the data steam 

![python_01](https://user-images.githubusercontent.com/8238803/104816497-a4dee680-57e9-11eb-8c0d-a33d2a2b5ebf.gif)

## Example 02: data_two_intervals
This example shows the utilization of MuLES to:
- Start acquisition of data from one device
- Get data from MuLES during two periods of 15 and 10 seconds
- Send triggers that will be reflected in the saved data

![python_02](https://user-images.githubusercontent.com/8238803/104816499-a7d9d700-57e9-11eb-8a10-fd7bfa2363f3.PNG)

## Example 03: auto_start_mules
This example shows how to:
- Start programatically MuLES instances to acquired data from a given device or file
- Connect to MuLES instance
- Stream data for 10 s
- Close programatically MuLES instances  

![python_03](https://user-images.githubusercontent.com/8238803/104816500-a9a39a80-57e9-11eb-822d-0819fb41605d.PNG)


