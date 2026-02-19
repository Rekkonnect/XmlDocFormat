# This script is intended to be invoked as the first build command
# to allow all projects to be successfully built due to the lack of
# an easy way in MSBuild to declare that a package will be created
# and properly restored by the time other projects get restored.

Echo $PSScriptRoot

echo "Building the XmlDocFormat.InternalGenerators and packing"
dotnet build $PSScriptRoot/../../tools/XmlDocFormat.InternalGenerators/XmlDocFormat.InternalGenerators.csproj --configuration Release

echo "Building the entire solution"
dotnet build $PSScriptRoot/../../
