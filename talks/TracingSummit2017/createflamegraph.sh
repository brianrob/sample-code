#!/bin/bash

# This script is adapted from http://www.brendangregg.com/perf.html#FlameGraphs.

# Download the FlameGraph utilities if necessary.
FlameGraphPath=~/src/FlameGraph
if [ ! -d "$FlameGraphPath" ]
then
    echo "Cloning FlameGraph tools."
    git clone https://github.com/BrendanGregg/FlameGraph ~/src/FlameGraph
else
    echo "FlameGraph tools already available.  Skipping download."
fi

# Assume perf.data is in the current working directory.
echo "Running perf script | stackcollapse-perf.pl"
perf script | $FlameGraphPath/stackcollapse-perf.pl > out.perf-folded

# Convert the folded output into a flamegraph svg.
echo "Running cat out.perf-folded | ./flamegraph.pl"
cat out.perf-folded | $FlameGraphPath/flamegraph.pl > perf-cpu.svg

echo "Created flame graph file perf-cpu.svg."
