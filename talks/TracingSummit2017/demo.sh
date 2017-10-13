#!/bin/bash

# Remind the user to set the necessary complus variables.
echo "Make sure the following environment variables are set for the application being profiled:"
echo "export COMPlus_PerfMapEnabled=1"
echo "export COMPlus_EnableEventLog=1"
echo ""
read -p "Press enter to continue."

# Collect data using perf and LTTng.
echo "Running perfcollect using default collection parameters."
sudo ./perfcollect collect demo

# Unzip the demo file.
unzip demo.trace.zip

# Create a flamegraph out of the CPU trace.
cp demo.trace/perf.data .
./createflamegraph.sh
rm perf.data

# Run PerfViewCollect to generate GCStats
dotnet ~/src/perfview/src/PerfViewCollect/bin/Release/netcoreapp2.0/PerfViewCollect.dll UserCommand LinuxGCStats demo.trace.zip

echo ""
echo ""

# Run PerfViewCollect to generate GCStats
dotnet ~/src/perfview/src/PerfViewCollect/bin/Release/netcoreapp2.0/PerfViewCollect.dll UserCommand LinuxJitStats demo.trace.zip

echo ""
echo ""
echo "Completed."
