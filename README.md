# RateLimiter

## Run solution from terminal
````
    runme.bat
````

## Exit from client/server
````
    Press Ctrl-C
````

## Server configuration: appsettings.json
````json
{
  "FixedFramePolicyOptions": {
    "PermitLimit": 5,
    "FrameInSeconds": 5
  }
}
````

## Client configuration: appsettings.json
````json
{
  "RateLimiterClientOptions": {
    "ClientCount": 5,
    "MaxIntervalSeconds": 1
  }
}
````

