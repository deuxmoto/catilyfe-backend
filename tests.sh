#!/bin/bash

# Runs tests on on test files
for PROJ in `find -name *MSTest.csproj`;
    do
    echo $PROJ
	dotnet test $PROJ
    done