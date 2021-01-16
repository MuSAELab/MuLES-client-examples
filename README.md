# MuLES-client-examples

This repository is comprised of 3 examples of clients for MuLES.
Each of the examples has been implemented in [MATLAB](https://github.com/rcassani/MuLES-client-examples/tree/main/matlab), [Python3](https://github.com/rcassani/MuLES-client-examples/tree/main/python) and [Unity3D (C#)](https://github.com/rcassani/MuLES-client-examples/tree/main/unity3d). A standard [installation of MuLES](https://github.com/MuSAELab/MuLES/releases/tag/v1.4) is required.

The examples in this repository are:
1. simple_client
2. data_two_intervals
3. auto_start_mules

The description of each experiment is below.

## Example 01: simple_client
This example shows the utilization of MuLES to:
- Start acquisition of data from one device
- Stream the acquire data to MATLAB
- Online plotting the data steam 

## Example 02: data_two_intervals
This example shows the utilization of MuLES to:
- Start acquisition of data from one device
- Get data from MuLES during two periods of 15 and 10 seconds
- Send triggers that will be reflected in the saved data

## Example 03: auto_start_mules
This example shows how to:
- Start programatically MuLES instances to acquired data from a given device or file
- Connect to MuLES instance
- Stream data for 10 s
- Close programatically MuLES instances  

