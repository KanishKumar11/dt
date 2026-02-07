#!/bin/sh
# Set ASPNETCORE_URLS from HTTP_PORTS if provided by the platform (e.g., Coolify)
if [ -n "$HTTP_PORTS" ]; then
  export ASPNETCORE_URLS="http://+:${HTTP_PORTS}"
else
  # Fall back to an existing ASPNETCORE_URLS or default to port 80
  export ASPNETCORE_URLS="${ASPNETCORE_URLS:-http://+:80}"
fi

exec dotnet MyApi.dll
