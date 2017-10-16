#!/bin/bash

####
# Tracing demo script for the Tracing Summit 2017.
# Author: Brian Robbins
# E-mail: brianrob@microsoft.com
####

RESULTSDIR=results
PERFCOLLECT=supportfiles/perfcollect
FLAMEGRAPHTOOLS=supportfiles/FlameGraph

WriteLine()
{
    # Set color to green.
    tput setaf 2

    # Write the message.
    echo "$1"

    # Reset color.
    tput sgr0

}

WriteLineAndWaitForEnter()
{
    # Set color to yellow.
    tput setaf 3

    read -p "$1"

    # Reset color.
    tput sgr0
}

# Print header.
WriteLine "**********************************************"
WriteLine "**********************************************"
WriteLine "****                                      ****"
WriteLine "**** Tracing Summit 2017 .NET Demo Script ****"
WriteLine "****                                      ****"
WriteLine "**********************************************"
WriteLine "**********************************************"
WriteLine

# Print a description of what this script will do.
WriteLine "This script will perform the following actions:"
WriteLine ""
WriteLine "1. Collect a performance trace using perfcollect.  Under the covers, this uses perf and LTTng."
WriteLine "2. Create an On-CPU FlameGraph of the data collected using perf."
WriteLine "3. Process the LTTng trace and create GCStats and JITStats HTML reports using PerfViewCollect."

# Wait for confirmation to continue.
WriteLine ""
WriteLineAndWaitForEnter "Press enter to continue."

# Step 0: Clean and create the results directory.
WriteLine ""
WriteLine "***********************************************"
WriteLine "Step 0: Clean and create the results directory."
WriteLine "***********************************************"
WriteLine ""
if [ -d $RESULTSDIR ]
then
    rm -rf $RESULTSDIR
    WriteLine "Deleted old results directory."
fi

mkdir $RESULTSDIR
WriteLine "Created new results directory."


# Step 1: Collect a performance trace using perfcollect.
WriteLine ""
WriteLine "******************************************************"
WriteLine "Step 1: Collect a performance trace using perfcollect."
WriteLine "******************************************************"
WriteLine ""

# Remind the user to set the necessary complus variables.
WriteLine "Make sure the following environment variables are set for the application being profiled:"
WriteLine "export COMPlus_PerfMapEnabled=1"
WriteLine "export COMPlus_EnableEventLog=1"
WriteLine ""
WriteLineAndWaitForEnter "Press enter to continue."

# Collect data using perf and LTTng.
WriteLine "Running perfcollect using default collection parameters."
sudo $PERFCOLLECT collect demo

# Move the trace into the results directory.
mv demo.trace.zip $RESULTSDIR

# Press enter to continue to step 2.
WriteLineAndWaitForEnter "Press enter to continue."

# Step 2: Create an On-CPU FlameGraph.
WriteLine ""
WriteLine "************************************"
WriteLine "Step 2: Create an On-CPU FlameGraph."
WriteLine "************************************"
WriteLine ""

# Unzip the demo file.
WriteLine "Extracting the trace file demo.trace.zip."
unzip $RESULTSDIR/demo.trace.zip -d $RESULTSDIR > /dev/null
WriteLine ""

# Run stackcollapse-perf.pl against the trace.
WriteLine "Runnning stackcollapse-perf.pl against the trace."
WriteLine "Command: perf script | ./stackcollapse-perf.pl > out.perf-folded"
perf script -i results/demo.trace/perf.data | $FLAMEGRAPHTOOLS/stackcollapse-perf.pl > results/out.perf-folded
WriteLine ""

WriteLine "Running cat out.perf-folded | ./flamegraph.pl"
cat $RESULTSDIR/out.perf-folded | $FLAMEGRAPHTOOLS/flamegraph.pl > $RESULTSDIR/demo-cpu.svg

# Press enter to continue to step 3.
WriteLine ""
WriteLineAndWaitForEnter "Press enter to continue."

# Step 3: Create GCStats and JITStats HTML Reports.
WriteLine ""
WriteLine "*************************************************"
WriteLine "Step 3: Create GCStats and JITStats HTML Reports."
WriteLine "*************************************************"
WriteLine ""

# Generate GCStats HTML report.
WriteLine "Running dotnet PerfViewCollect.dll UserCommand LinuxGCStats demo.trace.zip"
dotnet ~/src/perfview/src/PerfViewCollect/bin/Release/netcoreapp2.0/PerfViewCollect.dll UserCommand LinuxGCStats $RESULTSDIR/demo.trace.zip


# Run PerfViewCollect to generate GCStats
WriteLine ""
WriteLine "Running dotnet PerfViewCollect.dll UserCommand LinuxJITStats demo.trace.zip"
dotnet ~/src/perfview/src/PerfViewCollect/bin/Release/netcoreapp2.0/PerfViewCollect.dll UserCommand LinuxJITStats $RESULTSDIR/demo.trace.zip

# All Steps Completed.
WriteLine ""
WriteLine "*************************************************"
WriteLine "All steps completed."
WriteLine "Results available in `pwd`/$RESULTSDIR."
WriteLine "*************************************************"
WriteLine ""

