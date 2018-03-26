#!/bin/bash

echo "DOIN SOME RESTORIN"
dotnet restore
echo "AIGH ALL RESTORED"

if [ $TRAVIS_PULL_REQUEST == "false" ]
then
    echo "AIIIGHHTTT NOW WE CAN DEPLOY YO."
    echo "BUILDIN THAT RELEASE MODE YO"
    dotnet build -c Release
else
    echo "AIIIGHHTTT WE PULLIN. SO WE AINT DEPLOYIN."
    echo "WE BUILDIN DEBUG"
    dotnet build -c Debug
fi
echo "AIGHT SHE ALL BUILT UP"


echo "RUNNIN SOME TESTIES"

# Runs tests on on test files
for PROJ in `find -name *MSTest.csproj`;
    do
    echo $PROJ
	dotnet test $PROJ
    done

echo "TESTIES are all done"

if [ $TRAVIS_PULL_REQUEST == "false" ]
then
    cd ./CatiLyfe.Backend.Web.Core
    dotnet publish -c Release
    cd ./bin/Release/netcoreapp2.0/publish
    git init
    DEPLOY_URL="https://$CL_AZURE_DEPLOYMENT_USER:$CL_AZURE_DEPLOYMENT_USER_PASSWORD@caticake.scm.azurewebsites.net/caticake.git"
    echo "$DEPLOY_URL"
    git remote add azure "$DEPLOY_URL"
    git add .
    git commit -m "Deployin this lil guy."
    git push -f azure master
    echo "ALL DONE NOW"
fi
