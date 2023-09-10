# RateLimiter

## Run solution from terminal
````
    runme.bat
````

## Server configuration
````json
{
  "FixedFramePolicyOptions": {
    "PermitLimit": 5,
    "FrameInSeconds": 5
  }
}
````

## Client configuration
````json
{
  "RateLimiterClientOptions": {
    "ClientCount": 5,
    "MaxIntervalSeconds": 1
  }
}
````

