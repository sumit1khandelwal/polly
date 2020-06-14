# Normal API Request Without Polly

http://localhost:57697/api/catalog/2

- Service throws 500 internal service error for 3 times
- Catalog controller returns successfull response 200 when only one of out four requests will succeed.
- /images/retrywithoutpolly.JPG

# Call Service with Retry Polly Policy

http://localhost:57697/api/retry/catalog/2

- Service applies retry policy to execute internally 3 times before sending response to caller.
- Catalog controller returns 200 in the same request.
- /images/retryWithPolly.JPG
