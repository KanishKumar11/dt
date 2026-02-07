#!/bin/sh
# Entrypoint for the .NET app - no URL setting needed, handled in Program.cs
exec dotnet MyApi.dll
